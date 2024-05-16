using System.Collections.Generic;
using HotChocolate.ResolverProcessingExtensions;
using RepoDb;
using RepoDb.PagingPrimitives.CursorPaging;
using RepoDb.PagingPrimitives.OffsetPaging;

namespace HotChocolate.RepoDb;

public interface IGraphQLRepoDbMapper
{
    IParamsContext GraphQLParamsContext { get; }

    /// <summary>
    /// Map the Selection name values for a specific GraphQL type from the HotChocolate GraphQL Schema (names of the Schema) 
    /// to RepoDb specific values that have the underlying DB field name (as potentially mapped on the Model).
    /// All Fields are returned as a default if the value is undefined and/or invalid and cannot be mapped.
    /// NOTE: Property names and db fields names are not guaranteed to be the same.
    /// </summary>
    /// <returns>
    /// List of Database fields mapped from all of the available GraphQL Selections mapped to the generics
    /// model type TEntity specified. As a fallback default, all DB Fields are returned if no Selections are available from the 
    /// GraphQL ParamsContext.
    /// </returns>
    IEnumerable<Field> GetSelectFieldsFor<TGraphQLEntity>();

    /// <summary>
    /// Map the Selection name values from all available HotChocolate GraphQL Query selections (names of the Schema) to 
    /// RepoDb specific values that have the underlying DB field name (as potentially mapped on the Model).
    /// All Fields are returned as a default if the value is undefined and/or invalid and cannot be mapped.
    /// NOTE: Property names and db fields names are not guaranteed to be the same.
    /// </summary>
    /// <returns>
    /// List of Database fields mapped from all the available GraphQL Selections mapped to the generics
    /// model type TEntity specified. As a fallback default, all DB Fields are returned if no Selections are available from the 
    /// GraphQL ParamsContext.
    /// </returns>
    IEnumerable<Field> GetSelectFields();

    /// <summary>
    /// Map the Selection name values from the specified Selection Names provided to the   
    /// RepoDb specific values that have the underlying DB field name (as potentially mapped on the Model).
    /// All Fields are returned as a default if the value is undefined and/or invalid and cannot be mapped.
    /// NOTE: Property names and db fields names are not guaranteed to be the same.
    /// </summary>
    /// <param name="selectionNamesFilter"></param>
    /// <returns>
    /// List of Database fields mapped from all the available GraphQL Selections mapped to the generics
    /// model type TEntity specified. As a fallback default, all DB Fields are returned if no Selections are available from the 
    /// GraphQL ParamsContext.
    /// </returns>
    IEnumerable<Field> GetSelectFields(IEnumerable<string> selectionNamesFilter);

    /// <summary>
    /// Map the SortOrderField values from HotChocolate Custom Extensions to RepoDb specific
    /// OrderField(s) values that have the underlying DB field name (as potentially mapped on the Model).
    /// Null is returned if the value is undefined and/or invalid and cannot be mapped.
    /// NOTE: Property names and db fields names are not guaranteed to be the same.
    /// </summary>
    /// <returns></returns>
    IEnumerable<OrderField> GetSortOrderFields();

    /// <summary>
    /// Map the HotChocolate CursorPagingArguments into the RepoDb specific Cursor paging parameter.
    /// Null is returned if the value is undefined and/or invalid and cannot be mapped.
    /// The naming convention matches the correct usage along with the [UsePaging] HotChocolate attribute; 
    /// whereby the default paging method is Cursor based paging. 
    /// Otherwise, use OffsetPagingArgs with the [UseOffsetPaging] attribute.
    /// </summary>
    /// <returns></returns>
    IRepoDbCursorPagingParams GetPagingParameters();

    /// <summary>
    /// Map the HotChocolate CursorPagingArguments into the RepoDb specific Cursor paging parameter.
    /// Null is returned if the value is undefined and/or invalid and cannot be mapped. 
    /// This will return the same results a GetPagingParameters() matching the same Cursor based
    /// paging default as HotChocolate does with the [UsePaging] attribute.
    /// </summary>
    /// <returns></returns>
    IRepoDbCursorPagingParams GetCursorPagingParameters();

    /// <summary>
    /// Map the HotChocolate OffsetPagingArguments into the RepoDb specific offset paging parameter.
    /// Null is returned if the value is undefined and/or invalid and cannot be mapped.
    /// The naming convention matches the correct usage along with the [UseOffsetPaging] HotChocolate attribute.
    /// </summary>
    /// <returns></returns>
    RepoDbOffsetPagingParams GetOffsetPagingParameters();
}