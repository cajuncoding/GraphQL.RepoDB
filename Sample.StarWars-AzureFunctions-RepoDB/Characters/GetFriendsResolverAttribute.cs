using System.Reflection;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.RepoDb;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using StarWars.Characters.DbModels;
using StarWars.Repositories;

namespace StarWars.Characters
{
    public sealed class GetFriendsResolverAttribute : ObjectFieldDescriptorAttribute
    {
        protected override void OnConfigure(
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

                //Get RepoDb specific mapper for the GraphQL parameter context...
                //Note: It's important that we map to the DB Model (not the GraphQL model).
                var repoDbParams = new GraphQLRepoDbMapper<CharacterDbModel>(graphQLParams);

                //Now we can retrieve the related and paginated data from the Repo...
                var pagedFriends = await repository.GetCharacterFriendsAsync(character.Id, repoDbParams.GetCursorPagingParameters());
                return new PreProcessedCursorSlice<ICharacter>(pagedFriends);
                //********************************************************************************
            });
        }
    }
}