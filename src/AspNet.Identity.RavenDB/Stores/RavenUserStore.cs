using AspNet.Identity.RavenDB.Entities;
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
        IUserTwoFactorStore<TUser, string>,
        IUserLockoutStore<TUser, string>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
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

            if (user.UserName == null)
            {
                throw new InvalidOperationException("Cannot create user as the 'UserName' property is null on user parameter.");
            }

            await DocumentSession.StoreAsync(user).ConfigureAwait(false);
            await DocumentSession.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            return GetUser(userId);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            return GetUserByUserName(userName);
        }

        /// <remarks>
        /// This method assumes that incomming TUser parameter is tracked in the session. So, this method literally behaves as SaveChangeAsync
        /// </remarks>
        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

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
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            IEnumerable<TUser> users = await DocumentSession.Query<TUser>()
                .Where(usr => usr.Logins.Any(lgn => lgn.LoginProvider == login.LoginProvider && lgn.ProviderKey == login.ProviderKey))
                .Take(1)
                .ToListAsync()
                .ConfigureAwait(false);

            return users.FirstOrDefault();
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");

            user.Logins.Add(new RavenUserLogin(login));
            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");

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
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<Claim>>(user.Claims.Select(clm => new Claim(clm.ClaimType, clm.ClaimValue)).ToList());
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");

            user.Claims.Add(new RavenUserClaim(claim));
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");

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
            return Task.FromResult(0);
        }

        // IUserEmailStore

        public async Task<TUser> FindByEmailAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }

            string keyToLookFor = RavenUserEmail.GenerateKey(email);
            RavenUserEmail ravenUserEmail = await DocumentSession
                .Include<RavenUserEmail, TUser>(usrEmail => usrEmail.UserId)
                .LoadAsync(keyToLookFor)
                .ConfigureAwait(false);

            return (ravenUserEmail != null)
                ? await DocumentSession.LoadAsync<TUser>(ravenUserEmail.UserId).ConfigureAwait(false)
                : default(TUser);
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

            if (user.Email == null)
            {
                throw new InvalidOperationException("Cannot get the confirmation status of the e-mail because user doesn't have an e-mail.");
            }

            RavenUserEmailConfirmation confirmation = await GetUserEmailConfirmationAsync(user.Email)
                .ConfigureAwait(false);

            return confirmation != null;
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (email == null) throw new ArgumentNullException("email");

            user.Email = email;
            RavenUserEmail ravenUserEmail = new RavenUserEmail(email)
            {
                UserId = user.Id
            };

            return DocumentSession.StoreAsync(ravenUserEmail);
        }

        public async Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.Email == null)
            {
                throw new InvalidOperationException("Cannot set the confirmation status of the e-mail because user doesn't have an e-mail.");
            }

            RavenUserEmail userEmail = await GetUserEmailAsync(user.Email).ConfigureAwait(false);
            if (userEmail == null)
            {
                throw new InvalidOperationException("Cannot set the confirmation status of the e-mail because user doesn't have an e-mail as RavenUserEmail document.");
            }

            if (confirmed)
            {
                userEmail.ConfirmationRecord = new RavenUserEmailConfirmation
                {
                    ConfirmedOn = DateTimeOffset.UtcNow
                };
            }
            else
            {
                userEmail.ConfirmationRecord = null;
            }
        }

        // IUserPhoneNumberStore

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PhoneNumber);
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.PhoneNumber == null)
            {
                throw new InvalidOperationException("Cannot get the confirmation status of the phone number because user doesn't have a phone number.");
            }

            RavenUserPhoneNumberConfirmation confirmation = await GetUserPhoneNumberConfirmationAsync(user.PhoneNumber)
                .ConfigureAwait(false);

            return confirmation != null;
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (phoneNumber == null) throw new ArgumentNullException("phoneNumber");

            user.PhoneNumber = phoneNumber;
            RavenUserPhoneNumber ravenUserPhoneNumber = new RavenUserPhoneNumber(phoneNumber)
            {
                UserId = user.Id
            };

            return DocumentSession.StoreAsync(ravenUserPhoneNumber);
        }

        public async Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.PhoneNumber == null)
            {
                throw new InvalidOperationException("Cannot set the confirmation status of the phone number because user doesn't have a phone number.");
            }

            RavenUserPhoneNumber userPhoneNumber = await GetUserPhoneNumberAsync(user.Email).ConfigureAwait(false);
            if (userPhoneNumber == null)
            {
                throw new InvalidOperationException("Cannot set the confirmation status of the phone number because user doesn't have a phone number as RavenUserPhoneNumber document.");
            }

            if (confirmed)
            {
                userPhoneNumber.ConfirmationRecord = new RavenUserPhoneNumberConfirmation
                {
                    ConfirmedOn = DateTimeOffset.UtcNow
                };
            }
            else
            {
                userPhoneNumber.ConfirmationRecord = null;
            }
        }

        // IUserLockoutStore

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (user.LockoutEndDate == null) throw new InvalidOperationException("LockoutEndDate has no value.");

            return Task.FromResult(user.LockoutEndDate.Value);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEndDate = lockoutEnd;
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            // NOTE: Not confortable to do this like below but this will work out for the intended scenario.
            user.AccessFailedCount++;
            return Task.FromResult(0);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.IsLockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.IsLockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        // privates

        private Task<RavenUserEmail> GetUserEmailAsync(string email)
        {
            string keyToLookFor = RavenUserEmail.GenerateKey(email);
            return DocumentSession.LoadAsync<RavenUserEmail>(keyToLookFor);
        }

        private Task<RavenUserPhoneNumber> GetUserPhoneNumberAsync(string phoneNumber)
        {
            string keyToLookFor = RavenUserPhoneNumber.GenerateKey(phoneNumber);
            return DocumentSession.LoadAsync<RavenUserPhoneNumber>(keyToLookFor);
        }

        private async Task<RavenUserEmailConfirmation> GetUserEmailConfirmationAsync(string email)
        {
            RavenUserEmail userEmail = await GetUserEmailAsync(email).ConfigureAwait(false);

            return (userEmail != null)
                ? userEmail.ConfirmationRecord
                : null;
        }

        private async Task<RavenUserPhoneNumberConfirmation> GetUserPhoneNumberConfirmationAsync(string phoneNumber)
        {
            RavenUserPhoneNumber userPhoneNumber = await GetUserPhoneNumberAsync(phoneNumber).ConfigureAwait(false);

            return (userPhoneNumber != null)
                ? userPhoneNumber.ConfirmationRecord
                : null;
        }
    }
}