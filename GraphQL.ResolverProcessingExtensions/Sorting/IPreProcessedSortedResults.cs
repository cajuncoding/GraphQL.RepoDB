using System;
using System.Collections.Generic;

namespace HotChocolate.ResolverProcessingExtensions.Sorting
{
    [Obsolete("It is now Recommended to use the SetSortingIsHandled() convenience method or access the SortArgs (which will then cause HC to set Sorting as already handled),"
              + " and then return your IEnumerable, List, etc. directly from the resolver; this will simplify your code and offer some performance improvement. This will be removed in a future release.")]
    public interface IResolverProcessedSortedResults<out TEntity> : IEnumerable<TEntity>, IAmResolverProcessedResult
    {
    }
}
