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
                IUserLoginStore<RavenUser> userLoginStore = new RavenUserStore<RavenUser>(ses);
                RavenUser user = new RavenUser { Id = "RavenUsers/1", UserName = userName };
                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                UserLoginInfo loginToAdd = new UserLoginInfo("Local", userName);
                await userLoginStore.AddLoginAsync(user, new UserLoginInfo("Local", userName));

                // Assert
                Assert.Equal(1, user.Logins.Count);
            }
        }
    }
}