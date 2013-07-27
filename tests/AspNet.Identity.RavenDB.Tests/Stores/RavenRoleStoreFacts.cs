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
    public class RavenRoleStoreFacts : TestBase
    {
        // ------- GetRolesForUser -------

        [Fact]
        public async Task GetRolesForUser_Should_Retrieve_Correct_Roles()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/2", UserName = "Tugberk2", Roles = new List<Role> { new Role { Id = "Admin" } } });
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/3", UserName = "Tugberk3", Roles = new List<Role> { new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                IEnumerable<string> roles = await roleStore.GetRolesForUser("RavenUsers/1");

                Assert.Equal(2, roles.Count());
                Assert.True(roles.Any(role => role.Equals("Admin", StringComparison.InvariantCultureIgnoreCase)));
                Assert.True(roles.Any(role => role.Equals("Guest", StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        [Fact]
        public async Task GetRolesForUser_Should_Return_Empty_Enumerable_If_Roles_Property_Is_Null()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk" });
                await ses.SaveChangesAsync();

                IEnumerable<string> roles = await roleStore.GetRolesForUser("RavenUsers/1");

                Assert.Equal(0, roles.Count());
            }
        }

        [Fact]
        public async Task GetRolesForUser_Should_Return_Empty_Enumerable_If_User_Does_Not_Exist()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                IEnumerable<string> roles = await roleStore.GetRolesForUser("RavenUsers/2");

                Assert.Equal(0, roles.Count());
            }
        }

        // ------- IsUserInRole -------

        [Fact]
        public async Task IsUserInRole_Should_Return_True_If_User_Is_Really_In_Role()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                bool isAdmin = await roleStore.IsUserInRole("RavenUsers/1", "Admin");

                Assert.True(isAdmin);
            }
        }

        [Fact]
        public async Task IsUserInRole_Should_Return_False_If_User_Is_Not_In_Role()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                bool isSalesGuy = await roleStore.IsUserInRole("RavenUsers/1", "SalesGuy");

                Assert.False(isSalesGuy);
            }
        }

        [Fact]
        public async Task IsUserInRole_Should_Return_False_If_User_Does_Not_Exist()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                bool isAdmin = await roleStore.IsUserInRole("RavenUsers/2", "Admin");

                Assert.False(isAdmin);
            }
        }

        // ------- GetUsersInRoles -------

        [Fact]
        public async Task GetUsersInRoles_Should_Return_Correct_Usurs_In_Role()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using(IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/2", UserName = "Tugberk2", Roles = new List<Role> { new Role { Id = "Admin" } } });
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/3", UserName = "Tugberk3", Roles = new List<Role> { new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                IEnumerable<string> users = await roleStore.GetUsersInRoles("Admin");

                Assert.Equal(2, users.Count());
                Assert.Equal("RavenUsers/1", users.ElementAt(0), StringComparer.InvariantCultureIgnoreCase);
                Assert.Equal("RavenUsers/2", users.ElementAt(1), StringComparer.InvariantCultureIgnoreCase);
            }
        }

        [Fact]
        public async Task GetUsersInRoles_Should_Return_Enumberable_Empty_If_There_Is_No_user_In_Role()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/2", UserName = "Tugberk2", Roles = new List<Role> { new Role { Id = "Admin" } } });
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/3", UserName = "Tugberk3", Roles = new List<Role> { new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                IEnumerable<string> users = await roleStore.GetUsersInRoles("Sales");

                Assert.NotNull(users);
                Assert.False(users.Any());
            }
        }

        [Fact]
        public void GetUsersInRoles_Should_Throw_ArgumentException_If_RoleId_Param_Is_Null()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);

                Assert.Throws<ArgumentException>(() => 
                {
                    try
                    {
                        roleStore.GetUsersInRoles(null).Wait();
                    }
                    catch (AggregateException ex)
                    {
                        throw ex.GetBaseException();
                    }
                });
            }
        }

        [Fact]
        public void GetUsersInRoles_Should_Throw_ArgumentException_If_RoleId_Param_Is_Empty()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);

                Assert.Throws<ArgumentException>(() =>
                {
                    try
                    {
                        roleStore.GetUsersInRoles(string.Empty).Wait();
                    }
                    catch (AggregateException ex)
                    {
                        throw ex.GetBaseException();
                    }
                });
            }
        }

        [Fact]
        public void GetUsersInRoles_Should_Throw_ArgumentException_If_RoleId_Param_Is_Whitespace()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);

                Assert.Throws<ArgumentException>(() =>
                {
                    try
                    {
                        roleStore.GetUsersInRoles(" ").Wait();
                    }
                    catch (AggregateException ex)
                    {
                        throw ex.GetBaseException();
                    }
                });
            }
        }

        // ------- AddUserToRole -------

        [Fact]
        public async Task AddUserToRole_Should_Add_The_User_In_Role_If_User_Exists_And_Is_Not_In_Role()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                bool result = await roleStore.AddUserToRole("Sales", "RavenUsers/1");
                await ses.SaveChangesAsync();

                RavenUser user = await ses.LoadAsync<RavenUser>("RavenUsers/1");
                Assert.True(result);
                Assert.True(user.Roles.Any(role => role.Id.Equals("Sales", StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        [Fact]
        public async Task AddUserToRole_Should_Add_The_User_In_Roles_Consecutively_If_User_Exists_And_Is_Not_In_Role()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                bool result = await roleStore.AddUserToRole("Sales", "RavenUsers/1");
                bool result2 = await roleStore.AddUserToRole("Accounting", "RavenUsers/1");
                await ses.SaveChangesAsync();

                RavenUser user = await ses.LoadAsync<RavenUser>("RavenUsers/1");

                Assert.True(result);
                Assert.True(result2);
                Assert.True(user.Roles.Any(role => role.Id.Equals("Sales", StringComparison.InvariantCultureIgnoreCase)));
                Assert.True(user.Roles.Any(role => role.Id.Equals("Accounting", StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        // privates
        private static async Task AddUsers(IAsyncDocumentSession ses)
        {
            await ses.StoreAsync(new RavenUser { UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
            await ses.StoreAsync(new RavenUser { UserName = "Tugberk2", Roles = new List<Role> { new Role { Id = "Admin" } } });
            await ses.StoreAsync(new RavenUser { UserName = "Tugberk2", Roles = new List<Role> { new Role { Id = "Guest" } } });
            await ses.SaveChangesAsync();
        }
    }
}
