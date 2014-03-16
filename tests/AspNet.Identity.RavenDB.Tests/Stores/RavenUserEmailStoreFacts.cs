using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Xunit;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    public class RavenUserEmailStoreFacts : TestBase
    {
        // FindByEmailAsync

        [Fact]
        public async Task FindByEmailAsync_Should_Return_The_Correct_User_If_Available()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/1";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser { Id = userId, UserName = userName };
                    RavenUserEmail userEmail = new RavenUserEmail(email) { UserId = userId };
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userEmail);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser user = await userEmailStore.FindByEmailAsync(email);

                    Assert.NotNull(user);
                    Assert.Equal(userId, user.Id);
                    Assert.Equal(userName, user.UserName);
                }
            }
        }

        [Fact]
        public async Task FindByEmailAsync_Should_Return_Null_If_User_Is_Not_Available()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/1";
            const string email = "tugberk@example.com";
            const string emailToLookFor = "foobar@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser { Id = userId, UserName = userName };
                    RavenUserEmail userEmail = new RavenUserEmail(email) { UserId = userId };
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userEmail);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser user = await userEmailStore.FindByEmailAsync(emailToLookFor);

                    Assert.Null(user);
                }
            }
        }

        // GetEmailAsync

        [Fact]
        public async Task GetEmailAsync_Should_Return_User_Email_If_Available()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/1";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser { Id = userId, UserName = userName };
                    RavenUserEmail userEmail = new RavenUserEmail(email) { UserId = userId };
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userEmail);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId);
                    string userEmail = await userEmailStore.GetEmailAsync(ravenUser);

                    Assert.NotNull(userEmail);
                    Assert.Equal(email, userEmail);
                }
            }
        }

        [Fact]
        public async Task GetEmailAsync_Should_Return_Null_If_User_Email_Is_Not_Available()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/1";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser { Id = userId, UserName = userName };
                    await ses.StoreAsync(user);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId);
                    string userEmail = await userEmailStore.GetEmailAsync(ravenUser);

                    Assert.Null(userEmail);
                }
            }
        }
    }
}
