using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNet.Identity.RavenDB.Sample.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            var req = HttpContext;
            return View();
        }
    }
}