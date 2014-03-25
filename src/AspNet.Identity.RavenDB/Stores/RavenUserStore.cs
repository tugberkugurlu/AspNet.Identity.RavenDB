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
    public class RavenUserStore<TUser> : IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser, string>,
        IUserLockoutStore<TUser, string>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IDisposable where TUser : RavenUser
    {
        private readonly bool _disposeDocumentSession;
        private readonly IAsyncDocumentSession _documentSession;

        public RavenUserStore(IAsyncDocumentSession documentSession)
            : this(documentSession, true)
        {
        }

        public RavenUserStore(IAsyncDocumentSession documentSession, bool disposeDocumentSession)
        {
            if (documentSession == null) throw new ArgumentNullException("documentSession");

            _documentSession = documentSession;
            _disposeDocumentSession = disposeDocumentSession;
        }

        // IQueryableUserStore

        public IQueryable<TUser> Users
        {
            get
            {
                return _documentSession.Query<TUser>();
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

            await _documentSession.StoreAsync(user).ConfigureAwait(false);
            await _documentSession.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            if (userId == null) throw new ArgumentNullException("userId");

            return _documentSession.LoadAsync<TUser>(userId);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");

            return _documentSession.LoadAsync<TUser>(RavenUser.GenerateKey(userName));
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

            return _documentSession.SaveChangesAsync();
        }

        public Task DeleteAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _documentSession.Delete<TUser>(user);
            return _documentSession.SaveChangesAsync();
        }

        // IUserLoginStore

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            return Task.FromResult<IList<UserLoginInfo>>(
                user.Logins.Select(login => 
                    new UserLoginInfo(login.LoginProvider, login.ProviderKey)).ToList());
        }

        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null) throw new ArgumentNullException("login");

            string keyToLookFor = RavenUserLogin.GenerateKey(login.LoginProvider, login.ProviderKey);
            RavenUserLogin ravenUserLogin = await _documentSession
                .Include<RavenUserLogin, TUser>(usrLogin => usrLogin.UserId)
                .LoadAsync(keyToLookFor)
                .ConfigureAwait(false);

            return (ravenUserLogin != null)
                ? await _documentSession.LoadAsync<TUser>(ravenUserLogin.UserId).ConfigureAwait(false)
                : default(TUser);
        }

        public async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");

            RavenUserLogin ravenUserLogin = new RavenUserLogin(user.Id, login);
            await _documentSession.StoreAsync(ravenUserLogin).ConfigureAwait(false);
            user.Logins.Add(ravenUserLogin);
        }

        public async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");

            string keyToLookFor = RavenUserLogin.GenerateKey(login.LoginProvider, login.ProviderKey);
            RavenUserLogin ravenUserLogin = await _documentSession.LoadAsync<RavenUserLogin>(keyToLookFor).ConfigureAwait(false);
            if (ravenUserLogin != null)
            {
                _documentSession.Delete(ravenUserLogin);
            }

            RavenUserLogin userLogin = user.Logins.FirstOrDefault(lgn => lgn.Id.Equals(keyToLookFor, StringComparison.InvariantCultureIgnoreCase));
            if (userLogin != null)
            {
                user.Logins.Remove(userLogin);
            }
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
            if (user == null) throw new ArgumentNullException("user");

            if (enabled) 
            {
                user.EnableTwoFactorAuthentication();
            }
            else
            {
                user.DisableTwoFactorAuthentication();
            }

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
            RavenUserEmail ravenUserEmail = await _documentSession
                .Include<RavenUserEmail, TUser>(usrEmail => usrEmail.UserId)
                .LoadAsync(keyToLookFor)
                .ConfigureAwait(false);

            return (ravenUserEmail != null)
                ? await _documentSession.LoadAsync<TUser>(ravenUserEmail.UserId).ConfigureAwait(false)
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

            ConfirmationRecord confirmation = await GetUserEmailConfirmationAsync(user.Email)
                .ConfigureAwait(false);

            return confirmation != null;
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (email == null) throw new ArgumentNullException("email");

            user.SetEmail(email);
            RavenUserEmail ravenUserEmail = new RavenUserEmail(email, user.Id);

            return _documentSession.StoreAsync(ravenUserEmail);
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
                userEmail.SetConfirmed();
            }
            else
            {
                userEmail.SetUnconfirmed();
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

            ConfirmationRecord confirmation = await GetUserPhoneNumberConfirmationAsync(user.PhoneNumber)
                .ConfigureAwait(false);

            return confirmation != null;
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (phoneNumber == null) throw new ArgumentNullException("phoneNumber");

            user.PhoneNumber = phoneNumber;
            RavenUserPhoneNumber ravenUserPhoneNumber = new RavenUserPhoneNumber(phoneNumber, user.Id);

            return _documentSession.StoreAsync(ravenUserPhoneNumber);
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
                userPhoneNumber.SetConfirmed();
            }
            else
            {
                userPhoneNumber.SetUnconfirmed();
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

        // Dispose

        protected void Dispose(bool disposing)
        {
            if (_disposeDocumentSession && disposing && _documentSession != null)
            {
                _documentSession.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // privates

        private Task<RavenUserEmail> GetUserEmailAsync(string email)
        {
            string keyToLookFor = RavenUserEmail.GenerateKey(email);
            return _documentSession.LoadAsync<RavenUserEmail>(keyToLookFor);
        }

        private Task<RavenUserPhoneNumber> GetUserPhoneNumberAsync(string phoneNumber)
        {
            string keyToLookFor = RavenUserPhoneNumber.GenerateKey(phoneNumber);
            return _documentSession.LoadAsync<RavenUserPhoneNumber>(keyToLookFor);
        }

        private async Task<ConfirmationRecord> GetUserEmailConfirmationAsync(string email)
        {
            RavenUserEmail userEmail = await GetUserEmailAsync(email).ConfigureAwait(false);

            return (userEmail != null)
                ? userEmail.ConfirmationRecord
                : null;
        }

        private async Task<ConfirmationRecord> GetUserPhoneNumberConfirmationAsync(string phoneNumber)
        {
            RavenUserPhoneNumber userPhoneNumber = await GetUserPhoneNumberAsync(phoneNumber).ConfigureAwait(false);

            return (userPhoneNumber != null)
                ? userPhoneNumber.ConfirmationRecord
                : null;
        }
    }
}