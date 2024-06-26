﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.PagingPrimitives.OffsetPaging
{
    /// <summary>
    /// Model class for representing a paging result set that was computed using Cursor Pagination process by offering
    /// a default implementation of the IOffsetPageResults interface.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class OffsetPageResults<TEntity> : IOffsetPageResults<TEntity>
    {
        public OffsetPageResults(IEnumerable<TEntity> results, bool hasNextPage, bool hasPreviousPage, int startIndex, int endIndex, int? totalCount)
        {
            this.Results = results?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(results));
            this.PageCount = Results.Count;
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
            this.HasNextPage = hasNextPage;
            this.HasPreviousPage = hasPreviousPage;
            this.TotalCount = totalCount;
        }

        public IReadOnlyList<TEntity> Results { get; protected set; }

        public int StartIndex { get; }

        public int EndIndex { get; }

        public int? TotalCount { get; protected set; }

        public int PageCount { get; }

        public bool HasNextPage { get; protected set; }

        public bool HasPreviousPage { get; protected set; }


        public virtual OffsetPageResults<TTargetType> OfType<TTargetType>() 
        {
            var results = this.Results?.OfType<TTargetType>();
            return new OffsetPageResults<TTargetType>(results, this.HasNextPage, this.HasPreviousPage, this.StartIndex, this.EndIndex, this.TotalCount);
        }

        public virtual OffsetPageResults<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc)
        {
            if (mappingFunc == null)
                throw new ArgumentException(nameof(mappingFunc));

            var results = this.Results?.Select(mappingFunc);
            return new OffsetPageResults<TTargetType>(results, this.HasNextPage, this.HasPreviousPage, this.StartIndex, this.EndIndex, this.TotalCount);
        }
    }
}
