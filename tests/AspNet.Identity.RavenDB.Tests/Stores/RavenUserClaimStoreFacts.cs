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
    public class RavenUserClaimStoreFacts : TestBase
    {
        [Fact]
        public async Task GetUserClaims_Should_Retrieve_Correct_Claims_For_User()
        {
            string userName = "Tugberk";
            string userId = "RavenUsers/1";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserClaimStore userClaimStore = new RavenUserClaimStore<RavenUser, UserClaim>(ses);
                RavenUser user = new RavenUser
                {
                    Id = userId,
                    UserName = userName,
                    UserClaims = new List<UserClaim>
                    {
                        new UserClaim { UserId = userId, ClaimType = "Scope", ClaimValue = "Read" },
                        new UserClaim { UserId = userId, ClaimType = "Scope", ClaimValue = "Write" }
                    }
                };

                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                IEnumerable<IUserClaim> claims = await userClaimStore.GetUserClaims(userId);

                Assert.Equal(2, claims.Count());
                Assert.Equal("Read", claims.ElementAt(0).ClaimValue);
                Assert.Equal("Write", claims.ElementAt(1).ClaimValue);
            }
        }

        [Fact]
        public async Task GetUserClaims_Should_Return_Enumerable_Empty_If_User_Claims_Null()
        {
            string userName = "Tugberk";
            string userId = "RavenUsers/1";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserClaimStore userClaimStore = new RavenUserClaimStore<RavenUser, UserClaim>(ses);
                RavenUser user = new RavenUser
                {
                    Id = userId,
                    UserName = userName
                };

                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                IEnumerable<IUserClaim> claims = await userClaimStore.GetUserClaims(userId);

                // Assert
                Assert.Equal(0, claims.Count());
            }
        }

        [Fact]
        public async Task GetUserClaims_Should_Return_Enumerable_Empty_If_User_Does_Not_Exist()
        {
            string userName = "Tugberk";
            string userId = "RavenUsers/1";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserClaimStore userClaimStore = new RavenUserClaimStore<RavenUser, UserClaim>(ses);
                RavenUser user = new RavenUser
                {
                    Id = userId,
                    UserName = userName
                };

                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                IEnumerable<IUserClaim> claims = await userClaimStore.GetUserClaims("RavenUsers/2");

                // Assert
                Assert.Equal(0, claims.Count());
            }
        }
    }
}
