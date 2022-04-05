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
            ViewData["HeaderImage"] = "/img/header-bg-2.jpg";
            ViewData["HeaderContent"] = "Mental Expressions by Eric Phillips";
            ViewData["HeaderSubContent"] = "Peeling away labels to identify the nature of things.";

            var pageNumber = page ?? 1;
            var pageSize = 5;

            //var blogs = _context.Blogs.Where(
            //    b => b.Posts.Any(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady))
            //    .OrderByDescending(b => b.Created)
            //    .ToPagedListAsync(pageNumber, pageSize);

            var blogs = _context.Blogs
                .Include(b => b.BlogUser)
                .OrderByDescending(b => b.Created)
                .ToPagedListAsync(pageNumber, pageSize);

            // Previous Basic implementation without pagedList:
            // var blogs = await _context.Blogs
            //    .Include(b => b.BlogUser)
            //    .ToListAsync();

            // return View(blogs);

            return View(await blogs);
        }

        public IActionResult About()
        {
            ViewData["HeaderImage"] = "/img/header-bg-1.jpg";
            ViewData["HeaderContent"] = "About the creator";
            ViewData["HeaderSubContent"] = "Not THE CREATOR, just the one for this site.";

            return View();
        }
        // GET: 
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}