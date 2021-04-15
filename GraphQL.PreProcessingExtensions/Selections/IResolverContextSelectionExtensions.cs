#nullable enable

using HotChocolate.Execution.Processing;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.PreProcessingExtensions.Selections;

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
                if (lookup.Contains(SelectionNodeName.Nodes) || lookup.Contains(SelectionNodeName.Edges))
                {
                    //NOTE: nodes and edges are not mutually exclusive per Relay spec so
                    //          we gather from all if they are defined...
                    if (lookup.Contains(SelectionNodeName.Nodes))
                    {
                        var nodesSelectionField = lookup[SelectionNodeName.Nodes].FirstOrDefault();
                        var childSelections = GatherChildSelections(context, nodesSelectionField);
                        selectionResults.AddRange(childSelections);
                    }

                    if (lookup.Contains(SelectionNodeName.Edges))
                    {
                        var edgesSelectionField = lookup[SelectionNodeName.Edges].FirstOrDefault();
                        var nodeSelectionField = FindChildSelectionByName(context, SelectionNodeName.EdgeNode, edgesSelectionField);
                        var childSelections = GatherChildSelections(context, nodeSelectionField);
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

        /// <summary>
        /// Find the selection that matches the speified name.
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
                s => s.SelectionName.ToString().Equals(selectionFieldName, StringComparison.OrdinalIgnoreCase)
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

            var gathered = new List<PreProcessingSelection>();

            //Initialize the optional base field selection if specified...
            var baseFieldSelection = baseSelection?.GraphQLFieldSelection;
            
            //Dynamically support re-basing to the specified baseSelection or fallback to current Context.Field
            var field = baseFieldSelection?.Field ?? context.Field;

            //Initialize the optional SelectionSet to rebase processing as the root for GetSelections()
            //  if specified (but is optional & null safe)...
            SelectionSetNode? baseSelectionSetNode = baseFieldSelection is ISelection baseISelection
                ? baseISelection.SelectionSet
                : null!;

            //Get all possible ObjectType(s); InterfaceTypes & UnitionTypes will have more than one...
            var objectTypes = GetObjectTypesSafely(field.Type, context.Schema);

            //Map all object types into PreProcessingSelection (adapter classes)...
            foreach (var objectType in objectTypes)
            {
                //Now we can process the ObjectType with the correct context (selectionSet may be null resulting
                //  in default behavior for current field.
                var childSelections = context.GetSelections(objectType, baseSelectionSetNode);
                var preprocessSelections = childSelections.Select(s => new PreProcessingSelection(objectType, s));
                gathered.AddRange(preprocessSelections);
            }

            return gathered;
        }

        /// <summary>
        /// ObjectType resolver function to get the current object type enhanced with support
        /// for InterfaceTypes & UnionTypes; initially modeled after from HotChocolate source:
        /// HotChocolate.Data -> SelectionVisitor`1.cs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectType"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static List<ObjectType> GetObjectTypesSafely(IType type, ISchema schema)
        {
            var results = new List<ObjectType>();
            switch (type)
            {
                case NonNullType nonNullType:
                    results.AddRange(GetObjectTypesSafely(nonNullType.NamedType(), schema));
                    break;
                case ObjectType objType:
                    results.Add(objType);
                    break;
                case ListType listType:
                    results.AddRange(GetObjectTypesSafely(listType.InnerType(), schema));
                    break;
                case InterfaceType interfaceType:
                    var possibleInterfaceTypes = schema.GetPossibleTypes(interfaceType);
                    var objectTypesForInterface = possibleInterfaceTypes.SelectMany(t => GetObjectTypesSafely(t, schema));
                    results.AddRange(objectTypesForInterface);
                    break;
                //TODO: TEST UnionTypes!
                case UnionType unionType:
                    var possibleUnionTypes = schema.GetPossibleTypes(unionType);
                    var objectTypesForUnion = possibleUnionTypes.SelectMany(t => GetObjectTypesSafely(t, schema));
                    results.AddRange(objectTypesForUnion);
                    break;
            }

            return results;
        }
    }
}
