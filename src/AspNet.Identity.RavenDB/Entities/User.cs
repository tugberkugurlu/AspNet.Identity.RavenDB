using Microsoft.AspNet.Identity;

namespace AspNet.Identity.RavenDB.Entities
{
    public class User : IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
