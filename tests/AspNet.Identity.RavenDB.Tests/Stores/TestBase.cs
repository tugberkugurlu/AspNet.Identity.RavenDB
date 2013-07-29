using AspNet.Identity.RavenDB.Indexes;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Client.Listeners;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    public abstract class TestBase
    {
        internal protected IDocumentStore CreateEmbeddableStore()
        {
            EmbeddableDocumentStore store = new EmbeddableDocumentStore
            {
                Configuration =
                {
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    RunInMemory = true,
                }
            };

            store.Initialize();
            store.RegisterListener(new NoStaleQueriesListener());
            IndexCreation.CreateIndexes(typeof(RavenUser_Roles).Assembly, store);

            return store;
        }
    }

    // http://stackoverflow.com/questions/9181204/ravendb-how-to-flush
    public class NoStaleQueriesListener : IDocumentQueryListener
    {
        public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
        {
            queryCustomization.WaitForNonStaleResults();
        }
    }
}
