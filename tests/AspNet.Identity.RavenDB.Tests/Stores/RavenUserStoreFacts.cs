using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Raven.Client.Embedded;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    public class RavenUserStoreFacts : TestBase
    {
        [Fact]
        public async Task Should_Create_User()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                await userStore.CreateAsync(new RavenUser { UserName = userName });

                IUser user = (await ses.Query<RavenUser>()
                    .Where(usr => usr.UserName == userName)
                    .Take(1)
                    .ToListAsync()
                    .ConfigureAwait(false)).FirstOrDefault();

                Assert.NotNull(user);
            }
        }

        [Fact]
        public async Task Should_Retrieve_User_By_UserName()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser { UserName = userName });
                await ses.SaveChangesAsync();

                IUser user = await userStore.FindByNameAsync(userName);

                Assert.NotNull(user);
                Assert.Equal(userName, user.UserName, StringComparer.InvariantCultureIgnoreCase);
            }
        }

        [Fact]
        public async Task Should_Return_Null_For_Non_Existing_User_By_UserName()
        {
            string userName = "Tugberk";
            string nonExistingUserName = "Tugberk2";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser { UserName = userName });
                await ses.SaveChangesAsync();

                IUser user = await userStore.FindByNameAsync(nonExistingUserName);

                Assert.Null(user);
            }
        }

        [Fact]
        public async Task Should_Retrieve_User_By_UserId()
        {
            string userName = "Tugberk";
            string userId = "RavenUser/2";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser { Id = userId, UserName = userName });
                await ses.SaveChangesAsync();

                IUser user = await userStore.FindByIdAsync(userId);

                Assert.NotNull(user);
                Assert.Equal(userName, user.UserName, StringComparer.InvariantCultureIgnoreCase);
            }
        }
    }
}