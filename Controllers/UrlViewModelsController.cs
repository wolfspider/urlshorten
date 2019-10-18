using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using urlshorten.Models;

namespace urlshorten.Controllers
{
    public class UrlViewModelsController : Controller
    {
        private readonly URLShortenDBContext _context;

        public UrlViewModelsController(URLShortenDBContext context)
        {
            _context = context;
        }

        // GET: UrlViewModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.UrlViewModels.ToListAsync());
        }

        // GET: UrlViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlViewModel = await _context.UrlViewModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (urlViewModel == null)
            {
                return NotFound();
            }

            return View(urlViewModel);
        }

        // GET: UrlViewModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UrlViewModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Created,Modified,Active,UrlHash,User,Address,ShortAddress")] UrlViewModel urlViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(urlViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(urlViewModel);
        }

        // GET: UrlViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlViewModel = await _context.UrlViewModels.FindAsync(id);
            if (urlViewModel == null)
            {
                return NotFound();
            }
            return View(urlViewModel);
        }

        // POST: UrlViewModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Created,Modified,Active,UrlHash,User,Address,ShortAddress")] UrlViewModel urlViewModel)
        {
            if (id != urlViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(urlViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UrlViewModelExists(urlViewModel.Id))
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
            return View(urlViewModel);
        }

        // GET: UrlViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlViewModel = await _context.UrlViewModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (urlViewModel == null)
            {
                return NotFound();
            }

            return View(urlViewModel);
        }

        // POST: UrlViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var urlViewModel = await _context.UrlViewModels.FindAsync(id);
            _context.UrlViewModels.Remove(urlViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UrlViewModelExists(int id)
        {
            return _context.UrlViewModels.Any(e => e.Id == id);
        }
    }
}
