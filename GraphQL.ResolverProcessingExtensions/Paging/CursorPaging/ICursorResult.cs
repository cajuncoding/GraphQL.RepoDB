using System;

namespace HotChocolate.ResolverProcessingExtensions.Pagination
{
    /// <summary>
    /// Interface representing a Decorator class for any TEntity model provided along 
    /// with an associated Cursor Index value.
    /// 
    /// As a general solution, covering all use-cases the Cursor is an Index representing the location
    /// of the entity in a sorted result set (e.g. ROW_NUMBER() from ordered results from Sql Server)
    /// 
    /// This class represents a single result/node of an edge/slice/page result set.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ICursorResult<out TEntity>
    {
        //As a general cover-all solution, the Cursor should be an Index value (e.g. Sql ROW_NUMBER()) 
        // so that it can be efficiently queried with Between X & Y or greater than X and less than Y syntax!
        public int CursorIndex { get; }
        public TEntity Entity { get; }
    }
}
