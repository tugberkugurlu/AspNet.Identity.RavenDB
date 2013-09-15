using System.Security.Claims;

namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUserClaim
    {
        public RavenUserClaim()
        {
        }

        public RavenUserClaim(Claim claim)
        {
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}