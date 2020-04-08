using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNZBlog.Data;
using MyNZBlog.Models;
using MyNZBlog.Models.ViewModel;

namespace MyNZBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyNZBlogContext _context;

        public HomeController(ILogger<HomeController> logger, MyNZBlogContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? pageNumber, int? pageSize, string tagSection = "")
        {
            ViewData["TagSection"] = tagSection;
            if (pageNumber == null || pageNumber < 0)
            {
                pageNumber = 1;
            }

            if (pageSize == null || pageSize < 0)
            {
                pageSize = 5;
            }

            var listArticles = await _context.Articles
                .OrderByDescending(a => a.ReleaseDate)
                .ToListAsync();

            List<Article> removedArticles = new List<Article>();
            foreach (var article in listArticles)
            {
                var articlesHasTags = await _context.ArticleHasTags.Where(a => a.ArticleId == article.Id).ToListAsync();
                article.ArticleHasTags = articlesHasTags;
                if (tagSection == "job" && articlesHasTags.Count == 0)
                {
                    removedArticles.Add(article);
                }
                foreach (var item in article.ArticleHasTags)
                {
                    var tag = await _context.ContentTags.FirstOrDefaultAsync(a => a.Id == item.ContentTagId);
                    item.ContentTag = tag;
                    if (tagSection == "job" && item.ContentTag.Tag != "IT career")
                    {
                        removedArticles.Add(article);
                    }
                    else if (tagSection == "no-job" && item.ContentTag.Tag == "IT career")
                    {
                        removedArticles.Add(article);
                    }
                }
            }

            foreach (var article in removedArticles)
            {
                listArticles.Remove(article);
            }

            int size = listArticles.Count;
            listArticles = listArticles.AsEnumerable().Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value).ToList();
            ArticleViewModel articleViewModel = new ArticleViewModel()
            {
                articles = listArticles,
                pageNumber = pageNumber.Value,
                pageSize = pageSize.Value,
                size = size,

            };
            
            return View(articleViewModel);
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Id,Title,ReleaseDate,Content")] Article article)
        {
            if (ModelState.IsValid)
            {
                List<ContentTag> contentTags = await _context.ContentTags.ToListAsync();

                foreach (var item in contentTags)
                {
                    string isChecked = Request.Form[item.Tag];
                    if (isChecked != "false")
                    {
                        article.ArticleHasTags.Add(new ArticleHasTag()
                        {
                            Article = article,
                            ArticleId = article.Id,
                            ContentTag = item,
                            ContentTagId = item.Id
                        });
                    }
                }

                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }
        */
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var article = await _context.Articles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            var articlesHasTags = await _context.ArticleHasTags.Where(a => a.ArticleId == id).ToListAsync();
            article.ArticleHasTags = articlesHasTags;
            foreach (var item in article.ArticleHasTags)
            {
                var tag = await _context.ContentTags.FirstOrDefaultAsync(a => a.Id == item.ContentTagId);
                item.ContentTag = tag;
            }

            return View(article);
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
