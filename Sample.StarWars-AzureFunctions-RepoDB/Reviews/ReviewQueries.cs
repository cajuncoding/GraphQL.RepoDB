using System.Linq;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.RepoDb.InMemoryPaging;
using HotChocolate.ResolverProcessingExtensions;
using HotChocolate.ResolverProcessingExtensions.Sorting;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using StarWars.Characters;
using StarWars.Repositories;

namespace StarWars.Reviews
{
    [ExtendObjectType("Query")]
    public class ReviewQueries
    {
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public Connection<Review> GetReviews(
            Episode episode,
            [Service]IReviewRepository repository,
            //THIS is now injected by the Resolver Processing Extensions middleware...
            [GraphQLParams] IParamsContext graphqlParams
        )
        {
            //********************************************************************************
            //Get the data and convert to List() to ensure it's an Enumerable
            //  and no longer using IQueryable to successfully simulate 
            //  pre-processed results.
            var reviews = repository.GetReviews(episode).ToList();

            //Perform some pre-processed Sorting & Then Paging!
            //This could be done in a lower repository or pushed to the Database!
            var sortedReviews = reviews.SortDynamically(graphqlParams.SortArgs);
            var slicedReviews = sortedReviews.SliceAsCursorPage(graphqlParams.PagingArgs);

            //With a valid Page/Slice we can return a ResolverProcessed Cursor Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering will be applied but ONLY to the results we are now returning;
            //       Because this would normally be pushed down to the Sql Database layer.
            return slicedReviews.ToGraphQLConnection();
            //********************************************************************************
        }
    }
}
