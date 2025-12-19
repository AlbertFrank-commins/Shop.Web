using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shop.Contracts.Identity;
using Shop.Web.Services.Identity;
using Shop.Web.ViewModels.Identity;

namespace Shop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _auth;

        public AccountController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View(vm);

            var result = await _auth.LoginAsync(new LoginRequest
            {
                Email = vm.Email,
                Password = vm.Password
            });

            if (!result.Success)
            {
                vm.Error = result.Error ?? "No se pudo iniciar sesión.";
                return View(vm);
            }

           
            var user = result.Value!;

            
            const string CLAIM_NAME_ID = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            const string CLAIM_NAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
            const string CLAIM_EMAIL = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
            const string CLAIM_ROLE = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

            
            string userId = user.Id.ToString();
            // si tu DTO es Guid Id, esto funciona
            string name = user.FullName ?? user.FullName ?? user.Email ?? "User";
            string email = user.Email ?? "";
            string role = user.Role ?? "USER";

            var claims = new System.Security.Claims.Claim[]
            {
                new System.Security.Claims.Claim(CLAIM_NAME_ID, userId),
                new System.Security.Claims.Claim(CLAIM_NAME, name),
                new System.Security.Claims.Claim(CLAIM_EMAIL, email),
                new System.Security.Claims.Claim(CLAIM_ROLE, role),
            };

            var identity = new System.Security.Claims.ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new System.Security.Claims.ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

