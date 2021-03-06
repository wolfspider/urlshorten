﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using urlshorten.Models;

namespace urlshorten.Controllers
{
    public class HomeController : Controller
    {
        private readonly URLShortenDBContext _context;

        private readonly ILogger<HomeController> _logger;

        private readonly IUrlCache<string> _cache;

        private readonly IConfiguration _config;
        
        public HomeController(ILogger<HomeController> logger, URLShortenDBContext context, IUrlCache<string> cache, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
            _config = config;
        }

        // GET: Index/aBcDeF
        [HttpGet("{urlhash}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<IActionResult> Index(string urlhash)
        {
            //match the URL hash with an index
            try
            {       
                var url = await _cache.GetOrCreate(urlhash, _context);

                if(url == "not found" || url == "error")
                    return NotFound();

                return Redirect(url);
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Redirect Error: "+ex.ToString());
                return NotFound();
            }
                
        }

        //TODO: Simplify this function
        //[Authorize]
        public IActionResult Index(bool duplicate = false, bool success = false, string redir = "", bool wlist = false)
        {
            //use non-standard port for debugging
            var debugPort = _config["debugport:port"];
            
            if(!String.IsNullOrEmpty(debugPort))
                ViewBag.DebugPort = debugPort;

            if(wlist is true)
                ViewBag.WList = true;
            
            if(duplicate is true)
                ViewBag.Duplicate = true;
            
            if(success is true)
                ViewBag.Success = true;
                ViewBag.Redirect = redir;

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
