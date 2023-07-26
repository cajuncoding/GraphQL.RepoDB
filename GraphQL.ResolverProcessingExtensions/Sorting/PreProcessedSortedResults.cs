using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.ResolverProcessingExtensions.Sorting
{
    /// <summary>
    /// Payload response container for HotChocolate pipeline that contains all deatils needed for resolver processed
    /// results to integrate with the existing OOTB paging pipeline, but with results that are already completely
    /// processed by the Resolver (or lower layer).
    /// 
    /// We must inherit from List<TEntity> to ensure that HotChocolate can correctly Infer the proper Schema from 
    /// the base TEntity generic type; simply providing IEnumerable<TEntity> isn't enough.  
    /// As a real List<> the PureCode Schema inference works as expected!
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [Obsolete("It is now Recommended to use the SetSortingIsHandled() convenience method or access the SortArgs (which will then cause HC to set Sorting as already handled),"
              + " and then return your IEnumerable, List, etc. directly from the resolver; this will simplify your code and offer some performance improvement. This will be removed in a future release.")]
    public class ResolverProcessedSortedResults<TEntity> : IList<TEntity>, IResolverProcessedSortedResults<TEntity>, IAmResolverProcessedResult
    {
        private readonly List<TEntity> _innerList;

        public ResolverProcessedSortedResults(IEnumerable<TEntity> results)
        {
            _innerList = results?.ToList() ?? throw new ArgumentNullException(nameof(results));
        }


        #region IList<T> implementation

        public TEntity this[int index]
        {
            get => _innerList[index];
            set => _innerList[index] = value;
        }

        public int Count => _innerList.Count;

        public bool IsReadOnly => ((ICollection<TEntity>)_innerList).IsReadOnly;

        public void Add(TEntity item) => _innerList.Add(item);

        public void Clear() => _innerList.Clear();

        public bool Contains(TEntity item) => _innerList.Contains(item);

        public void CopyTo(TEntity[] array, int arrayIndex) => _innerList.CopyTo(array, arrayIndex);

        public IEnumerator<TEntity> GetEnumerator() => _innerList.GetEnumerator();

        public int IndexOf(TEntity item) => _innerList.IndexOf(item);

        public void Insert(int index, TEntity item) => _innerList.Insert(index, item);

        public bool Remove(TEntity item) => _innerList.Remove(item);

        public void RemoveAt(int index) => _innerList.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        #endregion
    }

    public static class ResolverProcessedSortedResultsExtensions
    {
        /// <summary>
        /// Convenience method to Wrap the current Enumerable Result Items as a ResolverProcessedSortResults; to eliminate
        /// ceremonial code for new-ing up the results.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="enumerableItems"></param>
        /// <returns></returns>
        [Obsolete("It is now Recommended to use the SetSortingIsHandled() convenience method or access the SortArgs (which will then cause HC to set Sorting as already handled),"
                  + " and then return your IEnumerable, List, etc. directly from the resolver; this will simplify your code and offer some performance improvement. This will be removed in a future release.")]
        public static ResolverProcessedSortedResults<TEntity> ToResolverProcessedSortResults<TEntity>(this IEnumerable<TEntity> enumerableItems)
        {
            if (enumerableItems == null)
                return null;

            return new ResolverProcessedSortedResults<TEntity>(enumerableItems);
        }
    }
}
