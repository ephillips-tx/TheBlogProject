using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TheBlogProject.Models;
using TheBlogProject.Services;
using TheBlogProject.ViewModels;

namespace TheBlogProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, IBlogEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            ViewData["HeaderContent"] = "Mental Expressions by Eric Phillips";
            ViewData["HeaderSubContent"] = "Peeling away labels to identify the nature of things.";

            return View();
        }

        public IActionResult About()
        {
            ViewData["HeaderContent"] = "About the creator";
            ViewData["HeaderSubContent"] = "Not THE CREATOR, just the one for this site.";

            return View();
        }
        // GET: 
        public IActionResult Contact()
        {
            ViewData["HeaderContent"] = "Want to send me a message?";
            ViewData["HeaderSubContent"] = "Fill out the form below and I'll get back with you ASAP.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe model)
        {
            //This is where we will be emailing...
            model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
            await _emailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject, model.Message);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}