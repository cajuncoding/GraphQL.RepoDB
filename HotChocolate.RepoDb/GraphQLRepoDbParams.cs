using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Pagination;
using RepoDb;
using RepoDb.CursorPagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HotChocolate.RepoDb
{
    public class GraphQLRepoDbParams<TEntity> where TEntity: class
    {
        protected List<Field> _selectFields;
        protected List<OrderField> _sortOrderFields;
        protected IRepoDbCursorPagingParams _pagingParameters;

        public GraphQLRepoDbParams(IParamsContext graphQLParams)
        {
            this.RepoDbMapper = new GraphQLRepoDbMapper<TEntity>(graphQLParams);
        }

        public IParamsContext GraphQLParams => RepoDbMapper?.GraphQLParamsContext;

        public GraphQLRepoDbMapper<TEntity> RepoDbMapper { get; protected set; }

        public IReadOnlyList<Field> SelectFields => _selectFields 
            ??= RepoDbMapper.GetSelectFields()?.ToList();

        public IReadOnlyList<OrderField> SortOrderFields => _sortOrderFields
            ??= RepoDbMapper.GetSortOrderFields()?.ToList();

        public IRepoDbCursorPagingParams PagingParameters => _pagingParameters 
            ??= RepoDbMapper?.GetPagingParameters();

    }
}
