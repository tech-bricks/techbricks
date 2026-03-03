using Microsoft.AspNetCore.Mvc;
using TechBricks.Helper;
using TechBricks.Models;

namespace TechBricks.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailSender _emailSender;

        // Inject the service via the constructor
        public HomeController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [Route("home")]
        public IActionResult Index()
        {
            // MVC looks for a view at: /Views/Home/Index.cshtml

            ViewData["Title"] = "Home page";
            return View();
        }

        [Route("360tour")]
        public IActionResult VirtualTours()
        {
            // MVC looks for a view at: /Views/Home/About.cshtml
            return View();
        }

        // POST: Handles the form submission
        [HttpPost]
        [Route("home/contact")]
        public async Task<IActionResult> Contact(ContactFormModel model)
        {
            if (ModelState.IsValid)
            {
                // Prepare the HTML content for the email body
                string message = $"<p><strong>Name:</strong> {model.Name}</p>" +
                                 $"<p><strong>Email:</strong> {model.Email}</p>" +
                                 $"<p><strong>Subject:</strong> {model.Subject}</p>" +
                                 $"<hr><p><strong>Message:</strong></p><p>{model.Message.Replace("\n", "<br>")}</p>";

                string subject = $"New Inquiry: {model.Subject} from {model.Name}";
                string recipientEmail = "info@tech-bricks.com";

                try
                {
                    await _emailSender.SendEmailAsync(recipientEmail, subject, message);

                    ViewBag.MessageSent = true;
                    ModelState.Clear();
                    return View(new ContactFormModel());
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "We encountered an issue sending your message. Please try again later.");
                    return View(model);
                }
            }

            // Process your email/database logic here...
            return Ok(new { message = "Success" });
        }
    }
}
