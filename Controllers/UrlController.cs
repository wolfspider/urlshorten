using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using urlshorten.Models;

namespace urlshorten.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly URLShortenDBContext _context;

        private readonly ILogger<UrlController> _logger;

        public UrlController(ILogger<UrlController> logger, URLShortenDBContext context)
        {
            _logger = logger;
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
        public async Task<ActionResult<string>> Shorten([FromBody] dynamic kurl)
        {
            //_logger.LogInformation("Url shortened from original " + url);

            return "https://acbo.cc/" + await Task.Run(() =>
            {

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                //Rick Grimes says- kurl! gimme that knife! we're still hurr..thAts whut matters

                //create a URL knife object
                var knife = JsonSerializer.Deserialize<UrlKnife>(kurl.ToString(), options);

                if (!Uri.TryCreate(knife.normalizedUrl, UriKind.Absolute, out Uri uri) || null == uri)
                    return "invalid url (e.g. http://alachuacounty.us/Pages/AlachuaCounty)";

                var url = (uri.Host + uri.PathAndQuery + uri.Fragment).TrimEnd('/');

                using UrlShorten _urlShorten = new UrlShorten(url);
                return _urlShorten.ShortenedUrl;

            });

        }

        [HttpPost("{id}")]
        public async Task<ActionResult<int>> Decode(string id)
        {
            return await Task.Run(() =>
            {
                using UrlShorten _urlShorten = new UrlShorten();
                return _urlShorten.Decode(id);
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
