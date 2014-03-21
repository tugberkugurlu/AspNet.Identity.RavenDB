using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Abstractions.Exceptions;
using Raven.Client;
using System;
using System.Threading.Tasks;
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
            const string userId = "RavenUsers/Tugberk";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { Email = email };
                    RavenUserEmail userEmail = new RavenUserEmail(email, user.Id);
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
            const string email = "tugberk@example.com";
            const string emailToLookFor = "foobar@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { Email = email };
                    RavenUserEmail userEmail = new RavenUserEmail(email, user.Id);
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
            const string userId = "RavenUsers/Tugberk";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { Email = email };
                    RavenUserEmail userEmail = new RavenUserEmail(email, user.Id);
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
            const string userId = "RavenUsers/Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName);
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
            const string userId = "RavenUsers/Tugberk";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { Email = email };
                    RavenUserEmail userEmail = new RavenUserEmail(email, user.Id);
                    RavenUserEmailConfirmation userEmailConfirmation = new RavenUserEmailConfirmation { ConfirmedOn = DateTimeOffset.UtcNow };
                    userEmail.ConfirmationRecord = userEmailConfirmation;
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userEmail);
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
            const string userId = "RavenUsers/Tugberk";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { Email = email };
                    RavenUserEmail userEmail = new RavenUserEmail(email, user.Id);
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
            const string userId = "RavenUsers/Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { UserName = userName };
                    await ses.StoreAsync(user);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId);
                    
                    await Assert.ThrowsAsync<InvalidOperationException>(async () => 
                    {
                        bool isConfirmed = await userEmailStore.GetEmailConfirmedAsync(ravenUser);
                    });
                }
            }
        }

        // SetEmailAsync

        [Fact]
        public async Task SetEmailAsync_Should_Set_The_Email_Correctly()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/Tugberk";
            const string emailToSave = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { UserName = userName };
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
            const string email = "tugberk@example.com";
            const string userName2 = "Tugberk2";
            const string userId2 = "RavenUsers/Tugberk2";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { UserName = userName, Email = email };
                    RavenUser user2 = new RavenUser(userName2);
                    RavenUserEmail userEmail = new RavenUserEmail(email, user.Id);
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

                    await Assert.ThrowsAsync<ConcurrencyException>(async () =>
                    {
                        await ses.SaveChangesAsync();
                    });
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId2);
                    Assert.Null(ravenUser.Email);
                }
            }
        }

        // SetEmailConfirmedAsync

        [Fact]
        public async Task SetEmailConfirmedAsync_With_Confirmed_Param_True_Should_Set_The_Email_As_Confirmed_If_Not_Confirmed_Already()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/Tugberk";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { UserName = userName, Email = email };
                    RavenUserEmail userEmail = new RavenUserEmail(email, user.Id);
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userEmail);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId);
                    await userEmailStore.SetEmailConfirmedAsync(ravenUser, confirmed: true);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    string keyToLookFor = RavenUserEmail.GenerateKey(email);
                    RavenUserEmail userEmail = await ses.LoadAsync<RavenUserEmail>(keyToLookFor);

                    Assert.NotNull(userEmail.ConfirmationRecord);
                    Assert.NotEqual(default(DateTimeOffset), userEmail.ConfirmationRecord.ConfirmedOn);
                }
            }
        }

        [Fact]
        public async Task SetEmailConfirmedAsync_With_Confirmed_Param_False_Should_Set_The_Email_As_Not_Confirmed_If_Confirmed_Already()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/Tugberk";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { Email = email };
                    RavenUserEmail userEmail = new RavenUserEmail(email, user.Id);
                    userEmail.ConfirmationRecord = new RavenUserEmailConfirmation { ConfirmedOn = DateTimeOffset.UtcNow };
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userEmail);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId);
                    await userEmailStore.SetEmailConfirmedAsync(ravenUser, confirmed: false);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    string keyToLookFor = RavenUserEmail.GenerateKey(email);
                    RavenUserEmail userEmail = await ses.LoadAsync<RavenUserEmail>(keyToLookFor);

                    Assert.Null(userEmail.ConfirmationRecord);
                }
            }
        }

        [Fact]
        public async Task SetEmailConfirmedAsync_Should_Throw_InvalidOperationException_If_User_Email_Property_Is_Not_Available()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/Tugberk";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName);
                    RavenUserEmail userEmail = new RavenUserEmail(email, user.Id);
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userEmail);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId);

                    await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                    {
                        await userEmailStore.SetEmailConfirmedAsync(ravenUser, confirmed: true);
                    });
                }
            }
        }

        [Fact]
        public async Task SetEmailConfirmedAsync_Should_Throw_InvalidOperationException_If_User_Email_Property_Is_Available_But_UserEmail_Document_Not()
        {
            const string userName = "Tugberk";
            const string userId = "RavenUsers/Tugberk";
            const string email = "tugberk@example.com";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    RavenUser user = new RavenUser(userName) { Email = email };
                    await ses.StoreAsync(user);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    IUserEmailStore<RavenUser> userEmailStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser ravenUser = await ses.LoadAsync<RavenUser>(userId);

                    await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                    {
                        await userEmailStore.SetEmailConfirmedAsync(ravenUser, confirmed: true);
                    });
                }
            }
        }
    }
}