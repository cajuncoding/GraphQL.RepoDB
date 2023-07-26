# nullable enable

using System;
using HotChocolate.Data.Sorting;
using HotChocolate.Resolvers;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.ResolverProcessingExtensions.Sorting
{
    public static class IResolverContextSortingExtensions
    {
        /// <summary>
        /// Safely process the GraphQL context to retrieve the Order argument;
        /// matches the default name used by HotChocolate Sorting middleware (order: {{field1}: ASC, {field2}: DESC).
        /// Will return Empty List if the order arguments/info is not available.
        ///NOTE: HC will set Sorting Handled() to true as Default behaviour immediately when the GetSortingContext() is called;
        /// therefore this holds true also for when this is called.
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
                ?? Enumerable.Empty<ISortOrderField>();

            return sortOrderFields.ToList();
        }
    }
}
