using Raven.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace AspNet.Identity.RavenDB.Stores
{
    public abstract class RavenIdentityStore<TUser> where TUser : RavenUser
    {
        protected readonly IAsyncDocumentSession DocumentSession;

        internal protected RavenIdentityStore(IAsyncDocumentSession documentSession)
        {
            if (documentSession == null)
            {
                throw new ArgumentNullException("documentSession");
            }

            DocumentSession = documentSession;
        }

        internal protected async Task<TUser> GetUserById(string userId)
        {
            return await DocumentSession.LoadAsync<TUser>(userId).ConfigureAwait(false);
        }

        internal protected async Task<TUser> GetUserByUserName(string userName)
        {
            IEnumerable<TUser> users = await DocumentSession.Query<TUser>()
                .Where(user => user.UserName == userName)
                .Take(1)
                .ToListAsync()
                .ConfigureAwait(false);

            return users.FirstOrDefault();
        }
    }
}
