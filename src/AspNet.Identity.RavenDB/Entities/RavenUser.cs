using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNet.Identity;

namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUser : IUser
    {
        public RavenUser()
        {
            Claims = new Collection<RavenUserClaim>();
            Logins = new Collection<RavenUserLogin>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsTwoFactorEnabled { get; set; }

        public ICollection<RavenUserClaim> Claims { get; set; }
        public ICollection<RavenUserLogin> Logins { get; set; }
    }
}