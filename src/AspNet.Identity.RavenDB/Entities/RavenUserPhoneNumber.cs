
namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUserPhoneNumber
    {
        public RavenUserPhoneNumber()
        {
        }

        public RavenUserPhoneNumber(string phoneNumber)
        {
            Id = GenerateKey(phoneNumber);
            PhoneNumber = phoneNumber;
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }

        public RavenUserPhoneNumberConfirmation ConfirmationRecord { get; set; }

        internal static string GenerateKey(string phoneNumber)
        {
            return string.Format(Constants.RavenUserPhoneNumberKeyTemplate, phoneNumber);
        }
    }
}