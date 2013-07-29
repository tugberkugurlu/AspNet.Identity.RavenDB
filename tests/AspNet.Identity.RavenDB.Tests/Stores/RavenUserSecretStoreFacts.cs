using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    public class RavenUserSecretStoreFacts : TestBase
    {
        [Fact(Skip = "Issue #1 blocks this test")]
        public async Task Create_Should_Create_Secret_If_User_Exists_And_Secret_Does_Not()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = userName });
                bool result = await userSecretStore.Create(new UserSecret { UserName = userName, Secret = userSecret });
                await ses.SaveChangesAsync();

                RavenUser user = await ses.LoadAsync<RavenUser>("RavenUsers/1");

                Assert.True(result);
                Assert.NotNull(user.Secret);
            }
        }

        [Fact]
        public async Task Find_Should_Get_The_Secret_If_User_And_Secret_Exists()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                RavenUser user = new RavenUser 
                { 
                    Id = "RavenUsers/1", 
                    UserName = userName,
                    Secret = new UserSecret 
                    {
                        UserName = userName,
                        Secret = userSecret
                    }
                };

                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                IUserSecret secret = await userSecretStore.Find(userName);

                Assert.NotNull(secret);
                Assert.Equal(userName, secret.UserName);
            }
        }
    }
}
