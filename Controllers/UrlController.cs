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

        public class UrlKnife
        {
            public string url {get; set;}
            public string normalizedUrl {get; set;}
            public string removedTailOnUrl {get; set;}
            public string protocol {get; set;}
            public string onlyDomain {get; set;}
            public string onlyParams {get; set;}
            public string onlyUri {get; set;}
            public string onlyUriWithParams {get; set;}
            public Dictionary<string, string> onlyParamsJsn {get; set;}
            public string type {get; set;}
            public string port {get; set;}

        }

        class UrlCollection
        {

        }

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
                  
            return "https://url.acbocc.us/" + await Task.Run(() =>
            {

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
       
                //create a URL knife object
                var knife = JsonSerializer.Deserialize<UrlKnife>(kurl.ToString(), options);
                
                if (!Uri.TryCreate(knife.normalizedUrl, UriKind.Absolute, out Uri uri) || null == uri)
                    return "invalid url (e.g. http://alachuacounty.us/Pages/AlachuaCounty)";

                //TODO: fragment at end causes forward slash at end must be removed
                                
                var url = uri.Host + uri.PathAndQuery + uri.Fragment;

                using UrlShorten _urlShorten = new UrlShorten(url);
                return _urlShorten.ShortenedUrl;
         
            });

        }

        [HttpPost("{id}")]
        public async Task<ActionResult<int>> Decode(string id)
        {
            //TODO: need to parse all the slashes in the url which means this needs to be parameter!

            return await Task.Run(() => {

                using UrlShorten _urlShorten = new UrlShorten();
                return _urlShorten.Decode(id);

                //Save the code here for shortened URL into one of the UrlView ents

                //obviously, this part needs to be planned out more...

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
