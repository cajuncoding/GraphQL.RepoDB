using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public class OffsetPageResults<TEntity> : IOffsetPageResults<TEntity> where TEntity : class
    {
        public OffsetPageResults(IEnumerable<TEntity> results, bool hasNextPage, bool hasPreviousPage, int totalCount)
        {
            this.Results = results;
            this.TotalCount = totalCount;
            this.HasNextPage = hasNextPage;
            this.HasPreviousPage = hasPreviousPage;
        }

        public IEnumerable<TEntity> Results { get; protected set; }

        public int? TotalCount { get; protected set; }

        public bool HasNextPage { get; protected set; }

        public bool HasPreviousPage { get; protected set; }


        public OffsetPageResults<TTargetType> OfType<TTargetType>() where TTargetType : class
        {
            var results = this.Results?.OfType<TTargetType>();
            return new OffsetPageResults<TTargetType>(results, this.HasNextPage, this.HasPreviousPage, (int)this.TotalCount);
        }

        public OffsetPageResults<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc) where TTargetType : class
        {
            var results = this.Results?.Select(r => mappingFunc(r));
            return new OffsetPageResults<TTargetType>(results, this.HasNextPage, this.HasPreviousPage, (int)this.TotalCount);
        }

        public PreProcessedOffsetPageResults<TEntity> AsPreProcessedPageResults()
        {
            return new PreProcessedOffsetPageResults<TEntity>(this);
        }

    }
}
