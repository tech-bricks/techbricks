using Microsoft.AspNetCore.Mvc;
using TechBricks.Helper;

namespace TechBricks.Controllers
{
    public class EmailController : Controller
    {
        private readonly IBulkEmailSender _emailSender;

        // Inject the service via the constructor
        public EmailController(IBulkEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
