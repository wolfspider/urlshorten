﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using urlshorten.Models;

namespace urlshorten.Controllers
{
    public class HomeController : Controller
    {
        private readonly URLShortenDBContext _context;

        private readonly ILogger<HomeController> _logger;

        private readonly IUrlCache<string> _cache;
        
        public HomeController(ILogger<HomeController> logger, URLShortenDBContext context, IUrlCache<string> cache)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
        }

        // GET: Index/aBcDeF
        [HttpGet("{urlhash}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<IActionResult> Index(string urlhash)
        {
            //match the URL hash with an index
            var url = await _cache.GetOrCreate(urlhash, _context);

            if (url == null)
            {
                return NotFound();
            }

            return Redirect(url);
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
