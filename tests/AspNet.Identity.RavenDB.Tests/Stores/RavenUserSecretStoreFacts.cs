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
        [Fact]
        public async Task Create_Should_Create_Secret_If_User_Exists_And_Secret_Does_Not()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = userName });
                await ses.SaveChangesAsync();

                bool result = await userSecretStore.Create(new UserSecret { UserName = userName, Secret = userSecret });
                await ses.SaveChangesAsync();

                RavenUser user = await ses.LoadAsync<RavenUser>("RavenUsers/1");

                Assert.True(result);
                Assert.NotNull(user.Secret);
            }
        }
    }
}
