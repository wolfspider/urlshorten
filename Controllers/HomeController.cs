using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using urlshorten.Models;

namespace urlshorten.Controllers
{
    public class HomeController : Controller
    {
        private readonly URLShortenDBContext _context;

        private readonly ILogger<HomeController> _logger;

        private readonly static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        private readonly UrlCache<string> _urlcache;

        public HomeController(ILogger<HomeController> logger, URLShortenDBContext context)
        {
            _logger = logger;
            _context = context;
            _urlcache = new UrlCache<string>(_cache, _context);
        }
   
        // GET: Index/aBcDeF
        [HttpGet("{urlhash}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public IActionResult Index(string urlhash)
        {  
            //match the URL hash with an index
            var url = _context.UrlViewModels.Where(x => x.ShortAddress == urlhash).SingleOrDefault();

            if (url == null)
            {
                return NotFound();
            }

            return Redirect(url.Address);
        }

        public IActionResult Index(bool duplicate = false, bool success = false)
        {
            
            if(duplicate is true)
                ViewBag.Duplicate = true;
            
            if(success is true)
                ViewBag.Success = true;

            return View();
        }

        public IActionResult Privacy()
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
