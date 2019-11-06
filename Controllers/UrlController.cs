using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // POST: api/Url
        [HttpPost]
        public async Task<ActionResult<string>> Shorten(string url)
        {
                  
            return "https://url.acbocc.us/" + await Task.Run(() =>
            {

                //test to see if string is correct url
                

                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri) || null == uri)
                    return "invalid url (e.g. http://alachuacounty.us/Pages/AlachuaCounty)";

                url = uri.Host + uri.PathAndQuery + uri.Fragment;

                using UrlShorten _urlShorten = new UrlShorten(url);
                    return _urlShorten.ShortenedUrl;
         
            });

        }

        [HttpPost("{id}")]
        public async Task<ActionResult<int>> Decode(string id)
        {

            return await Task.Run(() => {

                using (UrlShorten _urlShorten = new UrlShorten())
                {
                    var code = _urlShorten.Decode(id);

                    //Save the code here for shortened URL into one of the UrlView ents

                    //obviously, this part needs to be planned out more...
                    
                    return code;
                }

            });            
        }

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
