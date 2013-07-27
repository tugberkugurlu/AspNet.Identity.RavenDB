using AspNet.Identity.RavenDB.Entities;
using Raven.Client.Indexes;
using System.Linq;

namespace AspNet.Identity.RavenDB.Indexes
{
    public class RavenUser_Roles : AbstractIndexCreationTask<RavenUser, RavenUser_Roles.ReduceResult>
    {
        public class ReduceResult
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }

        public RavenUser_Roles()
        {
            Map = users => users.SelectMany(usr => usr.Roles).Select(role => new { Name = role, Count = 1 });

            Reduce = roles => from role in roles
                              group role by role.Name
                              into groupedRole
                              select new 
                              {
                                  Name = groupedRole.Key,
                                  Count = groupedRole.Sum(role => role.Count)
                              };
        }
    }
}
