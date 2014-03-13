using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Entities.Entities;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Stores
{
    public sealed class RavenUserStore<TUser> : RavenIdentityStore<TUser>,
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserEmailStore<TUser>,
        IUserStore<TUser> where TUser : RavenUser
    {
        public RavenUserStore(IAsyncDocumentSession documentSession)
            : this(documentSession, true)
        {
        }

        public RavenUserStore(IAsyncDocumentSession documentSession, bool disposeDocumentSession)
            : base(documentSession, disposeDocumentSession)
        {
        }

        // IQueryableUserStore

        public IQueryable<TUser> Users
        {
            get
            {
                return DocumentSession.Query<TUser>();
            }
        }

        // IUserStore

        /// <remarks>
        /// This method doesn't perform uniquness. That's the responsibility of the session provider.
        /// </remarks>
        public async Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            await DocumentSession.StoreAsync(user).ConfigureAwait(false);
            await DocumentSession.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("Input cannot be null, empty or white space", "userId");
            }

            return GetUser(userId);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Input cannot be null, empty or white space", "userName");
            }

            return GetUserByUserName(userName);
        }

        /// <remarks>
        /// This method assumes that incomming TUser parameter is tracked in the session. So, this method literally behaves as SaveChangeAsync
        /// </remarks>
        public Task UpdateAsync(TUser user)
        {
            return DocumentSession.SaveChangesAsync();
        }

        public Task DeleteAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            DocumentSession.Delete<TUser>(user);
            return DocumentSession.SaveChangesAsync();
        }

        // IUserLoginStore

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<UserLoginInfo>>(
                user.Logins.Select(login => new UserLoginInfo(login.LoginProvider, login.ProviderKey)).ToList()
            );
        }

        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
            IEnumerable<TUser> users = await DocumentSession.Query<TUser>()
                .Where(usr => usr.Logins.Any(lgn => lgn.LoginProvider == login.LoginProvider && lgn.ProviderKey == login.ProviderKey))
                .Take(1)
                .ToListAsync()
                .ConfigureAwait(false);

            return users.FirstOrDefault();
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            user.Logins.Add(new RavenUserLogin(login));
            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            RavenUserLogin userLogin = user.Logins
                .FirstOrDefault(lgn => lgn.LoginProvider == login.LoginProvider && lgn.ProviderKey == login.ProviderKey);

            if (userLogin != null)
            {
                user.Logins.Remove(userLogin);
            }

            return Task.FromResult(0);
        }

        // IUserClaimStore

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return Task.FromResult<IList<Claim>>(user.Claims.Select(clm => new Claim(clm.ClaimType, clm.ClaimValue)).ToList());
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            user.Claims.Add(new RavenUserClaim(claim));
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            RavenUserClaim userClaim = user.Claims
                .FirstOrDefault(clm => clm.ClaimType == claim.Type && clm.ClaimValue == claim.Value);

            if (userClaim != null)
            {
                user.Claims.Remove(userClaim);
            }

            return Task.FromResult(0);
        }

        // IUserPasswordStore

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<string>(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<bool>(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        // IUserSecurityStampStore

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<string>(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        // IUserTwoFactorStore

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.IsTwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.IsTwoFactorEnabled = enabled;
            return Task.FromResult<int>(0);
        }

        // IUserEmailStore

        public Task<TUser> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.Email);
        }

        public async Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(user.Email) == true)
            {
                throw new InvalidOperationException("Cannot set the E-mail as confirmed because the 'Email' property on the 'user' parameter is null.");
            }

            string keyToLookFor = RavenUserEmailConfirmation.GenerateKey(user.UserName, user.Email);
            RavenUserEmailConfirmation confirmation = await DocumentSession.LoadAsync<RavenUserEmailConfirmation>(keyToLookFor).ConfigureAwait(false);

            return confirmation != null;
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.Email = email;
            return Task.FromResult<int>(0);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if(string.IsNullOrEmpty(user.Email) == true)
            {
                throw new InvalidOperationException("Cannot set the E-mail as confirmed because the 'Email' property on the 'user' parameter is null.");
            }

            RavenUserEmailConfirmation confirmation = new RavenUserEmailConfirmation(user.UserName, user.Email);
            return DocumentSession.StoreAsync(confirmation);
        }
    }
}