using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Entities
{
    /// <summary>
    /// Represents the user's e-mail address. This is stored in a seperate document as
    /// we need to ensure the uniqueness of the e-mail address.
    /// </summary>
    /// <remarks>
    /// Storing the e-mail in a seperate document to ensure it's uniqueness by the Id of the document
    /// will only work out if the provided IAsyncDocumentSession is configured for optimistic concurrency.
    /// </remarks>
    public class RavenUserEmail
    {
        public RavenUserEmail()
        {
        }

        public RavenUserEmail(string userName, string email)
        {
            Id = GenerateKey(userName, email);
        }

        public string Id { get; set; }
        public string Email { get; set; }

        internal static string GenerateKey(string userName, string email)
        {
            return string.Format(Constants.RavenUserEmailKeyTemplate, userName, email);
        }
    }
}
