using AspNet.Identity.RavenDB.Indexes;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    public abstract class TestBase
    {
        internal protected IDocumentStore CreateEmbeddableStore()
        {
            IDocumentStore store = new EmbeddableDocumentStore
            {
                Configuration =
                {
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    RunInMemory = true,
                }
            }.Initialize();
            IndexCreation.CreateIndexes(typeof(RavenUser_Roles).Assembly, store);

            return store;
        }
    }
}
