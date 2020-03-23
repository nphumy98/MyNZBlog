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
    public class ContentTagsController : Controller
    {
        private readonly MyNZBlogContext _context;

        public ContentTagsController(MyNZBlogContext context)
        {
            _context = context;
        }

        // GET: ContentTags
        public async Task<IActionResult> Index()
        {
            return View(await _context.ContentTags.ToListAsync());
        }

        // GET: ContentTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentTag = await _context.ContentTags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contentTag == null)
            {
                return NotFound();
            }

            return View(contentTag);
        }

        // GET: ContentTags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ContentTags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tag")] ContentTag contentTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contentTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contentTag);
        }

        // GET: ContentTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentTag = await _context.ContentTags.FindAsync(id);
            if (contentTag == null)
            {
                return NotFound();
            }
            return View(contentTag);
        }

        // POST: ContentTags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tag")] ContentTag contentTag)
        {
            if (id != contentTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contentTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentTagExists(contentTag.Id))
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
            return View(contentTag);
        }

        // GET: ContentTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentTag = await _context.ContentTags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contentTag == null)
            {
                return NotFound();
            }

            return View(contentTag);
        }

        // POST: ContentTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contentTag = await _context.ContentTags.FindAsync(id);
            _context.ContentTags.Remove(contentTag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContentTagExists(int id)
        {
            return _context.ContentTags.Any(e => e.Id == id);
        }
    }
}
