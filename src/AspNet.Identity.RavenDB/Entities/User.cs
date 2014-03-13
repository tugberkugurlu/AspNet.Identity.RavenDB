using Microsoft.AspNet.Identity;

namespace AspNet.Identity.RavenDB.Entities
{
    public abstract class User : IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
    }
}