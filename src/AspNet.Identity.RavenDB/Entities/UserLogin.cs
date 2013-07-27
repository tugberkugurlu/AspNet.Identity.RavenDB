using Microsoft.AspNet.Identity;

namespace AspNet.Identity.RavenDB.Entities
{
    public class UserLogin : IUserLogin
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}
