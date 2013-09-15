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
        private readonly UserManager<RavenUser> _userManager;

        public AccountController(UserManager<RavenUser> userManager)
        {
            _userManager = userManager;
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
                RavenUserLogin login = new RavenUserLogin { LoginProvider = IdentityConfig.LocalLoginProvider, ProviderKey = requestModel.UserName };
                user.Logins.Add(login);

                IdentityResult result = await _userManager.CreateAsync(user, requestModel.Password);

                if (result.Succeeded)
                {
                    await InternalSignIn(user.Id, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (string item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item);
                    }
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
            RavenUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Replace UserIdentity claims with the application specific claims
                IList<Claim> userClaims = IdentityConfig.RemoveUserIdentityClaims(claims);
                IdentityConfig.AddUserIdentityClaims(userId, user.UserName, userClaims);
                // IdentityConfig.AddRoleClaims(await _userManager.Roles.GetRolesForUser(userId), userClaims);
                SignIn("Application", userClaims, ClaimTypes.Name, IdentityConfig.RoleClaimType, isPersistent);
            }
        }
	}
}