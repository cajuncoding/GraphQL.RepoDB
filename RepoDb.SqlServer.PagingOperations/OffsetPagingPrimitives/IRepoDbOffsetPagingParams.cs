namespace RepoDb.OffsetPaging
{
    public interface IRepoDbOffsetPagingParams
    {
        int? Skip { get; }
        int? Take { get; }
        bool IsTotalCountRequested { get; }
    }
}