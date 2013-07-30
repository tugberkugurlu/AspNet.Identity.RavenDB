using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNet.Identity.RavenDB.Sample.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityStoreContext _identityStoreContext;

        public AccountController(IIdentityStoreContext identityStoreContext)
        {
            _identityStoreContext = identityStoreContext;
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}