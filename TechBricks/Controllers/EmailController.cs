using Microsoft.AspNetCore.Mvc;
using TechBricks.Helper;
using TechBricks.Models;
using System.Text;
using ExcelDataReader;
using System.Data;
using System.IO;
using Microsoft.Extensions.Logging;
using TechBricks.Background; // new
using System.Threading;     // new

namespace TechBricks.Controllers
{
    public class EmailController : Controller
    {
        private readonly IBulkEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EmailController> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;

        public EmailController(
            IBulkEmailSender emailSender,
            IWebHostEnvironment env,
            ILogger<EmailController> logger,
            IBackgroundTaskQueue taskQueue)
        {
            _emailSender = emailSender;
            _env = env;
            _logger = logger;
            _taskQueue = taskQueue;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> Index(IFormFile uploadFile)
        {
            if (uploadFile == null || uploadFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select a CSV or Excel file to upload.");
                return View();
            }

            try
            {
                string subject = "20 years of expertise, now at your service via Tech Bricks";
                string bodyHtml = string.Empty;
                var recipients = new List<EmailRecipient>();

                var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var htmlTemplatePath = Path.Combine(wwwroot, "files", "htmlcontent.html");

                if (System.IO.File.Exists(htmlTemplatePath))
                {
                    bodyHtml = await System.IO.File.ReadAllTextAsync(htmlTemplatePath, Encoding.UTF8);
                }
                else
                {
                    ModelState.AddModelError("", "Email HTML template not found at wwwroot/files/htmlcontent.html. Add the file and try again.");
                    return View();
                }

                var ext = Path.GetExtension(uploadFile.FileName).ToLowerInvariant();

                if (ext == ".csv")
                {
                    using var sr = new StreamReader(uploadFile.OpenReadStream(), Encoding.UTF8);
                    string? headerLine = await sr.ReadLineAsync();
                    if (string.IsNullOrEmpty(headerLine))
                    {
                        ModelState.AddModelError("", "CSV appears empty or missing headers.");
                        return View();
                    }

                    var headers = headerLine.Split(',').Select(h => h.Trim().Trim('"')).ToArray();
                    int emailIdx = Array.FindIndex(headers, h => h.Equals("email", StringComparison.OrdinalIgnoreCase));
                    int nameIdx = Array.FindIndex(headers, h => h.Equals("name", StringComparison.OrdinalIgnoreCase));
                    if (nameIdx < 0)
                        nameIdx = Array.FindIndex(headers, h => h.Equals("firstname", StringComparison.OrdinalIgnoreCase) || h.Equals("fullname", StringComparison.OrdinalIgnoreCase));

                    string? line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var cols = line.Split(',').Select(c => c.Trim().Trim('"')).ToArray();
                        if (emailIdx >= 0 && emailIdx < cols.Length)
                        {
                            var email = cols[emailIdx];
                            var name = (nameIdx >= 0 && nameIdx < cols.Length) ? cols[nameIdx] : "";
                            if (!string.IsNullOrWhiteSpace(email))
                                recipients.Add(new EmailRecipient { Email = email, Name = name });
                        }
                    }
                }
                else if (ext == ".xls" || ext == ".xlsx")
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using var stream = uploadFile.OpenReadStream();
                    using var reader = ExcelReaderFactory.CreateReader(stream);

                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };
                    var result = reader.AsDataSet(conf);

                    var table = result.Tables[0];
                    var headerRow = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName?.Trim() ?? "").ToArray();
                    int emailIdx = Array.FindIndex(headerRow, h => h.Equals("email", StringComparison.OrdinalIgnoreCase));
                    int nameIdx = Array.FindIndex(headerRow, h => h.Equals("name", StringComparison.OrdinalIgnoreCase));
                    if (nameIdx < 0)
                        nameIdx = Array.FindIndex(headerRow, h => h.Equals("firstname", StringComparison.OrdinalIgnoreCase) || h.Equals("fullname", StringComparison.OrdinalIgnoreCase));

                    foreach (System.Data.DataRow dr in table.Rows)
                    {
                        var email = emailIdx >= 0 && emailIdx < table.Columns.Count ? dr[emailIdx]?.ToString()?.Trim() ?? "" : "";
                        var name = nameIdx >= 0 && nameIdx < table.Columns.Count ? dr[nameIdx]?.ToString()?.Trim() ?? "" : "";
                        if (!string.IsNullOrWhiteSpace(email))
                            recipients.Add(new EmailRecipient { Email = email, Name = name });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Unsupported file type. Please upload a .csv, .xls or .xlsx file.");
                    return View();
                }

                if (recipients.Count == 0)
                {
                    ModelState.AddModelError("", "No recipient rows were found in the uploaded file.");
                    return View();
                }

                var profilePdfPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "files", "CompanyProfile.pdf");
                if (!System.IO.File.Exists(profilePdfPath))
                {
                    ModelState.AddModelError("", "Company profile PDF not found at wwwroot/files/CompanyProfile.pdf. Add the file and try again.");
                    return View();
                }

                // ENQUEUE work and return immediately
                await _taskQueue.QueueBackgroundWorkItem(async ct =>
                {
                    try
                    {
                        _logger.LogInformation("Background job started: sending {Count} emails (file: {FileName})", recipients.Count, uploadFile.FileName);
                        // call your existing sender (it returns Task<int> or Task depending on your signature)
                        // if your SendBulkZohoEmailsAsync returns int, capture it; adjust as needed
                        await _emailSender.SendBulkZohoEmailsAsync(recipients, subject ?? "Company Update", bodyHtml ?? string.Empty, profilePdfPath);
                        _logger.LogInformation("Background job finished: emails processed for {FileName}", uploadFile.FileName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Background job error while sending emails for {FileName}", uploadFile.FileName);
                    }
                });

                ViewBag.Message = $"Email job queued for processing ({recipients.Count} recipients). You will receive confirmation in server logs.";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bulk email upload (file: {FileName})", uploadFile?.FileName);
                ModelState.AddModelError("", "An unexpected error occurred while processing your request. Check server logs for details.");
                return View();
            }
        }
    }
}