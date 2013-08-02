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

            IUserSecret secret = await GetSecret(userName);
            return secret;
        }

        public async Task<bool> Create(IUserSecret userSecret)
        {
            if (userSecret == null)
            {
                throw new ArgumentNullException("userSecret");
            }

            bool result;
            TUserSecret tUserSecret = userSecret as TUserSecret;
            IUserSecret existingSecret = await GetSecret(userSecret.UserName).ConfigureAwait(false);

            if (tUserSecret == null || existingSecret != null)
            {
                result = false;
            }
            else
            {
                tUserSecret.Secret = Crypto.HashPassword(tUserSecret.Secret);
                await DocumentSession.StoreAsync(tUserSecret).ConfigureAwait(false);
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
            IUserSecret existingSecret = await GetSecret(userName).ConfigureAwait(false);

            if (existingSecret != null)
			{
                existingSecret.Secret = Crypto.HashPassword(newSecret);
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
            TUserSecret existingSecret = await GetSecret(userName).ConfigureAwait(false);

            if (existingSecret != null)
            {
                DocumentSession.Delete<TUserSecret>(existingSecret);
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
            IUserSecret existingSecret = await GetSecret(userName).ConfigureAwait(false);

            if (existingSecret != null)
            {
                result = Crypto.VerifyHashedPassword(existingSecret.Secret, loginSecret);
            }
            else
            {
                result = false;
            }

            return result;
        }

        // privates

        private async Task<TUserSecret> GetSecret(string userName)
        {
            IEnumerable<TUserSecret> secrets = await DocumentSession
                .Query<TUserSecret>()
                .Where(secret => secret.UserName == userName)
                .Take(1)
                .ToListAsync().ConfigureAwait(false);

            return secrets.FirstOrDefault();
        }
    }
}