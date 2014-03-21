using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Raven.Client;
using System.Threading.Tasks;
using Xunit;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    public class RavenQueryableUserStoreFacts : TestBase
    {
        [Fact]
        public async Task RavenUserStore_Users_Should_Expose_IQueryable_Over_IRavenQueryable()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            {
                const string userName = "Tugberk";
                const string userNameToSearch = "TugberkUgurlu";

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { IsTwoFactorEnabled = false };
                    RavenUser userToSearch = new RavenUser(userNameToSearch) { IsTwoFactorEnabled = false };
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userToSearch);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    // Act
                    RavenUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser retrievedUser = await userStore.Users.FirstOrDefaultAsync(user => user.UserName == userNameToSearch);

                    // Assert
                    Assert.NotNull(retrievedUser);
                    Assert.Equal(userNameToSearch, retrievedUser.UserName);
                }
            }
        }
    }
}