using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessingExtensions.Sorting
{
    public interface IPreProcessedSortedResults<TEntity> : IEnumerable<TEntity>, IAmPreProcessedResult
    {
    }
}
