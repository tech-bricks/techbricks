using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TechBricks.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;

        // Default credentials come from configuration (appsettings) or fallback to these defaults.
        private const string DefaultUser = "admin";
        private const string DefaultPassword = "P@ssword123";

        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            // read configured creds if present
            var configuredUser = _config["AdminUser:Username"] ?? DefaultUser;
            var configuredPassword = _config["AdminUser:Password"] ?? DefaultPassword;

            if (string.Equals(username?.Trim(), configuredUser, StringComparison.Ordinal) &&
                string.Equals(password, configuredPassword, StringComparison.Ordinal))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, configuredUser),
                    new Claim("Role", "Admin")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Email");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}