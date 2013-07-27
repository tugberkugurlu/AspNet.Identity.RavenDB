using AspNet.Identity.RavenDB.Entities;
using System.Collections.Generic;

namespace AspNet.Identity.RavenDB
{
    public class RavenUser : User
    {
        public UserSecret Secret { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<UserClaim> UserClaims { get; set; }
        public ICollection<UserLogin> UserLogins { get; set; }
    }
}
