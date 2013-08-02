using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUser : User
    {
        public RavenUser()
        {
            Roles = new Collection<Role>();
            UserClaims = new Collection<UserClaim>();
            UserLogins = new Collection<UserLogin>();
        }

        public ICollection<Role> Roles { get; set; }
        public ICollection<UserClaim> UserClaims { get; set; }
        public ICollection<UserLogin> UserLogins { get; set; }
    }
}
