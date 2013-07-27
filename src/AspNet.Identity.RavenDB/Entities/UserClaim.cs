using Microsoft.AspNet.Identity;
using System;

namespace AspNet.Identity.RavenDB.Entities
{
    public class UserClaim : IUserClaim
    {
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
