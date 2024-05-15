namespace RepoDb.CursorPaging
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
        int CursorIndex { get; }
        string Cursor { get; }
        TEntity Entity { get; }
    }
}
