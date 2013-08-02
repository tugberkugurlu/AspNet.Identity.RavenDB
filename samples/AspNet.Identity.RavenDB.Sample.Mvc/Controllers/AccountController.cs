using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Sample.Mvc.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AspNet.Identity.RavenDB.Sample.Mvc.Controllers
{
    public class AccountController : OwinController
    {
        private readonly IIdentityStoreContext _identityStoreContext;

        public AccountController(IIdentityStoreContext identityStoreContext)
        {
            _identityStoreContext = identityStoreContext;
        }

        [HttpGet]
        public ViewResult Register()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Register")]
        public async Task<ActionResult> RegisterPost(RegisterRequestModel requestModel)
        {
            if (ModelState.IsValid)
            {
                // Create a profile, password, and link the local login before signing in the user
                RavenUser user = new RavenUser { UserName = requestModel.UserName };
                UserSecret userSecret = new UserSecret { UserName = requestModel.UserName, Secret = requestModel.Password };
                bool userStoreResult = await _identityStoreContext.Users.Create(user);
                bool userSecretStoreResult = await _identityStoreContext.Secrets.Create(userSecret);

                UserLogin login = new UserLogin { UserId = user.Id, LoginProvider = IdentityConfig.LocalLoginProvider, ProviderKey = requestModel.UserName };
                bool userLoginStoreResult = await _identityStoreContext.Logins.Add(login);

                if (userStoreResult && userSecretStoreResult && userLoginStoreResult)
                {
                    await _identityStoreContext.SaveChanges();
                    await InternalSignIn(user.Id, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Failed to create login for: " + requestModel.UserName);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(requestModel);
        }

        // privates
        private Task InternalSignIn(string userId, bool isPersistent)
        {
            return InternalSignIn(userId, new Claim[0], isPersistent);
        }

        private async Task InternalSignIn(string userId, IEnumerable<Claim> claims, bool isPersistent)
        {
            User user = await _identityStoreContext.Users.Find(userId) as User;
            if (user != null)
            {
                // Replace UserIdentity claims with the application specific claims
                IList<Claim> userClaims = IdentityConfig.RemoveUserIdentityClaims(claims);
                IdentityConfig.AddUserIdentityClaims(userId, user.UserName, userClaims);
                IdentityConfig.AddRoleClaims(await _identityStoreContext.Roles.GetRolesForUser(userId), userClaims);
                SignIn("Application", userClaims, ClaimTypes.Name, IdentityConfig.RoleClaimType, isPersistent);
            }
        }
	}
}