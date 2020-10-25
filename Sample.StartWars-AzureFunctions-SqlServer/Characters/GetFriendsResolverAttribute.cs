//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using HotChocolate.PreProcessedExtensions;
//using HotChocolate.PreProcessedExtensions.Pagination;
//using HotChocolate.Types;
//using HotChocolate.Types.Descriptors;
//using StarWars.Repositories;

//namespace StarWars.Characters
//{
//    public sealed class GetFriendsResolverAttribute
//        : ObjectFieldDescriptorAttribute
//    {
//        public override void OnConfigure(
//            IDescriptorContext context,
//            IObjectFieldDescriptor descriptor,
//            MemberInfo member)
//        {
//            descriptor.Resolve(async ctx =>
//            {
//                ICharacter character = ctx.Parent<ICharacter>();
//                ICharacterRepository repository = ctx.Service<ICharacterRepository>();

//                //********************************************************************************
//                //Perform some pre-processed Paging (FYI, without sorting this may be unprdeicatble
//                //  but works here due to the in-memory store used by Star Wars example!
//                var graphQLParams = new GraphQLParamsContext(ctx);
//                var friends = await repository.GetCharacterFriendsAsync(character);

//                var pagedFriends = friends.SliceAsCursorPage(graphQLParams.PagingArgs);
//                return new PreProcessedCursorSliceResults<ICharacter>(pagedFriends);
//                //********************************************************************************
//            });
//        }
//    }
//}