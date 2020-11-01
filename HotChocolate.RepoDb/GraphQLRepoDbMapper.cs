using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.PreProcessingExtensions.Sorting;
using HotChocolate.Types;
using RepoDb;
using RepoDb.CursorPagination;
using RepoDb.CustomExtensions;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.RepoDb
{
    /// <summary>
    /// Helper class to map normal, primitive, and valuees from HotChocolate GraphQL integration to RepoDb
    /// models for processing with RepoDb.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class GraphQLRepoDbMapper<TModel> where TModel : class
    {
        public IParamsContext GraphQLParamsContext { get; protected set; }

        public GraphQLRepoDbMapper(IParamsContext graphQLParams)
        {
            this.GraphQLParamsContext = graphQLParams;
        }

        /// <summary>
        /// Map the Selection name values for a specific GrqphQL type from the HotChocolate GraphQL Schema (names of the Schema) 
        /// to RepoDb specific values that have the underlying DB field name (as potentially mapped on the Model).
        /// All Fields are returned as a default if the value is undefined and/or invalid and cannot be mapped.
        /// NOTE: Property names and db fields names are not guaranteed to be the same.
        /// </summary>
        /// <param name="fieldNamesFilter"></param>
        /// <returns>
        /// List of Database fields mapped from all of the available GraphQL Selections mapped to the generics
        /// model type TEntity specified. As a fallback default, all DB Fields are returned if no Selections are available from the 
        /// GraphQL ParamsContext.
        /// </returns>
        public IEnumerable<Field> GetSelectFieldsFor<TGraphQLEntity>()
        {
            //Get the Selection Member names -- the actual class property/member names mapped by GraphQL;
            //  this may be different than the GraphQL Schema names due to GraphQL name mapping.
            IEnumerable<string> selectionNamesFilter = this.GraphQLParamsContext?.GetSelectionMappedNamesFor<TGraphQLEntity>(SelectionNameFlags.All);

            return GetSelectFields(selectionNamesFilter);
        }

        /// <summary>
        /// Map the Selection name values from all available HotChocolate GraphQL Query selections (names of the Schema) to 
        /// RepoDb specific values that have the underlying DB field name (as potentially mapped on the Model).
        /// All Fields are returned as a default if the value is undefined and/or invalid and cannot be mapped.
        /// NOTE: Property names and db fields names are not guaranteed to be the same.
        /// </summary>
        /// <param name="fieldNamesFilter"></param>
        /// <returns>
        /// List of Database fields mapped from all of the available GraphQL Selections mapped to the generics
        /// model type TEntity specified. As a fallback default, all DB Fields are returned if no Selections are available from the 
        /// GraphQL ParamsContext.
        /// </returns>
        public IEnumerable<Field> GetSelectFields()
        {
            //Get the Selection Member names -- the actual class property/member names mapped by GraphQL;
            //  this may be different than the GraphQL Schema names due to GraphQL name mapping.
            IEnumerable<string> selectionNamesFilter = this.GraphQLParamsContext?.GetSelectionMappedNames(SelectionNameFlags.All);

            return GetSelectFields(selectionNamesFilter);
        }

        /// <summary>
        /// Map the Selection name values from the specified Selection Names provided to the   
        /// RepoDb specific values that have the underlying DB field name (as potentially mapped on the Model).
        /// All Fields are returned as a default if the value is undefined and/or invalid and cannot be mapped.
        /// NOTE: Property names and db fields names are not guaranteed to be the same.
        /// </summary>
        /// <param name="fieldNamesFilter"></param>
        /// <returns>
        /// List of Database fields mapped from all of the available GraphQL Selections mapped to the generics
        /// model type TEntity specified. As a fallback default, all DB Fields are returned if no Selections are available from the 
        /// GraphQL ParamsContext.
        /// </returns>
        public IEnumerable<Field> GetSelectFields(IEnumerable<string> selectionNamesFilter)
        {
            // Ensure we are null safe and Get all the fields in that case...
            if (selectionNamesFilter == null)
            {
                //NOTE: Since there's no need to filter we can just get ALL fields from the FieldCache!
                return FieldCache.Get<TModel>();
            }
            else
            {
                //NOTE: For GraphQL we need to lookup the actual Db field by the Model's Property Name
                //  and then convert to the actual DB field name; which might also be mapped name via RepoDb attribute. 
                //  For more info see: https://repodb.net/cacher/propertymappednamecache
                //TODO: Add Caching Layer here if needed to Cached a Reverse Dictionary of mappings by Model Name!
                var mappingLookup = PropertyCache.Get<TModel>().ToLookup(p => p.PropertyInfo.Name.ToLower());

                var selectFields = selectionNamesFilter
                    .Select(name => mappingLookup[name.ToLower()]?.FirstOrDefault()?.AsField())
                    .Where(prop => prop != null);

                return selectFields;
            }
        }

        /// <summary>
        /// Map the SortOrderField values from HotChocolate Custom Extensions to RepoDb specific
        /// OrderField(s) values that have the underlying DB field name (as potentially mapped on the Model).
        /// Null is returned if the value is undefined and/or invalid and cannot be mapped.
        /// NOTE: Property names and db fields names are not guaranteed to be the same.
        /// </summary>
        /// <param name="graphQLSortFields"></param>
        /// <returns></returns>
        public IEnumerable<OrderField> GetSortOrderFields()
        {
            var graphQLSortFields = this.GraphQLParamsContext?.SortArgs;

            //Ensure we are null safe and return null if no fields are specified...
            if (graphQLSortFields?.Any() != true)
                return null;

            //NOTE: the RepDb PropertyCache provides mapping lookups, but only by mapped name (e.g. Database name)
            //  for GraphQL (Pure Code First) we need to lookup the field by the Model's Property Name
            //  and then convert to the mapped name. So we create a Lookup by Model Property Name!
            //  For more info see: https://repodb.net/cacher/propertymappednamecache
            //TODO: Add Caching Layer here if needed to Cached a Reverse Dictionary of mappings by Model Name!
            var mappingLookup = PropertyCache.Get<TModel>().ToLookup(p => p.PropertyInfo.Name.ToLower());

            var orderByFields = graphQLSortFields
                .Select(sf => new {
                    //Null safe checking for the mapped field from RepoDb...
                    //NOTE: We map based on the actual class property/member name not the fieldname which is
                    //      from the GraphQL schema and may be different than the underlying class property/member.
                    RepoDbField = mappingLookup[sf.MemberName.ToLower()]?.FirstOrDefault()?.AsField(),
                    //We test for Descencing so that Ascending is always the default for a mismatch.
                    RepoDbOrder = sf.IsDescending() ? Order.Descending : Order.Ascending
                })
                //Filter out if the RepoDbField is null; meaning it's invalid and/or doesn't exist.
                .Where(f => f.RepoDbField != null)
                .Select(f => new OrderField(f.RepoDbField.Name, f.RepoDbOrder));

            return orderByFields;
        }

        /// <summary>
        /// Map the HotChocolate CursorPagingArguments into the RepoDb specific Cursor paging parameter.
        /// Null is returned if the value is undefined and/or invalid and cannot be mapped.
        /// The default paging method is Cursor based paging which matches HotChocolate UsePaging default;
        ///     use GetOffsetPagingParameters() otherwise.
        /// </summary>
        /// <returns></returns>

        public IRepoDbCursorPagingParams GetPagingParameters() => GetCursorPagingParameters();

        /// <summary>
        /// Map the HotChocolate CursorPagingArguments into the RepoDb specific Cursor paging parameter.
        /// Null is returned if the value is undefined and/or invalid and cannot be mapped.
        /// </summary>
        /// <returns></returns>
        public IRepoDbCursorPagingParams GetCursorPagingParameters()
        {
            var graphQLPagingArgs = this.GraphQLParamsContext?.CursorPagingArgs;

            if (!graphQLPagingArgs.HasValue || !graphQLPagingArgs.Value.IsPagingArgumentsValid() == true)
            {
                return null;
            }

            return new RepoDbCursorPagingParams(
                first: graphQLPagingArgs.Value.First,
                after: graphQLPagingArgs.Value.After,
                before: graphQLPagingArgs.Value.Before,
                last: graphQLPagingArgs.Value.Last
            );
        }

        /// <summary>
        /// Map the HotChocolate CursorPagingArguments into the RepoDb specific Cursor paging parameter.
        /// Null is returned if the value is undefined and/or invalid and cannot be mapped.
        /// </summary>
        /// <returns></returns>
        public RepoDbOffsetPagingParams GetOffsetPagingParameters()
        {
            var graphQLPagingArgs = this.GraphQLParamsContext?.OffsetPagingArgs;

            if (!graphQLPagingArgs.HasValue || !graphQLPagingArgs.Value.IsPagingArgumentsValid() == true)
            {
                return null;
            }

            var pagingArgs = graphQLPagingArgs.Value;
            return RepoDbOffsetPagingParams.FromSkipTake(pagingArgs.Take, pagingArgs.Skip ?? 0);
        }
    }
}
