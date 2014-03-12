using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    public class RavenUserTwoFactorStoreFacts : TestBase
    {
        [Fact]
        public async Task GetTwoFactorEnabledAsync_Should_Get_User_IsTwoFactorEnabled_Value()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
            }
        }
    }
}
