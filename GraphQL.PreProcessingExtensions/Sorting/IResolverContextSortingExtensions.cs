# nullable enable

using HotChocolate.Data.Sorting;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HotChocolate.PreProcessingExtensions.Sorting
{
    public static class IResolverContextSortingExtensions
    {
        /// <summary>
        /// Safely process the GraphQL context to retrieve the Order argument;
        /// matches the default name used by HotChocolate Sorting middleware (order: {{field1}: ASC, {field2}: DESC).
        /// Will return Empty List if the order arguments/info is not available.
        /// </summary>
        /// <returns></returns>
        public static List<ISortOrderField>? GetSortingArgsSafely(this IResolverContext context, string sortOrderArgName = null!)
        {
            var sortContext = context.GetSortingContext();
            var sortOrderFields = sortContext?
                .GetFields()
                .SelectMany(sf => sf)
                .Where(sf => sf?.Value?.ValueNode?.Value != null)
                .Select(sf => new SortOrderField(sf, sf.Value!.ValueNode.Value!.ToString()))
                .Cast<ISortOrderField>()
                .ToList();

            return sortOrderFields;
        }
    }
}
