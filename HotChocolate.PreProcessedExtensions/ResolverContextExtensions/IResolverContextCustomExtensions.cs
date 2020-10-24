#nullable enable

using HotChocolate.Execution.Processing;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace HotChocolate.PreProcessedExtensions
{
    public static class IResolverContextCustomExtensions
    {
        public static class SelectionNodeName
        {
            public const string Nodes = "nodes";
            public const string Edges = "edges";
            public const string EdgeNode = "node";
        }

        /// <summary>
        /// Get an Argument for the specified Name & Type safely; null (default) value will be returned instead
        /// of exceptions being thrown.
        /// </summary>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="context"></param>
        /// <param name="argName"></param>
        /// <returns></returns>
        public static TArg ArgumentValueSafely<TArg>(this IResolverContext context, string argName)
        {
            try
            {
                return context != null
                    ? context.ArgumentValue<TArg>(argName)
                    : default!;
            }
            catch
            {
                return default!;
            }
        }

        /// <summary>
        /// Similar to CollectFields in v10, this uses GetSelections but safely validates that the current context
        ///     has selections before returning them, it will safely return null if unable to do so.
        /// This is a variation of the helper method provided by HotChocolate team here: 
        ///     https://github.com/ChilliCream/hotchocolate/issues/1527#issuecomment-596175928
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IReadOnlyList<PreProcessingSelection> GetPreProcessingSelections(this IResolverContext context)
        {
            if (context == null)
                return null!;

            var selectionResults = new List<PreProcessingSelection>();

            //var hotChocolateNamedType = context?.Field.Type.NamedType();
            //ObjectType? currentObjectType = null;

            //if (hotChocolateNamedType is ObjectType currentObjectType)
            //{
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
        /// Retrieve the current Primary node selection names as strings (e.g. SELECT fields from data) 
        /// from the current GraphQL query.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<string> GetPreProcessingSelectionNames(this IResolverContext context)
        {
            return context?
                .GetPreProcessingSelections()?
                .Select(s => s.Name.ToString())
                .ToList()!;
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
        private static PreProcessingSelection FindChildSelectionByName(IResolverContext context, string selectionFieldName, PreProcessingSelection? baseSelection)
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
        private static List<PreProcessingSelection> GatherChildSelections(IResolverContext context, PreProcessingSelection? baseSelection = null)
        {
            if (context == null)
                return null!;

            var gathered = new List<PreProcessingSelection>();

            //Initialize the optional base field selection if specified...
            var baseFieldSelection = baseSelection?.FieldSelection;

            //Dynamically support re-basing to the specified baseSelection or fallback to current Context.Field
            var field = baseFieldSelection?.Field ?? context.Field;
            
            var namedType = field.Type.NamedType();
            switch (namedType)
            {
                case InterfaceType interfaceType:
                    if (interfaceType?.Fields.Any() ?? false)
                    {
                        var preprocessSelections = interfaceType.Fields.Select(f => new PreProcessingSelection(f));
                        gathered.AddRange(preprocessSelections);
                    }
                    break;

                case ObjectType objectType:
                    if (baseFieldSelection == null || baseFieldSelection is ISelection)
                    {
                        //Initialize the optional ISelection (e.g. SelectionSet) if specified (null safe)...
                        var selectionSet = (baseFieldSelection as ISelection)?.SelectionSet;

                        //Now we can process the ObjectType with the correct context (selectionSet may be null resulting
                        //  in default behavior for current field.
                        var childSelections = context.GetSelections(objectType, selectionSet);
                        var preprocessSelections = childSelections.Select(s => new PreProcessingSelection(s));
                        gathered.AddRange(preprocessSelections);
                    }
                    break;
            }


            return gathered;
        }

        ///// <summary>
        ///// Gather/Collect ALL selectins from the current node down, recursively.
        ///// For more info. on Node parsing logic see here:
        ///// https://github.com/ChilliCream/hotchocolate/blob/a1f2438b74b19e965b560ca464a9a4a896dab79a/src/Core/Core.Tests/Execution/ResolverContextTests.cs#L83-L89
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="fieldSelection"></param>
        ///// <param name="gatherRecursively"></param>
        ///// <returns></returns>
        //private static List<PreProcessingSelection> GatherRecursiveSelections(IResolverContext context, IFieldSelection? fieldSelection = null)
        //{
        //    var gathered = new List<PreProcessingSelection>();
        //    var field = fieldSelection?.Field ?? context.Field;

        //    if (fieldSelection != null && field.Type.IsLeafType())
        //    {
        //        gathered.Add(new PreProcessingSelection(fieldSelection));
        //    }


        //    var childSelections = GatherChildSelections(context, fieldSelection);
        //    foreach (ISelection child in childSelections)
        //    {
        //        gathered.AddRange(GatherRecursiveSelections(context, child));
        //    }

        //    return gathered;
        //}

    }


}
