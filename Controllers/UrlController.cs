using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using urlshorten;
using urlshorten.Models;

namespace urlshorten.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly URLShortenDBContext _context;

        public UrlController(URLShortenDBContext context)
        {
            _context = context;
        }

        // GET: api/Url
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UrlViewModel>>> GetUrlViewModels()
        {
            return await _context.UrlViewModels.ToListAsync();
        }

        // GET: api/Url/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UrlViewModel>> GetUrlViewModel(int id)
        {
            var urlViewModel = await _context.UrlViewModels.FindAsync(id);

            if (urlViewModel == null)
            {
                return NotFound();
            }

            return urlViewModel;
        }

        // PUT: api/Url/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUrlViewModel(int id, UrlViewModel urlViewModel)
        {
            if (id != urlViewModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(urlViewModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UrlViewModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<string>> Shorten(string url)
        {
            return "https://ac-url/" + await Task.Run(() =>
            {
                using UrlShorten _urlShorten = new UrlShorten(url);
                return _urlShorten.ShortenedUrl;
         
            }); 
        }


        // POST: api/Url
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPost]
        //public async Task<ActionResult<UrlViewModel>> PostUrlViewModel(UrlViewModel urlViewModel)
        //{

        //    _context.UrlViewModels.Add(urlViewModel);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUrlViewModel", new { id = urlViewModel.Id }, urlViewModel);
        //}

        // DELETE: api/Url/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UrlViewModel>> DeleteUrlViewModel(int id)
        {
            var urlViewModel = await _context.UrlViewModels.FindAsync(id);
            if (urlViewModel == null)
            {
                return NotFound();
            }

            _context.UrlViewModels.Remove(urlViewModel);
            await _context.SaveChangesAsync();

            return urlViewModel;
        }

        private bool UrlViewModelExists(int id)
        {
            return _context.UrlViewModels.Any(e => e.Id == id);
        }
    }
}
