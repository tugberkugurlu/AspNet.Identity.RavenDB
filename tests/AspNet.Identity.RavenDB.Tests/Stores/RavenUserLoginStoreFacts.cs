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
    public class RavenUserLoginStoreFacts : TestBase
    {
        [Fact]
        public async Task Add_Should_Add_New_Login_If_User_Exists()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserLoginStore userLoginStore = new RavenUserLoginStore<RavenUser, RavenUserLogin>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = userName });
                await ses.SaveChangesAsync();

                bool result = await userLoginStore.Add(new RavenUserLogin { UserId = "RavenUsers/1", LoginProvider = "Local", ProviderKey = userName });
                await ses.SaveChangesAsync();

                RavenUser user = await ses.LoadAsync<RavenUser>("RavenUsers/1");

                Assert.True(result);
                Assert.Equal(1, user.Logins.Count);
            }
        }
    }
}
