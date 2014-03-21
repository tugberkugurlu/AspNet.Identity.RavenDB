using System;

namespace AspNet.Identity.RavenDB.Entities
{
    public class ConfirmationRecord
    {
        public ConfirmationRecord()
        {
            ConfirmedOn = DateTimeOffset.UtcNow;
        }

        public DateTimeOffset ConfirmedOn { get; private set; }
    }
}