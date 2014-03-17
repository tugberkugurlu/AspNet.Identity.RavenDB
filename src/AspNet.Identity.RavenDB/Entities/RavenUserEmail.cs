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

        public RavenUserEmail(string email)
        {
            Id = GenerateKey(email);
            Email = email;
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }

        public RavenUserEmailConfirmation ConfirmationRecord { get; set; }

        internal static string GenerateKey(string email)
        {
            return string.Format(Constants.RavenUserEmailKeyTemplate, email);
        }
    }
}