using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Raven.Imports.Newtonsoft.Json;

namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUser : IUser
    {
        private List<RavenUserClaim> _claims;
        private List<RavenUserLogin> _logins;

        [JsonConstructor]
        public RavenUser(string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");

            Id = GenerateKey(userName);
            UserName = userName;
            _claims = new List<RavenUserClaim>();
            _logins = new List<RavenUserLogin>();
        }

        public RavenUser(string userName, string email) : this(userName)
        {
            Email = email;
        }

        public string Id { get; private set; }
        public string UserName { get; set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string PasswordHash { get; private set; }
        public string SecurityStamp { get; private set; }
        public bool IsLockoutEnabled { get; private set; }
        public bool IsTwoFactorEnabled { get; private set; }

        public int AccessFailedCount { get; private set; }
        public DateTimeOffset? LockoutEndDate { get; private set; }

        public IEnumerable<RavenUserClaim> Claims
        {
            get
            {
                return _claims;
            }

            private set
            {
                if (_claims == null)
                {
                    _claims = new List<RavenUserClaim>();
                }

                _claims.AddRange(value);
            }
        }
        public IEnumerable<RavenUserLogin> Logins
        {
            get
            {
                return _logins;
            }

            private set
            {
                if (_logins == null)
                {
                    _logins = new List<RavenUserLogin>();
                }

                _logins.AddRange(value);
            }
        }

        public virtual void EnableTwoFactorAuthentication()
        {
            IsTwoFactorEnabled = true;
        }

        public virtual void DisableTwoFactorAuthentication()
        {
            IsTwoFactorEnabled = false;
        }

        public virtual void EnableLockout()
        {
            IsLockoutEnabled = true;
        }

        public virtual void DisableLockout()
        {
            IsLockoutEnabled = false;
        }

        public virtual void SetEmail(string email)
        {
            Email = email;
        }

        public virtual void SetPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public virtual void SetPasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public virtual void SetSecurityStamp(string securityStamp)
        {
            SecurityStamp = securityStamp;
        }

        public virtual void IncrementAccessFailedCount()
        {
            AccessFailedCount++;
        }

        public virtual void ResetAccessFailedCount()
        {
            AccessFailedCount = 0;
        }

        public virtual void LockUntil(DateTimeOffset lockoutEndDate)
        {
            LockoutEndDate = lockoutEndDate;
        }

        public virtual void AddClaim(Claim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            AddClaim(new RavenUserClaim(claim));
        }

        public virtual void AddClaim(RavenUserClaim ravenUserClaim)
        {
            if (ravenUserClaim == null)
            {
                throw new ArgumentNullException("ravenUserClaim");
            }

            _claims.Add(ravenUserClaim);
        }

        public virtual void RemoveClaim(RavenUserClaim ravenUserClaim)
        {
            if (ravenUserClaim == null)
            {
                throw new ArgumentNullException("ravenUserClaim");
            }

            _claims.Remove(ravenUserClaim);
        }

        public virtual void AddLogin(RavenUserLogin ravenUserLogin)
        {
            if (ravenUserLogin == null)
            {
                throw new ArgumentNullException("ravenUserLogin");
            }

            _logins.Add(ravenUserLogin);
        }

        public virtual void RemoveLogin(RavenUserLogin ravenUserLogin)
        {
            if (ravenUserLogin == null)
            {
                throw new ArgumentNullException("ravenUserLogin");
            }

            _logins.Remove(ravenUserLogin);
        }

        // statics

        internal static string GenerateKey(string userName)
        {
            return string.Format(Constants.RavenUserKeyTemplate, userName);
        }
    }
}