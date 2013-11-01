using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AspNet.Identity.RavenDB.Sample.Mvc.Startup))]
namespace AspNet.Identity.RavenDB.Sample.Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
