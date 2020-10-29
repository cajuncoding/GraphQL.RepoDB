using System.Reflection;
using HotChocolate.PreProcessedExtensions;
using HotChocolate.PreProcessedExtensions.Pagination;
using HotChocolate.RepoDb;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using StarWars.Characters.DbModels;
using StarWars.Repositories;

namespace StarWars.Characters
{
    public sealed class GetFriendsResolverAttribute
        : ObjectFieldDescriptorAttribute
    {
        public override void OnConfigure(
            IDescriptorContext context,
            IObjectFieldDescriptor descriptor,
            MemberInfo member)
        {
            descriptor.Resolve(async ctx =>
            {
                ICharacter character = ctx.Parent<ICharacter>();
                ICharacterRepository repository = ctx.Service<ICharacterRepository>();
                //This is injected by the PreProcessing middleware wen enabled...
                var graphQLParams = new GraphQLParamsContext(ctx);

                //********************************************************************************
                //Perform some pre-processed retrieval of data from the Repository... 
                //Notice Pagination processing is pushed down to the Repository layer also!
                var repoDbParams = new GraphQLRepoDbParams<CharacterDbModel>(graphQLParams);

                var friends = await repository.GetCharacterFriendsAsync(character.Id);

                //TODO: Fix this so that the paging implementation is pushed to the SQL Query, currently 
                //      not available until Where Filtering is added to the RepoDb GraphQLBatchSliceQury method...
                var pagedFriends = friends.SliceAsCursorPage(graphQLParams.PagingArgs);
                return new PreProcessedCursorSlice<ICharacter>(pagedFriends);
                //********************************************************************************
            });
        }
    }
}