using Owin;

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