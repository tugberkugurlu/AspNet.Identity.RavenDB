using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace AspNet.Identity.RavenDB.Sample.Mvc
{
    public static class IdentityConfig
    {
        public const string LocalLoginProvider = "Local";
        public const string RoleClaimType = ClaimsIdentity.DefaultRoleClaimType;
        public const string ClaimsIssuer = ClaimsIdentity.DefaultIssuer;
        public const string UserIdClaimType = "http://schemas.microsoft.com/aspnet/userid";
        public const string UserNameClaimType = "http://schemas.microsoft.com/aspnet/username";

        internal static IList<Claim> RemoveUserIdentityClaims(IEnumerable<Claim> claims)
        {
            return claims.Where(c => c.Type != ClaimTypes.Name && c.Type != ClaimTypes.NameIdentifier).ToList();
        }

        internal static void AddUserIdentityClaims(string userId, string displayName, IList<Claim> claims)
        {
            claims.Add(new Claim(ClaimTypes.Name, displayName, ClaimsIssuer));
            claims.Add(new Claim(UserIdClaimType, userId, ClaimsIssuer));
            claims.Add(new Claim(UserNameClaimType, displayName, ClaimsIssuer));
        }

        internal static void AddRoleClaims(IEnumerable<string> roles, IList<Claim> claims)
        {
            foreach (string role in roles)
            {
                claims.Add(new Claim(RoleClaimType, role, ClaimsIssuer));
            }
        }
    }
}