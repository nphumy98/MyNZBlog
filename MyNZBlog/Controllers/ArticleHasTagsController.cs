using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyNZBlog.Data;
using MyNZBlog.Models;

namespace MyNZBlog.Controllers
{
    public class ArticleHasTagsController : Controller
    {
        private readonly MyNZBlogContext _context;

        public ArticleHasTagsController(MyNZBlogContext context)
        {
            _context = context;
        }

        // GET: ArticleHasTags
        public async Task<IActionResult> Index()
        {
            var myNZBlogContext = _context.ArticleHasTags.Include(a => a.Article).Include(a => a.ContentTag);
            return View(await myNZBlogContext.ToListAsync());
        }

        // GET: ArticleHasTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleHasTag = await _context.ArticleHasTags
                .Include(a => a.Article)
                .Include(a => a.ContentTag)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (articleHasTag == null)
            {
                return NotFound();
            }

            return View(articleHasTag);
        }

        // GET: ArticleHasTags/Create
        public IActionResult Create()
        {
            ViewData["ArticleId"] = new SelectList(_context.Articles, "Id", "Id");
            ViewData["ContentTagId"] = new SelectList(_context.ContentTags, "Id", "Id");
            return View();
        }

        // POST: ArticleHasTags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,ContentTagId")] ArticleHasTag articleHasTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(articleHasTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleId"] = new SelectList(_context.Articles, "Id", "Id", articleHasTag.ArticleId);
            ViewData["ContentTagId"] = new SelectList(_context.ContentTags, "Id", "Id", articleHasTag.ContentTagId);
            return View(articleHasTag);
        }

        // GET: ArticleHasTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleHasTag = await _context.ArticleHasTags.FindAsync(id);
            if (articleHasTag == null)
            {
                return NotFound();
            }
            ViewData["ArticleId"] = new SelectList(_context.Articles, "Id", "Id", articleHasTag.ArticleId);
            ViewData["ContentTagId"] = new SelectList(_context.ContentTags, "Id", "Id", articleHasTag.ContentTagId);
            return View(articleHasTag);
        }

        // POST: ArticleHasTags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,ContentTagId")] ArticleHasTag articleHasTag)
        {
            if (id != articleHasTag.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(articleHasTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleHasTagExists(articleHasTag.ArticleId))
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
            ViewData["ArticleId"] = new SelectList(_context.Articles, "Id", "Id", articleHasTag.ArticleId);
            ViewData["ContentTagId"] = new SelectList(_context.ContentTags, "Id", "Id", articleHasTag.ContentTagId);
            return View(articleHasTag);
        }

        // GET: ArticleHasTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleHasTag = await _context.ArticleHasTags
                .Include(a => a.Article)
                .Include(a => a.ContentTag)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (articleHasTag == null)
            {
                return NotFound();
            }

            return View(articleHasTag);
        }

        // POST: ArticleHasTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var articleHasTag = await _context.ArticleHasTags.FindAsync(id);
            _context.ArticleHasTags.Remove(articleHasTag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleHasTagExists(int id)
        {
            return _context.ArticleHasTags.Any(e => e.ArticleId == id);
        }
    }
}
