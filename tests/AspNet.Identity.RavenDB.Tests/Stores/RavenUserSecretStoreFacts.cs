using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using AspNet.Identity.RavenDB.Utils;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

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
                bool result = await userSecretStore.Create(new UserSecret { UserName = userName, Secret = userSecret });
                await ses.SaveChangesAsync();

                UserSecret secret = await GetSecret(ses, userName);

                Assert.True(result);
                Assert.NotNull(secret.Secret);
            }
        }

        [Fact]
        public async Task Find_Should_Get_The_Secret_If_User_And_Secret_Exists()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";
            string hashedUserSecret = Crypto.HashPassword(userSecret);

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                RavenUser user = new RavenUser 
                { 
                    Id = "RavenUsers/1", 
                    UserName = userName
                };

                UserSecret secret = new UserSecret
                {
                    UserName = userName,
                    Secret = hashedUserSecret
                };

                await ses.StoreAsync(secret);
                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                IUserSecret secretToFind = await userSecretStore.Find(userName);

                Assert.NotNull(secretToFind);
                Assert.Equal(userName, secretToFind.UserName);
            }
        }

        [Fact]
        public async Task Update_Should_Update_The_User_Secret_If_User_Exists()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";
            string hashedUserSecret = Crypto.HashPassword(userSecret);
            string newUserSecret = "0987654321mnbvcxz";
            string newUserSecretHash = Crypto.HashPassword(newUserSecret);

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                RavenUser user = new RavenUser
                {
                    Id = "RavenUsers/1",
                    UserName = userName
                };

                UserSecret secret = new UserSecret
                {
                    UserName = userName,
                    Secret = hashedUserSecret
                };

                await ses.StoreAsync(secret);
                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                bool result = await userSecretStore.Update(userName, newUserSecret);
                await ses.SaveChangesAsync();

                // Assert
                Assert.True(result);
                Assert.True(Crypto.VerifyHashedPassword(
                    (await GetSecret(ses, userName)).Secret, newUserSecret));
            }
        }

        [Fact]
        public async Task Update_Should_Not_Update_The_User_Secret_If_User_Does_Not_Exist()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";
            string hashedUserSecret = Crypto.HashPassword(userSecret);
            string newUserSecret = "0987654321mnbvcxz";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                RavenUser user = new RavenUser
                {
                    Id = "RavenUsers/1",
                    UserName = userName
                };

                UserSecret secret = new UserSecret
                {
                    UserName = userName,
                    Secret = hashedUserSecret
                };

                await ses.StoreAsync(secret);
                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                bool result = await userSecretStore.Update("FooBar", newUserSecret);
                await ses.SaveChangesAsync();

                // Assert
                Assert.False(result);
                Assert.True(Crypto.VerifyHashedPassword(
                    (await GetSecret(ses, userName)).Secret, userSecret));
            }
        }

        [Fact]
        public async Task Delete_Should_Remove_The_User_Secret_If_User_Exists()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";
            string hashedUserSecret = Crypto.HashPassword(userSecret);

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                RavenUser user = new RavenUser
                {
                    Id = "RavenUsers/1",
                    UserName = userName
                };

                UserSecret secret = new UserSecret
                {
                    UserName = userName,
                    Secret = hashedUserSecret
                };

                await ses.StoreAsync(secret);
                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                bool result = await userSecretStore.Delete(userName);
                await ses.SaveChangesAsync();

                // Assert
                Assert.True(result);
                Assert.Null((await GetSecret(ses, userName)));
            }
        }

        [Fact]
        public async Task Delete_Should_Not_Remove_The_User_Secret_If_User_Does_Not_Exist()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";
            string hashedUserSecret = Crypto.HashPassword(userSecret);

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                RavenUser user = new RavenUser
                {
                    Id = "RavenUsers/1",
                    UserName = userName
                };

                UserSecret secret = new UserSecret
                {
                    UserName = userName,
                    Secret = hashedUserSecret
                };

                await ses.StoreAsync(secret);
                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                bool result = await userSecretStore.Delete("FooBar");
                await ses.SaveChangesAsync();

                // Assert
                Assert.False(result);
                Assert.NotNull((await GetSecret(ses, userName)));
            }
        }

        [Fact]
        public async Task Validate_Should_Validate_Successfully_If_User_Secret_Is_Correct()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";
            string hashedUserSecret = Crypto.HashPassword(userSecret);

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                RavenUser user = new RavenUser
                {
                    Id = "RavenUsers/1",
                    UserName = userName
                };

                UserSecret secret = new UserSecret
                {
                    UserName = userName,
                    Secret = hashedUserSecret
                };

                await ses.StoreAsync(secret);
                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                bool result = await userSecretStore.Validate(userName, userSecret);
                await ses.SaveChangesAsync();

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async Task Validate_Should_Not_Validate_Successfully_If_User_Does_Not_Exist()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";
            string hashedUserSecret = Crypto.HashPassword(userSecret);

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                RavenUser user = new RavenUser
                {
                    Id = "RavenUsers/1",
                    UserName = userName
                };

                UserSecret secret = new UserSecret
                {
                    UserName = userName,
                    Secret = hashedUserSecret
                };

                await ses.StoreAsync(secret);
                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                bool result = await userSecretStore.Validate("FooBar", userSecret);
                await ses.SaveChangesAsync();

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task Validate_Should_Not_Validate_Successfully_If_User_Secret_Is_Not_Correct()
        {
            string userName = "Tugberk";
            string userSecret = "1234567890qwertyuiop";
            string hashedUserSecret = Crypto.HashPassword(userSecret);

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserSecretStore userSecretStore = new RavenUserSecretStore<RavenUser, UserSecret>(ses);
                RavenUser user = new RavenUser
                {
                    Id = "RavenUsers/1",
                    UserName = userName
                };

                UserSecret secret = new UserSecret
                {
                    UserName = userName,
                    Secret = hashedUserSecret
                };

                await ses.StoreAsync(secret);
                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                bool result = await userSecretStore.Validate(userName, "FooBar");
                await ses.SaveChangesAsync();

                // Assert
                Assert.False(result);
            }
        }

        // privates

        private async Task<UserSecret> GetSecret(IAsyncDocumentSession ses, string userName)
        {
            IEnumerable<UserSecret> secrets = await ses
                .Query<UserSecret>()
                .Where(secret => secret.UserName == userName)
                .Take(1)
                .ToListAsync().ConfigureAwait(false);

            return secrets.FirstOrDefault();
        }
    }
}
