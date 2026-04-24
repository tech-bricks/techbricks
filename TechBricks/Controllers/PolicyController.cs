using Microsoft.AspNetCore.Mvc;

namespace TechBricks.Controllers
{
    public class PolicyController : Controller
    {
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        public IActionResult TermsOfUse()
        {
            return View();
        }

        public IActionResult CookiePolicy()
        {
            return View();
        }
    }
}