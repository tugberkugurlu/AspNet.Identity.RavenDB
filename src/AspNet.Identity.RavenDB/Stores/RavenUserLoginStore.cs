using AspNet.Identity.RavenDB.Entities;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Stores
{
    public class RavenUserLoginStore<TUser, TUserLogin> : RavenIdentityStore<TUser>, IUserLoginStore
        where TUser : RavenUser where TUserLogin : UserLogin
    {
        public RavenUserLoginStore(IAsyncDocumentSession documentSession)
            : base(documentSession)
        {
        }

        public async Task<IList<IUserLogin>> GetLogins(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            TUser user = await GetUserById(userId).ConfigureAwait(false);

            return user == null
                ? Enumerable.Empty<UserLogin>().ToList() as IList<IUserLogin>
                : user.UserLogins.ToList() as IList<IUserLogin>;
        }

        public async Task<string> GetProviderKey(string userId, string loginProvider)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            if (string.IsNullOrWhiteSpace(loginProvider))
            {
                throw new ArgumentException("loginProvider");
            }

            TUser user = await GetUserById(userId).ConfigureAwait(false);
            UserLogin userLogin = user.UserLogins.FirstOrDefault(login => login.LoginProvider.Equals(loginProvider, StringComparison.InvariantCultureIgnoreCase));

            return userLogin == null
                ? null
                : userLogin.ProviderKey;
        }

        public async Task<string> GetUserId(string loginProvider, string providerKey)
        {
            if (string.IsNullOrWhiteSpace(loginProvider))
            {
                throw new ArgumentException("loginProvider");
            }

            if (string.IsNullOrWhiteSpace(providerKey))
            {
                throw new ArgumentException("providerKey");
            }

            IEnumerable<TUser> users = await DocumentSession.Query<TUser>()
                .Where(usr => usr.UserLogins.Any(login => login.LoginProvider == loginProvider && login.ProviderKey == providerKey))
                .Take(1)
                .ToListAsync()
                .ConfigureAwait(false);

            TUser user = users.FirstOrDefault();

            return user == null
                ? null
                : user.Id;
        }

        public async Task<bool> Add(IUserLogin login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            if (string.IsNullOrWhiteSpace(login.UserId))
            {
                throw new ArgumentException("login.UserId");
            }

            if (string.IsNullOrWhiteSpace(login.LoginProvider))
            {
                throw new ArgumentException("login.LoginProvider");
            }

            if (string.IsNullOrWhiteSpace(login.ProviderKey))
            {
                throw new ArgumentException("login.ProviderKey");
            }

            bool result;
            TUserLogin tUserLogin = login as TUserLogin;

            if (tUserLogin == null || await GetUserId(login.LoginProvider, login.ProviderKey) != null)
            {
                result = false;
            }
            else
            {
                TUser user = await GetUserById(login.UserId).ConfigureAwait(false);
                if (user == null)
                {
                    result = false;
                }
                else
                {
                    user.UserLogins.Add(tUserLogin);
                    result = true;
                }
            }

            return result;
        }

        public async Task<bool> Remove(string userId, string loginProvider, string providerKey)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            if (string.IsNullOrWhiteSpace(loginProvider))
            {
                throw new ArgumentException("loginProvider");
            }

            if (string.IsNullOrWhiteSpace(providerKey))
            {
                throw new ArgumentException("providerKey");
            }

            bool result;
            TUser user = await GetUserById(userId).ConfigureAwait(false);

            if (user == null)
            {
                result = false;
            }
            else
            {
                UserLogin login = user.UserLogins.FirstOrDefault(lgn =>
                    lgn.LoginProvider.Equals(loginProvider, StringComparison.InvariantCultureIgnoreCase) &&
                    lgn.ProviderKey.Equals(providerKey, StringComparison.InvariantCultureIgnoreCase));

                if (login == null)
                {
                    result = false;
                }
                else
                {
                    user.UserLogins.Remove(login);
                    result = true;
                }
            }

            return result;
        }
    }
}
