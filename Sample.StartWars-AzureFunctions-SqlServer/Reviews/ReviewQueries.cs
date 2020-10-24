using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.PreProcessedExtensions;
using HotChocolate.PreProcessedExtensions.Pagination;
using HotChocolate.PreProcessedExtensions.Sorting;
using HotChocolate.Types;
using StarWars.Characters;
using StarWars.Repositories;

namespace StarWars.Reviews
{
    [ExtendObjectType(Name = "Query")]
    public class ReviewQueries
    {
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IEnumerable<Review> GetReviews(
            Episode episode,
            [Service]IReviewRepository repository,
            //THIS is now injected by Pre-Processed extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            //********************************************************************************
            //Get the data and convert to List() to ensure it's an Enumerable
            //  and no longer using IQueryable to successfully simulate 
            //  pre-processed results.
            var reviews = repository.GetReviews(episode).ToList();

            //Perform some pre-processed Sorting & Then Paging!
            //This could be done in a lower repository or pushed to the Database!
            var sortedReviews = reviews.SortDynamically(graphQLParams.SortArgs);
            var slicedreviews = sortedReviews.SliceAsCursorPage(graphQLParams.PagingArgs);

            //With a valid Page/Slice we can return a PreProcessed Cursor Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering will be applied but ONLY to the results we are now returning;
            //       Because this would normally be pushed down to the Sql Database layer.
            return new PreProcessedCursorSliceResults<Review>(slicedreviews);
            //********************************************************************************

        }

    }
}
