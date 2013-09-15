using Microsoft.AspNet.Identity;

namespace AspNet.Identity.RavenDB.Entities
{
    public class RavenUserLogin
    {
        public RavenUserLogin()
        {
        }

        public RavenUserLogin(UserLoginInfo loginInfo)
        {
            LoginProvider = loginInfo.LoginProvider;
            ProviderKey = loginInfo.ProviderKey;
        }

        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}