# nullable enable

using HotChocolate.Data.Sorting;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.PreProcessingExtensions.Sorting
{
    public static class IResolverContextSortingExtensions
    {
        /// <summary>
        /// Safely process the GraphQL context to retrieve the Order argument;
        /// matches the default name used by HotChocolate Sorting middleware (order: {{field1}: ASC, {field2}: DESC).
        /// Will return null if the order arguments/info is not available.
        /// </summary>
        /// <returns></returns>
        public static List<ISortOrderField>? GetSortingArgsSafely(this IResolverContext context, string sortOrderArgName = null!)
        {
            var results = new List<ISortOrderField>();

            //Unfortunately the Try/Catch is required to make this safe for easier coding when the argument is not specified,
            //  because the ResolverContext doesn't expose a method to check if an argument exists...
            try
            {
                var sortArgName = sortOrderArgName ?? SortConventionDefinition.DefaultArgumentName;

                //Get Sort Argument Fields and current Values...
                //NOTE: In order to correctly be able to Map names from GraphQL Schema to property/member names
                //      we need to get both the Fields (Schema) and the current order values...
                //NOTE: Not all Queries have Fields (e.g. no Selections, just a literal result), so .Field may
                //      throw internal NullReferenceException, hence we have the wrapper Try/Catch.
                var sortContext = context.GetSortingContext();
                //if (sortContext != null)
                //{
                //    //var sortFieldLookup = 
                //    var sortOrderFields = sortContext.GetFields().SelectMany(
                //        fi => new SordOrderField(fi, fi.)
                //    )
                //}


                IInputField sortArgField = context.Selection.Field.Arguments[sortArgName];
                ObjectValueNode sortArgValue = context.ArgumentLiteral<ObjectValueNode>(sortArgName);

                //Validate that we have some sort args specified and that the Type is correct (ListType of SortInputType values)...
                //NOTE: The Following processing logic was adapted from 'QueryableSortProvider' implementation in HotChocolate.Data core.
                //FIX: The types changed in v11.0.1/v11.0.2 the Sort Field types need to be checked with IsNull() method, and
                //      then against NonNullType.NamedType() is ISortInputType instead.
                if (sortContext != null
                    && !sortArgValue.IsNull()
                    && sortArgField.Type is ListType lt
                    && lt.ElementType is NonNullType nn
                    && nn.NamedType() is ISortInputType sortInputType)
                {
                    //Create a Lookup for the Fields...
                    //var sortFieldLookup = sortInputType.Fields.OfType<SortField>().ToLookup(f => f.Name.ToString().ToLower());
                    var sortFieldLookup = sortContext.GetFields().SelectMany(fi => fi).ToLookup(f => f.Field.Name, StringComparer.OrdinalIgnoreCase);

                    //Now only process the values provided, but initialize with the corresponding Field (metadata) for each value...
                    var sortOrderFields = sortArgValue.Fields.Select(
                        f => new SortOrderField(
                            sortFieldLookup[f.Name.ToString()].FirstOrDefault(),
                            f.Value.ToString()
                        )
                    );

                    results.AddRange(sortOrderFields);
                }

                return results;
            }
            catch
            {
                //Always safely return at least an Empty List to help minimize Null Reference issues.
                return results;
            }
        }
    }
}
