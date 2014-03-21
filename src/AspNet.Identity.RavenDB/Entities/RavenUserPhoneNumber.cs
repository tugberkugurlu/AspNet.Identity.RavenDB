using System;

namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUserPhoneNumber
    {
        public RavenUserPhoneNumber(string phoneNumber, string userId)
        {
            if (phoneNumber == null) throw new ArgumentNullException("phoneNumber");

            Id = GenerateKey(phoneNumber);
            UserId = userId;
            PhoneNumber = phoneNumber;
        }

        public string Id { get; private set; }
        public string UserId { get; private set; }
        public string PhoneNumber { get; private set; }

        public ConfirmationRecord ConfirmationRecord { get; private set; }

        public void SetConfirmed()
        {
            if (ConfirmationRecord == null)
            {
                ConfirmationRecord = new ConfirmationRecord();
            }
        }

        public void SetUnconfirmed()
        {
            ConfirmationRecord = null;
        }

        internal static string GenerateKey(string phoneNumber)
        {
            return string.Format(Constants.RavenUserPhoneNumberKeyTemplate, phoneNumber);
        }
    }
}