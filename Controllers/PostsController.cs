﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Models;
using TheBlogProject.ViewModels;
using TheBlogProject.Services;
using TheBlogProject.Enums;
using X.PagedList;

namespace TheBlogProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly BlogSearchService _blogSearchService;

        public PostsController(ApplicationDbContext context,
            ISlugService slugService,
            IImageService imageService,
            UserManager<BlogUser> userManager, 
            BlogSearchService blogSearchService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
        }

        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;

            var pageNumber = page ?? 1;
            var pageSize = 5;

            var posts = _blogSearchService.Search(searchTerm);    
            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Blog).Include(p => p.BlogUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BlogPostIndex
        public async Task<IActionResult> BlogPostIndex(int? id, int? page)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pageNumber = page ?? 1; // null coalescing operator
            var pageSize = 5;

            //var posts = _context.Posts.Where(p => p.BlogId == id).ToList();
            var posts = await _context.Posts
                         .Where(p => p.BlogId == id && p.ReadyStatus == ReadyStatus.ProductionReady)
                         .OrderByDescending(p => p.Created)
                         .ToPagedListAsync(pageNumber, pageSize);

            ViewData["HeaderImage"] = "/img/header-bg-2.jpg";
            ViewData["Title"] = "Blog Post Index";
            ViewData["HeaderContent"] = "Index of Posts";
            ViewData["HeaderSubContent"] = "This page shows a list of posts associated with this blog.";

            return View(posts);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var post = await _context.Posts
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null) return NotFound();

            var dataVM = new PostDetailViewModel()
            {
                Post = post,
                Tags = _context.Tags
                        .Select(t => t.Text.ToLower())
                        .Distinct().ToList()
            };
            
            ViewData["Title"] = "Post Detail Page";
            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData, post.ContentType);
            ViewData["HeaderContent"] = post.Title;
            ViewData["HeaderSubContent"] = post.Abstract;

            return View(dataVM);
        }



        //public async Task<IActionResult> Details(string slug)
        //{
        //    if (string.IsNullOrEmpty(slug))
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts
        //        .Include(p => p.Blog)
        //        .Include(p => p.BlogUser)
        //        .Include(p => p.Tags)
        //        .Include(p => p.Comments)
        //        .ThenInclude(c => c.BlogUser)
        //        .FirstOrDefaultAsync(m => m.Slug == slug);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(post);
        //}

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name"); // third parameter was "description" by default. 
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.UtcNow;

                var authorId = _userManager.GetUserId(User);
                post.BlogUserId = authorId;

                // Use the _imageService to store incoming user specified image
                post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                post.ContentType = _imageService.ContentType(post.Image);

                // Create the slug & determine if it is unique
                var slug = _slugService.urlFriendly(post.Title);

                // Create a variable to store whether error has occurred. 
                var validationError = false;

                if(string.IsNullOrEmpty(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("", "The Title you provided cannot be used as it results in an empty slug.");
                }
                // Detect incoming duplicate Slugs
                else if (!_slugService.IsUnique(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("Title", "The Title you provided cannot be used as it results in a duplicate slug.");
                }

                //else if(slug.Contains("test"))
                //{
                //    validationError = true;
                //    ModelState.AddModelError("", "Oops, are you testing again??");
                //    ModelState.AddModelError("Title", "The Title cannot contain the word test");
                //}

                if (validationError)
                {
                    // Pass the tags back when we return to the View
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                post.Slug = slug; 

                _context.Add(post);
                await _context.SaveChangesAsync();

                // How do I loop over the incoming list of string? 
                foreach(var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tagText,
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);

            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId); // removed "Description" fix drop down list to select blog name
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text)); // Use ViewData to load <select> list
            
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus")] Post post, IFormFile newImage, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    /* 
                     * coding it like this allows us to preserve previous post data that we do not want to change accidentally. 
                     * otherwise, we could get null values replacing info in the DB
                     */

                    // The originalPost
                    var newPost = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);

                    newPost.Updated = DateTime.UtcNow;
                    newPost.Title = post.Title;
                    newPost.Abstract = post.Abstract;
                    newPost.Content = post.Content;
                    newPost.ReadyStatus = post.ReadyStatus;

                    var newSlug = _slugService.urlFriendly(post.Title);
                    if(newSlug != newPost.Slug) // compare new slug and old slug
                    {
                        if(_slugService.IsUnique(newSlug))
                        {
                            newPost.Title = post.Title;
                            newPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "This title cannot be used as it results in a duplicate slug.");
                            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text)); // Use ViewData to load <select> list
                            return View(post);
                        }
                    }

                    if (newImage is not null)
                    {
                        newPost.ImageData = await _imageService.EncodeImageAsync(newImage);
                        newPost.ContentType = _imageService.ContentType(newImage);
                    }

                    // Remove all tags previously associated with this Post
                    _context.Tags.RemoveRange(newPost.Tags);

                    // Add in new tags
                    foreach(var tagText in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = newPost.BlogUserId,
                            Text = tagText
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);

            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
