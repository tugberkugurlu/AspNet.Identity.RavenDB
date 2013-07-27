using Microsoft.AspNet.Identity;

namespace AspNet.Identity.RavenDB.Entities
{
    public class UserSecret : IUserSecret
    {
        public string UserName { get; set; }
        public string Secret { get; set; }
    }
}
