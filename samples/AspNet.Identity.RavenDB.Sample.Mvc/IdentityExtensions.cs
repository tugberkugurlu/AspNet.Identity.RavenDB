using System;
using System.Security.Claims;
using System.Security.Principal;

namespace AspNet.Identity.RavenDB.Sample.Mvc
{
    public static class IdentityExtensions
    {
        public static string GetUserName(this IIdentity identity)
        {
            return identity.Name;
        }

        public static string GetUserId(this IIdentity identity)
        {
            ClaimsIdentity ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                return ci.FindFirstValue(IdentityConfig.UserIdClaimType);
            }
            return String.Empty;
        }

        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            Claim claim = identity.FindFirst(claimType);
            if (claim != null)
            {
                return claim.Value;
            }
            return null;
        }
    }
}