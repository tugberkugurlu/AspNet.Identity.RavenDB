AspNet.Identity.RavenDB
================

Fully asynchronous, new and sweet ASP.NET Identity implementation for RavenDB.

## Getting Started with Version 2.0.0
Using ASP.NET Identity RavenDB port is pretty straight forward. You can install the [AspNet.Identity.RavenDB](https://www.nuget.org/packages/AspNet.Identity.RavenDB) library through [NuGet](https://nuget.org). For now, the **AspNet.Identity.RavenDB** package which 
targets the ASP.NET Identity 2.0.0 release is pre-release. So, be sure to use the `-pre` switch while getting it through NuGet:

    Install-Package AspNet.Identity.RavenDB -Pre

The following code snippet shows the easiest way to stand up the `UserManager<TUser>` class with `RavenUserStore<TUser>`:

    IDocumentStore documentStore = new DocumentStore
    {
        Url = "http://localhost:8080",
        DefaultDatabase = "AspNetIdentity"
    }.Initialize();

    using (IAsyncDocumentSession session = documentStore.OpenAsyncSession())
    {
        session.Advanced.UseOptimisticConcurrency = true;
        RavenUserStore<RavenUser> ravenUserStore = new RavenUserStore<RavenUser>(session);
        UserManager<RavenUser> userManager = new UserManager<RavenUser>(ravenUserStore);

        // UserManager<RavenUser> is ready to use!
    }

Couple of things to note here:

 - You MUST set the `UseOptimisticConcurrency` flag to `true` on the `IAsyncDocumentSession` as shown above 
and leave it enabled at the end of the `IAsyncDocumentSession` lifetime because we need to ensure the uniqueness 
of the username and the email. The library check if you enabled optimistic concurrency or not and if you didn't, 
it will throw an exception. However, optimistic concurrency can be disabled any time during the `IAsyncDocumentSession` 
lifetime. That's why the library cannot possibly be sure to warn 100% of the time. So, it is extremely important to 
obey this rule. Otherwise, you will have a chance of ending up overriding the existing users data if a new user tries 
to register with the username of an existing user.

 - You don't need to use `RavenUser` entity type. However, your custom entity class must be derived from `RavenUser` class.

## Resources for Version 1.0.0
* [Introduction Blog Post: AspNet.Identity.RavenDB: Fully asynchronous, new and sweet ASP.NET Identity implementation for RavenDB](http://www.tugberkugurlu.com/archive/aspnet-identity-ravendb--fully-asynchronous-new-and-sweet-asp-net-identity-implementation-for-ravendb)