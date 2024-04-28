using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using StarWars.Repositories;

namespace StarWars.Reviews
{
    [ExtendObjectType("Mutation")]
    public class ReviewMutations
    {
        /// <summary>
        /// Creates a review for a given Star Wars episode.
        /// </summary>
        public Task<CreateReviewPayload> CreateReview(
            CreateReviewInput input,
            [Service]IReviewRepository repository//,
            //[Service]IEventSender eventSender
        )
        {
            var review = new Review(input.Stars, input.Commentary);
            repository.AddReview(input.Episode, review);
            //NOTE: REMOVED as Subscriptions have unknown support in Serverless Architecture (Azure Functions).
            //await eventSender.SendAsync(new OnReviewMessage(input.Episode, review));
            return Task.FromResult(new CreateReviewPayload(input.Episode, review));
        }
    }
}
