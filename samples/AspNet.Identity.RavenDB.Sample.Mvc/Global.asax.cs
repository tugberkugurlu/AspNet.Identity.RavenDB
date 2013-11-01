using Autofac;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Raven.Client.Extensions;
using Raven.Client.Document;
using Autofac.Integration.Mvc;
using AspNet.Identity.RavenDB.Stores;
using AspNet.Identity.RavenDB.Sample.Mvc.Models;
using Microsoft.AspNet.Identity;
using System.Reflection;

namespace AspNet.Identity.RavenDB.Sample.Mvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            const string RavenDefaultDatabase = "Users";
            ContainerBuilder builder = new ContainerBuilder();
            builder.Register(c =>
            {
                IDocumentStore store = new DocumentStore
                {
                    Url = "http://localhost:8080",
                    DefaultDatabase = RavenDefaultDatabase
                }.Initialize();

                store.DatabaseCommands.EnsureDatabaseExists(RavenDefaultDatabase);

                return store;

            }).As<IDocumentStore>().SingleInstance();

            builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession()).As<IAsyncDocumentSession>().InstancePerHttpRequest();
            builder.Register(c => new RavenUserStore<ApplicationUser>(c.Resolve<IAsyncDocumentSession>(), false)).As<IUserStore<ApplicationUser>>().InstancePerHttpRequest();
            builder.RegisterType<UserManager<ApplicationUser>>().InstancePerHttpRequest();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }
    }
}
