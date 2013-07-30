using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNet.Identity.RavenDB.Sample.Mvc
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseApplicationSignInCookie();

            // Enable the application to use a cookie to temporarily store information about a user logging in with a third party login provider
            // app.UseExternalSignInCookie();
        }
    }
}