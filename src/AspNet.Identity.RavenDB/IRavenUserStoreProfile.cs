
namespace AspNet.Identity.RavenDB
{
    public interface IRavenUserStoreProfile
    {
        bool EnsurePhoneNumberUniqueness { get; }
    }
}
