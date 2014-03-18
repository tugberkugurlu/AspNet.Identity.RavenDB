
namespace AspNet.Identity.RavenDB
{
    public interface IRavenUserStoreProfile
    {
        bool EnsureEmailUniqueness { get; }
        bool EnsurePhoneNumberUniqueness { get; }
    }
}
