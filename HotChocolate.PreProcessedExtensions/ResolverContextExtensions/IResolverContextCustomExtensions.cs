# nullable enable

using HotChocolate.Data.Sorting;
using HotChocolate.Data.Sorting.Expressions;
using HotChocolate.Execution.Processing;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
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
        public static IReadOnlyList<IFieldSelection> GetSelectionsSafely(this IResolverContext context)
        {
            if (context == null) 
                return null!;

            List<IFieldSelection> selectionResults = new List<IFieldSelection>();

            var hotChocolateNamedType = context?.Field.Type.NamedType();
            //ObjectType? currentObjectType = null;

            //TryGetObjecType()...
            //if (hotChocolateNamedType is InterfaceType interfaceType)
            //    currentObjectType = interfaceType.ResolveConcreteType(context, "allCharacters");
            //else if (hotChocolateNamedType is ObjectType objectType)
            //    currentObjectType = objectType;

            if (hotChocolateNamedType is ObjectType currentObjectType)
            {
                var selections = context!.GetSelections(currentObjectType);

                //BBernard
                //Determine if the Selection is for a Connection, and dive deeper to get the real
                //  selections from the node {} field.
                var lookup = selections.ToLookup(s => s.Field.Name.ToString());

                //Handle paging cases; current Node is a Connection so we have to look for selections inside
                //  ->edges->nodes, or inside the ->nodes (shortcut per Relay spec); both of which may exist(?)
                if (lookup.Contains(SelectionNodeName.Nodes) || lookup.Contains(SelectionNodeName.Edges))
                {
                    //NOTE: nodes and edges are not mutually exclusive per Relay spec so
                    //          we gather from all if they are defined...
                    if (lookup.Contains(SelectionNodeName.Nodes))
                    {
                        var nodesSelectionField = lookup[SelectionNodeName.Nodes].FirstOrDefault() as Selection;
                        var childSelections = GatherChildSelections(context, nodesSelectionField);
                        selectionResults.AddRange(childSelections);
                    }

                    if (lookup.Contains(SelectionNodeName.Edges))
                    {
                        var edgesSelectionField = lookup[SelectionNodeName.Edges].FirstOrDefault() as Selection;
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

            return selectionResults as IReadOnlyList<IFieldSelection>;
        }

        /// <summary>
        /// Retrieve the current Primary node selection names as strings (e.g. SELECT fields from data) 
        /// from the current GraphQL query.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<string> GetSelectionNamesSafely(this IResolverContext context)
        {
            return context.GetSelectionsSafely()?.Select(f => f.Field.Name.ToString()).ToList();
        }

        /// <summary>
        /// Find the selection that matches the speified name.
        /// For more info. on Node parsing logic see here:
        /// https://github.com/ChilliCream/hotchocolate/blob/a1f2438b74b19e965b560ca464a9a4a896dab79a/src/Core/Core.Tests/Execution/ResolverContextTests.cs#L83-L89
        /// </summary>
        /// <param name="context"></param>
        /// <param name="selection"></param>
        /// <param name="selectionFieldName"></param>
        /// <returns></returns>
        private static ISelection FindChildSelectionByName(IResolverContext context, string selectionFieldName, ISelection? selection)
        {
            var field = selection?.Field ?? context.Field;
            if (field.Type.NamedType() is ObjectType objectType)
            {
                var childSelections = context.GetSelections(objectType, selection?.SelectionSet);
                return childSelections.FirstOrDefault(
                    s => s.Field.Name.ToString().Equals(selectionFieldName, StringComparison.OrdinalIgnoreCase)
                ) as ISelection;
            }

            return null;
        }

        /// <summary>
        /// Gather all child selections of the specified Selection
        /// For more info. on Node parsing logic see here:
        /// https://github.com/ChilliCream/hotchocolate/blob/a1f2438b74b19e965b560ca464a9a4a896dab79a/src/Core/Core.Tests/Execution/ResolverContextTests.cs#L83-L89
        /// </summary>
        /// <param name="context"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        private static List<ISelection> GatherChildSelections(IResolverContext context, ISelection? selection = null)
        {
            List<ISelection> gathered = new List<ISelection>();
            var field = selection?.Field ?? context.Field;
            if (field.Type.NamedType() is ObjectType objectType)
            {
                var childSelections = context.GetSelections(objectType, selection?.SelectionSet);
                gathered.AddRange(childSelections.OfType<ISelection>());
            }

            return gathered;
        }

        /// <summary>
        /// Gather/Collect ALL selectins from the current node down, recursively.
        /// For more info. on Node parsing logic see here:
        /// https://github.com/ChilliCream/hotchocolate/blob/a1f2438b74b19e965b560ca464a9a4a896dab79a/src/Core/Core.Tests/Execution/ResolverContextTests.cs#L83-L89
        /// </summary>
        /// <param name="context"></param>
        /// <param name="selection"></param>
        /// <param name="gatherRecursively"></param>
        /// <returns></returns>
        private static List<ISelection> GatherRecursiveSelections(IResolverContext context, ISelection? selection = null)
        {
            List<ISelection> gathered = new List<ISelection>();
            var field = selection?.Field ?? context.Field;

            if (selection != null && field.Type.IsLeafType())
            {
                gathered.Add(selection);
            }

            if (field.Type.NamedType() is ObjectType objectType)
            {
                var childSelections = context.GetSelections(objectType, selection?.SelectionSet);
                foreach (ISelection child in childSelections)
                {
                    gathered.AddRange(GatherRecursiveSelections(context, child));
                }

            }

            return gathered;
        }
    }
}
