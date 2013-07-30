using AspNet.Identity.RavenDB.Indexes;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;
using System.Reflection;
using System.Web.Mvc;

namespace AspNet.Identity.RavenDB.Sample.Mvc
{
    public static class AutofacMvc
    {
        public static void Initialize()
        {
            var builder = new ContainerBuilder();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(RegisterServices(builder)));
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.Register(c =>
            {
                const string DefaultDatabase = "RavenDBIdentitySample";
                IDocumentStore store = new DocumentStore
                {
                    Url = "http://localhost:8080",
                    DefaultDatabase = DefaultDatabase
                }.Initialize();

                store.DatabaseCommands.EnsureDatabaseExists(DefaultDatabase);
                IndexCreation.CreateIndexes(typeof(RavenUser_Roles).Assembly, store);

                return store;

            }).As<IDocumentStore>().SingleInstance();

            builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession()).As<IAsyncDocumentSession>().InstancePerHttpRequest();
            builder.RegisterType<RavenIdentityStoreContext>().As<IIdentityStoreContext>().InstancePerHttpRequest();

            return builder.Build();
        }
    }
}