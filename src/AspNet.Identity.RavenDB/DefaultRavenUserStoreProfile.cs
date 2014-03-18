
namespace AspNet.Identity.RavenDB
{
    public class DefaultRavenUserStoreProfile : IRavenUserStoreProfile
    {
        private DefaultRavenUserStoreProfile()
        {
        }

        public bool EnsureEmailUniqueness { get { return true; } }
        public bool EnsurePhoneNumberUniqueness { get { return true; } }

        public static DefaultRavenUserStoreProfile Instance { get { return new DefaultRavenUserStoreProfile(); } }
    }
}