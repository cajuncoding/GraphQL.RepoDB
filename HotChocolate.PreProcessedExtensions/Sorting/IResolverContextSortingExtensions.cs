# nullable enable

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

namespace HotChocolate.PreProcessedExtensions.Sorting
{
    public static class IResolverContextSortingExtensions
    {
        /// <summary>
        /// Safely process the GraphQL context to retrieve the Order argument;
        /// matches the default name used by HotChocolate Sorting middleware (order: {{field1}: ASC, {field2}: DESC).
        /// Will return null if the order arguments/info is not available.
        /// </summary>
        /// <returns></returns>
        public static List<ISortOrderField>? GetSortingArgsSafely(this IResolverContext context, string orderByArgName = "order")
        {
            //Unfortunately the Try/Catch is required to make this safe for easier coding when the argument is not specified,
            //  because the ResolverContext doesn't expose a method to check if an argument exists...
            try
            {
                //Get Sort Argument...
                //NOTE: To get the data literally from the Query, we must use ArgumentLiteral with type ObjectValueNode,
                //  which will result in an object model that easy to consume and preserves order of arguments; which
                //  is critical for Sorting!
                var orderNodeInfo = context.ArgumentLiteral<ObjectValueNode>(orderByArgName);

                List<ISortOrderField> orderFields = orderNodeInfo.Fields.Select(
                    f => (ISortOrderField)new SortOrderField(f.Name.ToString(), f.Value.ToString())
                ).ToList();

                return orderFields;
            }
            catch
            {
                return null;
            }
        }
    }
}
