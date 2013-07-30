using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace AspNet.Identity.RavenDB.Sample.Mvc.Controllers
{
    public abstract class OwinController : Controller
    {
        private const string OwinEnvironmentKey = "owin.Environment";

        protected OwinRequest OwinRequest { get { return GetOwinRequest(HttpContext); } }
        protected OwinResponse OwinResponse { get { return GetOwinResponse(HttpContext); } }

        protected virtual void SignIn(ClaimsPrincipal principal, AuthenticationExtra extra)
        {
            OwinResponse.Grant(principal, extra);
        }

        protected virtual void SignIn(string authenticationType, IEnumerable<Claim> claims, string nameClaimType, string roleClaimType, bool isPersistent)
        {
            if (authenticationType == null)
            {
                throw new ArgumentNullException("authenticationType");
            }
            if (claims == null)
            {
                throw new ArgumentNullException("claims");
            }
            if (nameClaimType == null)
            {
                throw new ArgumentNullException("nameClaimType");
            }
            if (roleClaimType == null)
            {
                throw new ArgumentNullException("roleClaimType");
            }

            AuthenticationExtra extra = new AuthenticationExtra()
            {
                IsPersistent = isPersistent
            };

            SignIn(new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType, nameClaimType, roleClaimType)), extra);
        }

        // private helpers

        private static IDictionary<string, object> GetOwinEnvironment(HttpContextBase httpContext)
        {
            return (IDictionary<string, object>)httpContext.Items[(object)OwinEnvironmentKey];
        }

        private static OwinRequest GetOwinRequest(HttpContextBase httpContext)
        {
            IDictionary<string, object> owinEnvironment = GetOwinEnvironment(httpContext);
            if (owinEnvironment == null)
            {
                throw new InvalidOperationException("Could not found the OWIN environment");
            }

            return new OwinRequest(owinEnvironment);
        }

        private static OwinResponse GetOwinResponse(HttpContextBase httpContext)
        {
            IDictionary<string, object> owinEnvironment = GetOwinEnvironment(httpContext);
            if (owinEnvironment == null)
            {
                throw new InvalidOperationException("Could not found the OWIN environment");
            }

            return new OwinResponse(owinEnvironment);
        }
    }
}