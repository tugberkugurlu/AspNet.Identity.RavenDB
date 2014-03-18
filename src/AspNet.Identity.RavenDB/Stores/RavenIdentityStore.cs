using AspNet.Identity.RavenDB.Entities;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Stores
{
    public abstract class RavenIdentityStore<TUser> : IDisposable where TUser : RavenUser
    {
        private readonly bool _disposeDocumentSession;
        protected readonly bool EnsurePhoneNumberUniqueness;
        protected readonly IAsyncDocumentSession DocumentSession;

        public RavenIdentityStore(IAsyncDocumentSession documentSession, IRavenUserStoreProfile userStoreProfile, bool disposeDocumentSession)
        {
            if (documentSession == null) throw new ArgumentNullException("documentSession");
            if (userStoreProfile == null) throw new ArgumentNullException("userStoreProfile");

            DocumentSession = documentSession;
            EnsurePhoneNumberUniqueness = userStoreProfile.EnsurePhoneNumberUniqueness;
            _disposeDocumentSession = disposeDocumentSession;
        }

        protected Task<TUser> GetUser(string id)
        {
            return DocumentSession.LoadAsync<TUser>(id);
        }

        protected async Task<TUser> GetUserByUserName(string userName)
        {
            IEnumerable<TUser> users = await DocumentSession.Query<TUser>()
                .Where(user => user.UserName == userName)
                .Take(1)
                .ToListAsync()
                .ConfigureAwait(false);

            return users.FirstOrDefault();
        }

        protected void Dispose(bool disposing)
        {
            if (_disposeDocumentSession && disposing && DocumentSession != null)
            {
                DocumentSession.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}