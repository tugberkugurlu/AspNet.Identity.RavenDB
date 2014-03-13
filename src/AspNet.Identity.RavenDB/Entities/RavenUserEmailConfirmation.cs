using System;

namespace AspNet.Identity.RavenDB.Entities.Entities
{
    /// <summary>
    /// As we need to make the e-mail confirmation happen only once, this should be 
    /// in a seperate document with a distinct identifier which is RavenUsers/{userName}/{email}
    /// </summary>
    public class RavenUserEmailConfirmation
    {
        const string KeyTemplate = "RavenUserEmailConfirmations/{0}/{1}";

        public RavenUserEmailConfirmation()
        {
        }

        public RavenUserEmailConfirmation(string userName, string email)
        {
            Id = string.Format(KeyTemplate, userName, email);
        }

        public string Id { get; set; }
        public DateTimeOffset ConfirmedOn { get; set; }
    }
}
