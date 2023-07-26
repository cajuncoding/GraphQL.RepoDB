#nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Data.Projections.Context;

namespace HotChocolate.PreProcessingExtensions.Selections
{
    public static class IResolverContextSelectionExtensions
    {
        /// <summary>
        /// Similar to CollectFields in v10, this uses GetSelections but safely validates that the current context
        ///     has selections before returning them, it will safely return null if unable to do so.
        /// This is a variation of the helper method provided by HotChocolate team here: 
        ///     https://github.com/ChilliCream/hotchocolate/issues/1527#issuecomment-596175928
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IReadOnlyList<PreProcessingSelection> GetPreProcessingSelections(this IResolverContext? context)
        {
            if (context == null)
                return null!;

            var selectionResults = new List<PreProcessingSelection>();

            var selections = GatherChildSelections(context!);
            if (selections.Any())
            {
                //BBernard
                //Determine if the Selection is for a Connection, and dive deeper to get the real
                //  selections from the node {} field.
                var lookup = selections.ToLookup(s => s.SelectionName.ToString().ToLower());

                //Handle paging cases; current Node is a Connection so we have to look for selections inside
                //  ->edges->nodes, or inside the ->nodes (shortcut per Relay spec); both of which may exist(?)
                if (lookup.Contains(SelectionNodeName.Nodes) || lookup.Contains(SelectionNodeName.Edges) || lookup.Contains(SelectionNodeName.Items))
                {
                    //Cursor & Offset Paging are mutually exclusive so this small optimization prevents unnecessary processing...
                    var searchOffsetPagingEnabled = true;

                    //CURSOR PAGING SUPPORT - results are in either a 'Nodes' or 'Edges' Node!
                    //NOTE: nodes and edges are not mutually exclusive per Relay spec so
                    //          we gather from all if they are defined...
                    if (lookup.Contains(SelectionNodeName.Nodes))
                    {
                        var nodesSelectionField = lookup[SelectionNodeName.Nodes].FirstOrDefault();
                        var childSelections = GatherChildSelections(context, nodesSelectionField);
                        selectionResults.AddRange(childSelections);

                        searchOffsetPagingEnabled = false;
                    }

                    if (lookup.Contains(SelectionNodeName.Edges))
                    {
                        var edgesSelectionField = lookup[SelectionNodeName.Edges].FirstOrDefault();
                        //If Edges are specified then Selections are actually inside a nested 'Node' (singular, not plural) that we need to traverse...
                        var nodesSelectionField = FindChildSelectionByName(context, SelectionNodeName.EdgeNode, edgesSelectionField);
                        var childSelections = GatherChildSelections(context, nodesSelectionField);
                        selectionResults.AddRange(childSelections);
                        
                        searchOffsetPagingEnabled = false;
                    }

                    //OFFSET PAGING SUPPORT - results are in an 'Items' Node!
                    if (searchOffsetPagingEnabled && lookup.Contains(SelectionNodeName.Items))
                    {
                        var nodesSelectionField = lookup[SelectionNodeName.Items].FirstOrDefault();
                        var childSelections = GatherChildSelections(context, nodesSelectionField);
                        selectionResults.AddRange(childSelections);
                    }
                }
                //Handle Non-paging cases; current Node is an Entity...
                else
                {
                    selectionResults.AddRange(selections);
                }
            }

            return selectionResults;
        }

        public static PreProcessingSelection GetTotalCountSelectionField(this IResolverContext? context)
        {
            if (context == null)
                return null!;

            var totalCountSelectionField = FindChildSelectionByName(context!, SelectionFieldName.TotalCount, null);
            return totalCountSelectionField;
        }

        /// <summary>
        /// Find the selection that matches the specified name.
        /// For more info. on Node parsing logic see here:
        /// https://github.com/ChilliCream/hotchocolate/blob/a1f2438b74b19e965b560ca464a9a4a896dab79a/src/Core/Core.Tests/Execution/ResolverContextTests.cs#L83-L89
        /// </summary>
        /// <param name="context"></param>
        /// <param name="baseSelection"></param>
        /// <param name="selectionFieldName"></param>
        /// <returns></returns>
        private static PreProcessingSelection FindChildSelectionByName(IResolverContext? context, string selectionFieldName, PreProcessingSelection? baseSelection)
        {
            if (context == null)
                return null!;

            var childSelections = GatherChildSelections(context!, baseSelection);
            var resultSelection = childSelections?.FirstOrDefault(
                s => s.SelectionName.Equals(selectionFieldName, StringComparison.OrdinalIgnoreCase)
            )!;

            return resultSelection!;
        }

        /// <summary>
        /// Gather all child selections of the specified Selection
        /// For more info. on Node parsing logic see here:
        /// https://github.com/ChilliCream/hotchocolate/blob/a1f2438b74b19e965b560ca464a9a4a896dab79a/src/Core/Core.Tests/Execution/ResolverContextTests.cs#L83-L89
        /// </summary>
        /// <param name="context"></param>
        /// <param name="baseSelection"></param>
        /// <returns></returns>
        private static List<PreProcessingSelection> GatherChildSelections(IResolverContext? context, PreProcessingSelection? baseSelection = null)
        {
            if (context == null)
                return null!;

            //Initialize the optional base field selection if specified...
            //Dynamically support re-basing to the specified baseSelection or fallback to current Context.Selection
            var baseFieldSelection = baseSelection?.graphqlFieldSelection ?? context.GetSelectedField();

            //Following Logic for processing the SelectionContext was adapted from the HotChocolate Core Unit Tests:
            //  HotChocolate.Data.SelectionContextTests => GetFields_Should_ReturnAllTheSelectedFields()
            // NOTE: HC will throw an Exception if you attempt to get the Fields of an Abstract Type (Interface, Union, etc.)
            //          without specifying the type, and therefore due to all the possible combination of Lists, Connections, etc.
            //          along with Abstract Types it's safest and far more simple to always attempt to get possible types
            //          and enumerate them to then get the Fields for each possible type!
            var possibleTypes = context.Schema.GetPossibleTypes(baseFieldSelection.Type.NamedType());

            var gatheredSelections = possibleTypes
                .SelectMany(pt => baseFieldSelection.GetFields(pt))
                .Select(sf => new PreProcessingSelection(sf))
                .ToList();

            return gatheredSelections;
        }
    }
}
