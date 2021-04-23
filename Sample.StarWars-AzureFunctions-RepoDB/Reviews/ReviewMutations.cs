using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using StarWars.Repositories;

namespace StarWars.Reviews
{
    [ExtendObjectType(Name = "Mutation")]
    public class ReviewMutations
    {
        /// <summary>
        /// Creates a review for a given Star Wars episode.
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<CreateReviewPayload> CreateReview(
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            CreateReviewInput input,
            [Service]IReviewRepository repository//,
            //[Service]IEventSender eventSender
        )
        {
            var review = new Review(input.Stars, input.Commentary);
            repository.AddReview(input.Episode, review);
            //NOTE: REMOVED as Subscriptions have unknown support in Serverless Architecture (AzureFunctions).
            //await eventSender.SendAsync(new OnReviewMessage(input.Episode, review));
            return new CreateReviewPayload(input.Episode, review);
        }
    }
}
