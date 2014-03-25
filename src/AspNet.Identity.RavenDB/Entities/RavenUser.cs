using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNet.Identity;
using Raven.Imports.Newtonsoft.Json;

namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUser : IUser
    {
        [JsonConstructor]
        public RavenUser(string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");

            Id = GenerateKey(userName);
            UserName = userName;
            Claims = new Collection<RavenUserClaim>();
            Logins = new Collection<RavenUserLogin>();
        }

        public RavenUser(string userName, string email) : this(userName)
        {
            Email = email;
        }

        public string Id { get; private set; }
        public string UserName { get; set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsLockoutEnabled { get; set; }
        public bool IsTwoFactorEnabled { get; private set; }

        public int AccessFailedCount { get; set; }
        public DateTimeOffset? LockoutEndDate { get; set; }

        public ICollection<RavenUserClaim> Claims { get; private set; }
        public ICollection<RavenUserLogin> Logins { get; private set; }

        public virtual void EnableTwoFactorAuthentication()
        {
            IsTwoFactorEnabled = true;
        }

        public virtual void DisableTwoFactorAuthentication()
        {
            IsTwoFactorEnabled = false;
        }

        public virtual void SetEmail(string email)
        {
            Email = email;
        }

        // statics

        internal static string GenerateKey(string userName)
        {
            return string.Format(Constants.RavenUserKeyTemplate, userName);
        }
    }
}