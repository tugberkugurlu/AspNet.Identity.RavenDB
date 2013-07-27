using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Indexes;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Stores
{
    public class RavenRoleStore<TUser, TRole> : RavenIdentityStore<TUser>, IRoleStore 
        where TUser : RavenUser where TRole : Role, new()
    {
        public RavenRoleStore(IAsyncDocumentSession documentSession)
            : base(documentSession)
        {
        }

        public async Task<bool> IsUserInRole(string userId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("roleId");
            }

            bool result = await (from user in DocumentSession.Query<TUser>()
                                 where user.Id == userId && user.Roles.Any(role => role.Id == roleId)
                                 select user).AnyAsync();

            return result;
        }

        public async Task<IEnumerable<string>> GetRolesForUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            TUser user = await GetUserById(userId).ConfigureAwait(false);

            return user == null || user.Roles == null
                ? Enumerable.Empty<string>()
                : user.Roles.Select(role => role.Id);
        }

        public async Task<IEnumerable<string>> GetUsersInRoles(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("roleId");
            }

            IEnumerable<string> users = await DocumentSession.Query<TUser>()
                .Where(user => user.Roles.Any(role => role.Id == roleId))
                .Select(user => user.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            return users;
        }

        public async Task<bool> AddUserToRole(string roleId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("roleId");
            }

            bool result;
            TUser user = await DocumentSession.LoadAsync<TUser>(userId).ConfigureAwait(false);

            if (user == null || user.Roles.Any(role => role.Id == roleId))
            {
                result = false;
            }
            else
            {
                user.Roles.Add(new TRole { Id = roleId });
                result = true;
            }

            return result;
        }

        public async Task<bool> RemoveUserFromRole(string roleId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("roleId");
            }

            bool result;
            IEnumerable<TUser> users = await DocumentSession.Query<TUser>()
                .Where(usr => usr.Id == userId && usr.Roles.Any(role => role.Id == roleId))
                .Take(1).ToListAsync().ConfigureAwait(false);

            TUser user = users.FirstOrDefault();

            if (user == null)
            {
                result = false;
            }
            else
            {
                Role role = user.Roles.FirstOrDefault(rl => rl.Id.Equals(roleId, StringComparison.InvariantCultureIgnoreCase));
                user.Roles.Remove(role);
                result = true;
            }

            return result;
        }

        public async Task<bool> RoleExists(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("roleId");
            }

            bool result = await DocumentSession.Query<RavenUser_Roles.ReduceResult, RavenUser_Roles>()
                .Where(role => role.Name == roleId).AnyAsync();

            return result;
        }

        public Task<bool> CreateRole(IRole role)
        {
            // This's not applicable for RavenDB but return true for now anyway
            return Task.FromResult(true);
        }

        public Task<bool> DeleteRole(string roleId, bool failIfNonEmpty)
        {
            // This's not applicable for RavenDB but return true for now anyway
            return Task.FromResult(true);
        }
    }
}
