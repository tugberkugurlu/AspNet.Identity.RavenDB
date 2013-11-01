using System.Web;
using System.Web.Mvc;

namespace AspNet.Identity.RavenDB.Sample.Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
