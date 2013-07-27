using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Stores
{
    public class RavenUserStore<TUser> : RavenIdentityStore<TUser>, IUserStore where TUser : RavenUser
    {
        public RavenUserStore(IAsyncDocumentSession documentSession) : base(documentSession)
        {
        }

        public async Task<IUser> Find(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            return await DocumentSession.LoadAsync<TUser>(userId).ConfigureAwait(false);
        }

        public async Task<IUser> FindByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName");
            }

            IEnumerable<TUser> users = await DocumentSession.Query<TUser>()
                .Where(usr => usr.UserName == userName)
                .Take(1)
                .ToListAsync()
                .ConfigureAwait(false);

            return users.FirstOrDefault();
        }

        public async Task<bool> Create(IUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new ArgumentException("user.UserName");
            }

            bool result;
            TUser tUser = user as TUser;
            if (tUser == null)
            {
                result = false;
            }
            else
            {
                // TODO: This's poor man's uniqueness constraint and not safe. Find a better way.
                TUser existingUser = await GetUserByUserName(user.UserName);
                if (existingUser != null)
                {
                    result = false;
                }
                else
                {
                    await DocumentSession.StoreAsync(tUser).ConfigureAwait(false);
                    result = true;
                }
            }

            return result;
        }

        public async Task<bool> Delete(string userId)
        {
            IUser user = await Find(userId).ConfigureAwait(false);
            TUser tUser = user as TUser;
            if (tUser == null)
            {
                return false;
            }

            DocumentSession.Delete<TUser>(tUser);
            return true;
        }
    }
}