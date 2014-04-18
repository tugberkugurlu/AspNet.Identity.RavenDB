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
            const string userName = "Tugberk";
            const string loginProvider = "Twitter";
            const string providerKey = "12345678";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    IUserLoginStore<RavenUser, string> userLoginStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser user = new RavenUser(userName);
                    await ses.StoreAsync(user);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    IUserLoginStore<RavenUser, string> userLoginStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser user = await ses.LoadAsync<RavenUser>(RavenUser.GenerateKey(userName));

                    // Act
                    UserLoginInfo loginToAdd = new UserLoginInfo(loginProvider, providerKey);
                    await userLoginStore.AddLoginAsync(user, loginToAdd);
                    await ses.SaveChangesAsync();

                    // Assert
                    RavenUserLogin foundLogin = await ses.LoadAsync<RavenUserLogin>(RavenUserLogin.GenerateKey(loginProvider, providerKey));
                    Assert.Equal(1, user.Logins.Count());
                    Assert.NotNull(foundLogin);
                }
            }
        }

        [Fact]
        public async Task Add_Should_Add_New_Login_Just_After_UserManager_CreateAsync_Get_Called()
        {
            const string userName = "Tugberk";
            const string loginProvider = "Twitter";
            const string providerKey = "12345678";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    RavenUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                    UserManager<RavenUser> userManager = new UserManager<RavenUser>(userStore);

                    RavenUser user = new RavenUser(userName);
                    UserLoginInfo loginToAdd = new UserLoginInfo(loginProvider, providerKey);
                    await userManager.CreateAsync(user);
                    await userManager.AddLoginAsync(user.Id, loginToAdd);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    IUserLoginStore<RavenUser, string> userLoginStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser user = await ses.LoadAsync<RavenUser>(RavenUser.GenerateKey(userName));
                    RavenUserLogin foundLogin = await ses.LoadAsync<RavenUserLogin>(RavenUserLogin.GenerateKey(loginProvider, providerKey));

                    // Assert
                    Assert.Equal(1, user.Logins.Count());
                    Assert.NotNull(foundLogin);
                }
            }
        }

        [Fact]
        public async Task FindAsync_Should_Find_The_User_If_Login_Exists()
        {
            const string userName = "Tugberk";
            const string loginProvider = "Twitter";
            const string providerKey = "12345678";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                // Arrange
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    IUserLoginStore<RavenUser, string> userLoginStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser user = new RavenUser(userName);
                    RavenUserLogin userLogin = new RavenUserLogin(user.Id, new UserLoginInfo(loginProvider, providerKey));
                    user.AddLogin(userLogin);
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userLogin);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    IUserLoginStore<RavenUser, string> userLoginStore = new RavenUserStore<RavenUser>(ses);

                    // Act
                    UserLoginInfo loginInfo = new UserLoginInfo(loginProvider, providerKey);
                    RavenUser foundUser = await userLoginStore.FindAsync(loginInfo);

                    // Assert
                    Assert.NotNull(foundUser);
                    Assert.Equal(userName, foundUser.UserName);
                }
            }
        }
    }
}