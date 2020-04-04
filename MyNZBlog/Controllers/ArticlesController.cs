using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyNZBlog.Data;
using MyNZBlog.Models;
using MyNZBlog.Models.ViewModel;

namespace MyNZBlog.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly MyNZBlogContext _context;

        public ArticlesController(MyNZBlogContext context)
        {
            _context = context;
        }

        // GET: Articles
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            if (pageNumber == null ||  pageNumber < 0)
            {
                pageNumber = 1;
            }

            if (pageSize == null || pageSize < 0)
            {
                pageSize = 5;
            }

            int size = _context.Articles.Count();
            var listArticles = await _context.Articles
                .OrderByDescending(a => a.ReleaseDate)
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync();

            ArticleViewModel articleViewModel = new ArticleViewModel()
            {
                articles = listArticles,
                pageNumber = pageNumber.Value,
                pageSize = pageSize.Value,
                size = size,

            };
            foreach (var article in listArticles)
            {
                var articlesHasTags = await _context.ArticleHasTags.Where(a => a.ArticleId == article.Id).ToListAsync();
                article.ArticleHasTags = articlesHasTags;
                foreach (var item in article.ArticleHasTags)
                {
                    var tag = await _context.ContentTags.FirstOrDefaultAsync(a => a.Id == item.ContentTagId);
                    item.ContentTag = tag;
                }
            }
            return View(articleViewModel);
        }

        [HttpPost]

        // GET: Articles/Details/5
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

        // GET: Articles/Create
        public async Task<IActionResult> Create()
        {
            var listTag = await _context.ContentTags.ToListAsync();
            ViewData["ListTag"] = listTag;
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Content")] Article article)
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

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            var listTag = await _context.ContentTags.ToListAsync();
            var articlesHasTags = await _context.ArticleHasTags.Where(a => a.ArticleId == id).ToListAsync();
            article.ArticleHasTags = articlesHasTags;
            foreach (var item in article.ArticleHasTags)
            {
                var tag = await _context.ContentTags.FirstOrDefaultAsync(a => a.Id == item.ContentTagId);
                listTag.Remove(tag);
                item.ContentTag = tag;
            }

            ViewData["ListTag"] = listTag;
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Content")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<ContentTag> contentTags = await _context.ContentTags.ToListAsync();

                    foreach (var item in contentTags)
                    {
                        string isChecked = Request.Form[item.Tag];
                        if (isChecked == "false")
                        {
                            ArticleHasTag removedTag = await _context.ArticleHasTags.FindAsync(article.Id, item.Id);
                            if (removedTag!=null)
                                _context.ArticleHasTags.Remove(removedTag);
                        }
                        else
                        {
                            ArticleHasTag addTag = await _context.ArticleHasTags.FindAsync(article.Id, item.Id);
                            if (addTag == null)
                            {
                                addTag = new ArticleHasTag()
                                {
                                    Article = article,
                                    ArticleId = article.Id,
                                    ContentTag = item,
                                    ContentTagId = item.Id
                                };
                                _context.ArticleHasTags.Add(addTag);
                            }
                        }
                    }
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
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
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
