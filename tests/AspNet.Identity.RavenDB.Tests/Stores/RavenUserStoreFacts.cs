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
                IUserStore userStore = new RavenUserStore<RavenUser>(ses);
                bool result = await userStore.Create(new RavenUser { UserName = userName });
                await ses.SaveChangesAsync();

                IUser user = (await ses.Query<RavenUser>()
                    .Where(usr => usr.UserName == userName)
                    .Take(1)
                    .ToListAsync()
                    .ConfigureAwait(false)).FirstOrDefault();

                Assert.True(result);
                Assert.NotNull(user);
            }
        }

        [Fact]
        public async Task Should_Not_Create_Duplicate_User()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserStore userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser { UserName = userName });
                await ses.SaveChangesAsync();

                bool result = await userStore.Create(new RavenUser { UserName = userName });

                Assert.False(result);
            }
        }

        [Fact]
        public async Task Should_Retrieve_User_By_UserName()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserStore userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser { UserName = userName });
                await ses.SaveChangesAsync();

                IUser user = await userStore.FindByUserName(userName);

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
                IUserStore userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser { UserName = userName });
                await ses.SaveChangesAsync();

                IUser user = await userStore.FindByUserName(nonExistingUserName);

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
                IUserStore userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser { Id = userId, UserName = userName });
                await ses.SaveChangesAsync();

                IUser user = await userStore.Find(userId);

                Assert.NotNull(user);
                Assert.Equal(userName, user.UserName, StringComparer.InvariantCultureIgnoreCase);
            }
        }

        [Fact]
        public async Task Should_Remove_User()
        {
            string userName = "Tugberk";
            string userId = "RavenUser/2";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserStore userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser { Id = userId, UserName = userName });
                await ses.SaveChangesAsync();

                bool result = await userStore.Delete(userId);
                await ses.SaveChangesAsync();

                IUser user = await ses.LoadAsync<RavenUser>(userId).ConfigureAwait(false);

                Assert.True(result);
                Assert.Null(user);
            }
        }
    }
}
