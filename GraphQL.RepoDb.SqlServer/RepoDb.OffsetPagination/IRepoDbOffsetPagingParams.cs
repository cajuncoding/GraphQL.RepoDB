namespace RepoDb.OffsetPagination
{
    public interface IRepoDbOffsetPagingParams
    {
        int? Skip { get; }
        int? Take { get; }
    }
}