using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessedExtensions.Sorting
{
    public interface IPreProcessedSortedResults<TEntity> : IEnumerable<TEntity>, IAmPreProcessedResult
    {
    }
}
