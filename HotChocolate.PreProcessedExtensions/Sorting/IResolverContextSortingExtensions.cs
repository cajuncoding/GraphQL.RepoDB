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
        public static List<ISortOrderField>? GetSortingArgsSafely(this IResolverContext context, string orderByArgName = null)
        {
            //Unfortunately the Try/Catch is required to make this safe for easier coding when the argument is not specified,
            //  because the ResolverContext doesn't expose a method to check if an argument exists...
            try
            {
                var results = new List<ISortOrderField>();
                var orderArgName = orderByArgName ?? SortConventionDefinition.DefaultArgumentName;

                //Get Sort Argument Fields and current Values...
                //NOTE: In order to correctly be able to Map names from GraphQL Schema to property/member names
                //      we need to get both the Fields (Schema) and the current order values...
                IInputField sortArgField = context.Field.Arguments[orderArgName];
                ObjectValueNode sortArgValue = context.ArgumentLiteral<ObjectValueNode>(orderArgName);

                //Validate that we have some sort args specified and that the Type is correct (ListType of SortInputType values)...
                if (sortArgValue != null && sortArgField.Type is ListType lt
                    && lt.ElementType is SortInputType sortInput)
                {
                    //Create a Lookup for the Fields...
                    var sortFieldLookup = sortInput.Fields.OfType<SortField>().ToLookup(f => f.Name.ToString().ToLower());

                    //Now only process the values provided, but initialize with the corresponding Field (metadata) for each value...
                    var sortOrderFields = sortArgValue.Fields.Select(
                        f => new SortOrderField(sortFieldLookup[f.Name.ToString().ToLower()].FirstOrDefault(), f.Value.ToString())
                    );
                    
                    results.AddRange(sortOrderFields);
                }

                return results;
            }
            catch
            {
                return null;
            }
        }
    }
}
