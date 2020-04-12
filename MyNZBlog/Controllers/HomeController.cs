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

        public async Task<IActionResult> Index(int? pageNumber, int? pageSize, string tagSection = "", int month = 0,
            int year = 0, string searchString="")
        {
            ViewData["TagSection"] = tagSection;
            ViewData["SearchString"] = searchString;
            ViewData["Month"] = month;
            ViewData["Year"] = year;
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

            Dictionary<string, int> dates = new Dictionary<string, int>();
            List<Article> removedArticles = new List<Article>();
            foreach (var article in listArticles)
            {
                if (!dates.ContainsKey(article.ReleaseDate.Year.ToString() + article.ReleaseDate.Month))
                {
                    dates.Add(article.ReleaseDate.Year.ToString() + article.ReleaseDate.Month, article.ReleaseDate.Month);
                }
                if (month >= 0 &&
                    month <= 12 &&
                    year <= listArticles[0].ReleaseDate.Year &&
                    year >= listArticles[listArticles.Count - 1].ReleaseDate.Year)
                {
                    if (article.ReleaseDate.Month != month || article.ReleaseDate.Year != year)
                    {
                        removedArticles.Add(article);
                        continue;
                    }
                }
                var articlesHasTags = await _context.ArticleHasTags.Where(a => a.ArticleId == article.Id).ToListAsync();
                article.ArticleHasTags = articlesHasTags;
                
               
                if (tagSection == "job" && articlesHasTags.Count == 0)
                {
                    removedArticles.Add(article);
                    continue;
                }

                bool isJobKeep = false;
                foreach (var item in article.ArticleHasTags)
                {
                    var tag = await _context.ContentTags.FirstOrDefaultAsync(a => a.Id == item.ContentTagId);
                    item.ContentTag = tag;
                    if (tagSection == "job" && item.ContentTag.Tag == "IT career")
                    {
                        isJobKeep = true;
                    }
                    else if (tagSection == "no-job" && item.ContentTag.Tag == "IT career")
                    {
                        removedArticles.Add(article);
                    }
                }

                if (tagSection == "job" && !isJobKeep)
                {
                    removedArticles.Add(article);
                }
            }

            foreach (var article in removedArticles)
            {
                listArticles.Remove(article);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                listArticles = listArticles.Where(a => a.Title.Contains(searchString)).ToList();
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
                monthYear = dates
            };
            
            return View(articleViewModel);
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string searchString)
        {
            ArticleViewModel articleViewModel = new ArticleViewModel();
            if (ModelState.IsValid)
            {
                var articles = from m in _context.Articles
                               select m;

                if (!String.IsNullOrEmpty(searchString))
                {
                    articles = articles.Where(s => s.Title.Contains(searchString));
                }

                var listArticles = await articles.ToListAsync();

                articleViewModel = new ArticleViewModel()
                {
                    articles = listArticles,
                };
                
                var articles = await _context.Articles
                    .OrderByDescending(a => a.ReleaseDate)
                    .ToListAsync();

                if (!String.IsNullOrEmpty(searchString))
                {
                    articles = articles.Where(a => a.Title.Contains(searchString) = true);
                }
                
    }
            return View(articleViewModel);
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
