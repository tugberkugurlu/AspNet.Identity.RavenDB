using System;

namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUserEmailConfirmation
    {
        public RavenUserEmailConfirmation()
        {
            ConfirmedOn = DateTimeOffset.UtcNow;
        }

        public DateTimeOffset ConfirmedOn { get; private set; }
    }
}