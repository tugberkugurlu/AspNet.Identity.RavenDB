using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Abstractions.Exceptions;
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
                    RavenUser user = new RavenUser { Id = userId, UserName = userName, Email = email };
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
                    RavenUser user = new RavenUser { Id = userId, UserName = userName, Email = email };
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
                    RavenUser user = new RavenUser { Id = userId, UserName = userName, Email = email };
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

        // GetEmailConfirmedAsync

        [Fact]
        public async Task GetEmailConfirmedAsync_Should_Return_True_If_Email_Confirmed()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/1";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser { Id = userId, UserName = userName, Email = email };
                    RavenUserEmail userEmail = new RavenUserEmail(email) { UserId = userId };
                    RavenUserEmailConfirmation userEmailConfirmation = new RavenUserEmailConfirmation(userName, email) { ConfirmedOn = DateTimeOffset.UtcNow };
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userEmail);
                    await ses.StoreAsync(userEmailConfirmation);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId);
                    bool isConfirmed = await userEmailStore.GetEmailConfirmedAsync(ravenUser);

                    Assert.True(isConfirmed);
                }
            }
        }

        [Fact]
        public async Task GetEmailConfirmedAsync_Should_Return_False_If_Email_Is_Not_Confirmed()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/1";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser { Id = userId, UserName = userName, Email = email };
                    RavenUserEmail userEmail = new RavenUserEmail(email) { UserId = userId };
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userEmail);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId);
                    bool isConfirmed = await userEmailStore.GetEmailConfirmedAsync(ravenUser);

                    Assert.False(isConfirmed);
                }
            }
        }

        [Fact]
        public async Task GetEmailConfirmedAsync_Should_Throw_InvalidOperationException_If_Email_Is_Not_Available()
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
                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        try
                        {
                            bool isConfirmed = userEmailStore.GetEmailConfirmedAsync(ravenUser).Result;
                        }
                        catch (AggregateException ex)
                        {
                            throw ex.GetBaseException();
                        }
                    });
                }
            }
        }

        // SetEmailAsync

        [Fact]
        public async Task SetEmailAsync_Should_Set_The_Email_Correctly()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/1";
            const string emailToSave = "tugberk@example.com";

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
                    await userEmailStore.SetEmailAsync(ravenUser, emailToSave);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    string keyToLookFor = RavenUserEmail.GenerateKey(emailToSave);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId);
                    RavenUserEmail userEmail = await ses.LoadAsync<RavenUserEmail>(keyToLookFor);

                    Assert.NotNull(userEmail);
                    Assert.Equal(emailToSave, ravenUser.Email);
                    Assert.Equal(emailToSave, userEmail.Email);
                    Assert.Equal(userId, userEmail.UserId);
                }
            }
        }

        [Fact]
        public async Task SetEmailAsync_Should_Set_Email_And_SaveChangesAsync_Should_Throw_ConcurrencyException_If_The_Email_Already_Exists()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/1";
            const string email = "tugberk@example.com";
            const string userName2 = "Tugberk2";
            const string userId2 = "RavenUsers/2";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser { Id = userId, UserName = userName, Email = email };
                    RavenUser user2 = new RavenUser { Id = userId2, UserName = userName2 };
                    RavenUserEmail userEmail = new RavenUserEmail(email) { UserId = userId };
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(user2);
                    await ses.StoreAsync(userEmail);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId2);
                    await userEmailStore.SetEmailAsync(ravenUser, email);

                    Assert.Throws<ConcurrencyException>(() =>
                    {
                        try
                        {
                            ses.SaveChangesAsync().Wait();
                        }
                        catch (AggregateException ex)
                        {
                            throw ex.GetBaseException();
                        }
                    });
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId2);
                    Assert.Null(ravenUser.Email);
                }
            }
        }
    }
}