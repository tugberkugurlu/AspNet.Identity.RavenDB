using System;
using System.Security.Claims;
using Raven.Imports.Newtonsoft.Json;

namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUserClaim
    {
        public RavenUserClaim(Claim claim)
        {
            if (claim == null) throw new ArgumentNullException("claim");

            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }

        [JsonConstructor]
        public RavenUserClaim(string claimType, string claimValue)
        {
            if (claimType == null) throw new ArgumentNullException("claimType");
            if (claimValue == null) throw new ArgumentNullException("claimValue");

            ClaimType = claimType;
            ClaimValue = claimValue;
        }

        public string ClaimType { get; private set; }
        public string ClaimValue { get; private set; }
    }
}