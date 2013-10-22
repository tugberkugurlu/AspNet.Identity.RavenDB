using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Xunit;

namespace AspNet.Identity.RavenDB.Tests.Stores {
    public class RavenUserRoleStoreFacts : TestBase {
        [Fact]
        public async Task GetRoles_Should_Retrieve_Correct_Roles_For_User() 
        {
            string userName = "Tugberk";
            string userId = "RavenUsers/1";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession()) 
            {
                IUserRoleStore<RavenUser> userRoleStore = new RavenUserStore<RavenUser>(ses);
                IEnumerable<string> roles = new List<string>
                {
                    "Administrator", 
                    "PowerUser"
                };
                RavenUser user = new RavenUser 
                {
                    Id = userId,
                    UserName = userName
                };

                foreach (var role in roles) 
                {
                    user.Claims.Add(
                        new RavenUserClaim 
                        {
                            ClaimType = ClaimTypes.Role, 
                            ClaimValue = role
                        });
                }

                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                IEnumerable<string> retrievedRoles = await userRoleStore.GetRolesAsync(user);

                Assert.Equal(2, retrievedRoles.Count());
                Assert.Equal("Administrator", retrievedRoles.ElementAt(0));
                Assert.Equal("PowerUser", retrievedRoles.ElementAt(1));
            }
        }

        [Fact]
        public async Task AddRole_Should_Add_Correct_Role_To_User() 
        {
            string userName = "Tugberk";
            string userId = "RavenUsers/1";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession()) 
            {
                IUserRoleStore<RavenUser> userRoleStore = new RavenUserStore<RavenUser>(ses);
                IEnumerable<string> roles = new List<string>
                {
                    "Administrator"
                };
                RavenUser user = new RavenUser 
                {
                    Id = userId,
                    UserName = userName
                };

                foreach (var role in roles) 
                {
                    user.Claims.Add(
                        new RavenUserClaim 
                        {
                            ClaimType = ClaimTypes.Role,
                            ClaimValue = role
                        });
                }

                await userRoleStore.AddToRoleAsync(user, "PowerUser");

                Assert.Equal(2, user.Claims.Count());
                Assert.Equal("Administrator", user.Claims.ElementAt(0).ClaimValue);
                Assert.Equal("PowerUser", user.Claims.ElementAt(1).ClaimValue);
            }
        }

        [Fact]
        public async Task RemoveRole_Should_Remove_Correct_Role_To_User() 
        {
            string userName = "Tugberk";
            string userId = "RavenUsers/1";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession()) 
            {
                IUserRoleStore<RavenUser> userRoleStore = new RavenUserStore<RavenUser>(ses);
                IEnumerable<string> roles = new List<string>
                {
                    "Administrator", 
                    "PowerUser"
                };
                RavenUser user = new RavenUser 
                {
                    Id = userId,
                    UserName = userName
                };

                foreach (var role in roles) 
                {
                    user.Claims.Add(
                        new RavenUserClaim 
                        {
                            ClaimType = ClaimTypes.Role,
                            ClaimValue = role
                        });
                }

                await userRoleStore.RemoveFromRoleAsync(user, "Administrator");

                Assert.Equal(1, user.Claims.Count());
                Assert.Equal("PowerUser", user.Claims.ElementAt(0).ClaimValue);
            }
        }
    }
}