using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUser : User
    {
        public RavenUser()
        {
            Claims = new Collection<RavenUserClaim>();
            Logins = new Collection<RavenUserLogin>();
            Roles = new Collection<string>();
        }

        public ICollection<RavenUserClaim> Claims { get; set; }
        public ICollection<RavenUserLogin> Logins { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}