using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HotChocolate.PreProcessingExtensions.Sorting
{
    /// <summary>
    /// Payload response container for HotChocolate pipeline that contains all deatils needed for pre-processed
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
    public class PreProcessedSortedResults<TEntity> : List<TEntity>, IPreProcessedSortedResults<TEntity>, IAmPreProcessedResult
    {
        public PreProcessedSortedResults(IEnumerable<TEntity> results)
        {
            if(results != null)
                this.AddRange(results);
        }
    }

    public static class PreprocessedSortedResultsExtensions
    {
        /// <summary>
        /// Convenience method to Wrap the current Enumerable Result Items as a PreProcessedSortResults; to eliminate
        /// ceremonial code for new-ing up the results.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="enumerableItems"></param>
        /// <returns></returns>
        [Obsolete("It is now Recommended to use the SetSortingIsHandled() convenience method or access the SortArgs (which will then cause HC to set Sorting as already handled),"
                  + " and then return your IEnumerable, List, etc. directly from the resolver; this will simplify your code and offer some performance improvement. This will be removed in a future release.")]
        public static PreProcessedSortedResults<TEntity> AsPreProcessedSortResults<TEntity>(this IEnumerable<TEntity> enumerableItems)
        {
            if (enumerableItems == null)
                return null;

            return new PreProcessedSortedResults<TEntity>(enumerableItems);
        }
    }
}
