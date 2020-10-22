using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessedExtensions.Sorting
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
    public class PreProcessedSortedResults<TEntity> : List<TEntity>, IEnumerable<TEntity>, IAmPreProcessedResult
        where TEntity : class
    {
        public PreProcessedSortedResults(IEnumerable<TEntity> results)
        {
            if(results != null)
                this.AddRange(results);
        }

    }
}
