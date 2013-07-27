using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB
{
    public class RavenIdentityStoreContext : IIdentityStoreContext
    {
        private readonly IAsyncDocumentSession _documentSession;

        public RavenIdentityStoreContext(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public IUserStore Users
        {
            get 
            {
                return new RavenUserStore<RavenUser>(_documentSession);
            }
        }

        public IRoleStore Roles
        {
            get
            {
                return new RavenRoleStore<RavenUser, Role>(_documentSession);
            }
        }

        public IUserSecretStore Secrets
        {
            get
            {
                return new RavenUserSecretStore<RavenUser, UserSecret>(_documentSession);
            }
        }

        public IUserClaimStore UserClaims
        {
            get
            {
                return new RavenUserClaimStore<RavenUser, UserClaim>(_documentSession);
            }
        }

        public IUserLoginStore Logins
        {
            get
            {
                return new RavenUserLoginStore<RavenUser, UserLogin>(_documentSession);
            }
        }

        public Task SaveChanges()
        {
            return _documentSession.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
