using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Utils;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Stores
{
    public class RavenUserSecretStore<TUser, TUserSecret> : RavenIdentityStore<TUser>, IUserSecretStore
        where TUser : RavenUser where TUserSecret : UserSecret
    {
        public RavenUserSecretStore(IAsyncDocumentSession documentSession)
            : base(documentSession)
        {
        }

        public async Task<IUserSecret> Find(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName");
            }

            IEnumerable<UserSecret> userSecrets = await DocumentSession.Query<TUser>()
                .Where(user => user.UserName == userName)
                .Take(1)
                .Select(user => user.Secret)
                .ToListAsync()
                .ConfigureAwait(false);

            return userSecrets.FirstOrDefault();
        }

        public async Task<bool> Create(IUserSecret userSecret)
        {
            if (userSecret == null)
            {
                throw new ArgumentNullException("userSecret");
            }

            bool result;
            TUserSecret tUserSecret = userSecret as TUserSecret;
            TUser user = await GetUserByUserName(userSecret.UserName).ConfigureAwait(false);

			if (tUserSecret == null || user == null || user.Secret != null)
			{
				result = false;
			}
            else
            {
                tUserSecret.Secret = Crypto.HashPassword(tUserSecret.Secret);
                user.Secret = tUserSecret;
                result = true;
            }

            return result;
        }

        public async Task<bool> Update(string userName, string newSecret)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName");
            }

            if (string.IsNullOrWhiteSpace(newSecret))
            {
                throw new ArgumentException("newSecret");
            }

            bool result;
            TUser user = await GetUserByUserName(userName).ConfigureAwait(false);
			
			if (user != null && user.Secret != null)
			{
				user.Secret.Secret = Crypto.HashPassword(newSecret);
				result = true;
			}
			else
			{
				result = false;
			}

			return result;
        }

        public async Task<bool> Delete(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName");
            }

            bool result;
            TUser user = await GetUserByUserName(userName).ConfigureAwait(false);

            if (user != null && user.Secret != null)
            {
                user.Secret = null;
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> Validate(string userName, string loginSecret)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName");
            }

            if (string.IsNullOrWhiteSpace(loginSecret))
            {
                throw new ArgumentException("loginSecret");
            }

            bool result;
            TUser user = await GetUserByUserName(userName).ConfigureAwait(false);
            if (user != null && user.Secret != null)
            {
                result = Crypto.VerifyHashedPassword(user.Secret.Secret, loginSecret);
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}