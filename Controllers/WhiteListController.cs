using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using urlshorten;
using urlshorten.Models;

namespace urlshorten.Controllers
{
    [Authorize]
    public class WhiteListController : Controller
    {
        private readonly URLShortenDBContext _context;

        public WhiteListController(URLShortenDBContext context)
        {
            _context = context;
        }

        // GET: WhiteList
        public async Task<IActionResult> Index()
        {
            return View(await _context.WhiteListModel.ToListAsync());
        }

        // GET: WhiteList/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var whiteListModel = await _context.WhiteListModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (whiteListModel == null)
            {
                return NotFound();
            }

            return View(whiteListModel);
        }

        // GET: WhiteList/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WhiteList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Source,Url")] WhiteListModel whiteListModel)
        {
            if (ModelState.IsValid)
            {
                whiteListModel.Modified = DateTime.UtcNow;
                _context.Add(whiteListModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(whiteListModel);
        }

        // GET: WhiteList/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var whiteListModel = await _context.WhiteListModel.FindAsync(id);
            if (whiteListModel == null)
            {
                return NotFound();
            }
            return View(whiteListModel);
        }

        // POST: WhiteList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Source,Url")] WhiteListModel whiteListModel)
        {
            if (id != whiteListModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    whiteListModel.Modified = DateTime.UtcNow;
                    
                    _context.Update(whiteListModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WhiteListModelExists(whiteListModel.Id))
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
            return View(whiteListModel);
        }

        // GET: WhiteList/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var whiteListModel = await _context.WhiteListModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (whiteListModel == null)
            {
                return NotFound();
            }

            return View(whiteListModel);
        }

        // POST: WhiteList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var whiteListModel = await _context.WhiteListModel.FindAsync(id);
            _context.WhiteListModel.Remove(whiteListModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WhiteListModelExists(int id)
        {
            return _context.WhiteListModel.Any(e => e.Id == id);
        }
    }
}
