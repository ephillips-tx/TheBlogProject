using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TheBlogProject.Data;
using TheBlogProject.Models;
using TheBlogProject.Services;
using TheBlogProject.ViewModels;
using X.PagedList;

namespace TheBlogProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, 
                              IBlogEmailSender emailSender, 
                              ApplicationDbContext context)
        {
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<IActionResult> Index(int? page)
        {
            // Previous Basic implementation without pagedList:
            // var blogs = await _context.Blogs
            //    .Include(b => b.BlogUser)
            //    .ToListAsync();

            // return View(blogs);

            var pageNumber = page ?? 1;
            var pageSize = 5;


            var blogs = _context.Blogs
                .Include(b => b.BlogUser)
                .OrderByDescending(b => b.Created)
                .ToPagedListAsync(pageNumber, pageSize);

            ViewData["HeaderImage"] = "/img/header-bg-2.png";
            ViewData["HeaderContent"] = "Mental Expressions by Eric Phillips";
            ViewData["HeaderSubContent"] = "Peeling away labels to identify the nature of things.";
            ViewData["Title"] = "Home Page";

            return View(await blogs);
        }

        public async Task<IActionResult> About()
        {
            var allTags = _context.Tags
                        .Select(t => t.Text.ToLower())
                        .Distinct();

            ViewData["HeaderImage"] = "/img/header-bg-1.jpg";
            ViewData["HeaderContent"] = "Hi! I'm Eric Phillips";
            ViewData["HeaderSubContent"] = "I love building things with code.";
            ViewData["Title"] = "About Me";

            return View(await allTags.ToListAsync());
        }
        // GET: Home/Contact
        public IActionResult Contact()
        {
            ViewData["HeaderImage"] = "/img/header-bg-1.jpg";
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

        // GET: Home/Privacy
        public IActionResult Privacy()
        {
            ViewData["Title"] = "Privacy Policy";
            ViewData["HeaderImage"] = "/img/header-bg-1.jpg";
            ViewData["HeaderContent"] = ViewData["Title"];
            ViewData["HeaderSubContent"] = "Please read our privacy policy below.";

            return View();
        }

        // GET: Home/Terms
        public IActionResult Terms()
        {
            ViewData["Title"] = "Terms & Conditions";
            ViewData["HeaderImage"] = "/img/header-bg-1.jpg";
            ViewData["HeaderContent"] = ViewData["Title"];
            ViewData["HeaderSubContent"] = "Please read our Terms & Conditions below.";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}