using AspNet.Identity.RavenDB.Entities;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Stores
{
    public class RavenUserClaimStore<TUser, TUserClaim> : RavenIdentityStore<TUser>, IUserClaimStore
        where TUser : RavenUser where TUserClaim : UserClaim
    {
        public RavenUserClaimStore(IAsyncDocumentSession documentSession)
            : base(documentSession)
        {
        }

        public async Task<IEnumerable<IUserClaim>> GetUserClaims(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            TUser user = await GetUserById(userId).ConfigureAwait(false);

            return user == null
                ? Enumerable.Empty<UserClaim>()
                : user.UserClaims;
        }

        public async Task<bool> Add(IUserClaim userClaim)
        {
            if (userClaim == null)
            {
                throw new ArgumentNullException("userClaim");
            }

            if (string.IsNullOrWhiteSpace(userClaim.ClaimType))
            {
                throw new ArgumentException("userClaim.ClaimType");
            }

            if (string.IsNullOrWhiteSpace(userClaim.ClaimValue))
            {
                throw new ArgumentException("userClaim.ClaimValue");
            }

            bool result;
            TUserClaim tUserClaim = userClaim as TUserClaim;

            if (tUserClaim == null)
            {
                result = false;
            }
            else
            {
                TUser user = await GetUserById(userClaim.UserId).ConfigureAwait(false);
                if (user == null || user.UserClaims.Any(claim =>
                    claim.ClaimType.Equals(userClaim.ClaimType, StringComparison.InvariantCultureIgnoreCase) &&
                    claim.ClaimValue.Equals(userClaim.ClaimValue, StringComparison.InvariantCultureIgnoreCase)))
                {
                    result = false;
                }
                else
                {
                    user.UserClaims.Add(tUserClaim);
                    result = true;
                }
            }

            return result;
        }

        public async Task<bool> Remove(string userId, string claimType, string claimValue)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            if (string.IsNullOrWhiteSpace(claimType))
            {
                throw new ArgumentException("claimType");
            }

            if (string.IsNullOrWhiteSpace(claimValue))
            {
                throw new ArgumentException("claimValue");
            }

            bool result;
            TUser user = await GetUserById(userId).ConfigureAwait(false);
            if (user == null)
            {
                result = false;
            }
            else
            {
                UserClaim userClaim = user.UserClaims.FirstOrDefault(claim =>
                    claim.ClaimType.Equals(claimType, StringComparison.InvariantCultureIgnoreCase) &&
                    claim.ClaimValue.Equals(claimValue, StringComparison.InvariantCultureIgnoreCase));

                if (userClaim == null)
                {
                    result = false;
                }
                else
                {
                    user.UserClaims.Remove(userClaim);
                    result = true;
                }
            }

            return result;
        }
    }
}
