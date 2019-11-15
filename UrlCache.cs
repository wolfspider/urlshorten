using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace urlshorten
{
    public class UrlCache
    {
        private static ConcurrentDictionary<string, string> _lookup;

        private static IMemoryCache _cache;

        private readonly URLShortenDBContext _context;

        public UrlCache(IMemoryCache memcache, URLShortenDBContext context)
        {     
            //constructor should init cache and set it
            
            _lookup = new ConcurrentDictionary<string, string>();
            _cache = memcache;
            _context = context;

            DateTime cachedTime = DateTime.Now;

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            //lets go ahead and pull it out of in-mem DB for parallel ops
            
            var urlstore = _context.UrlViewModels.ToList();

            Parallel.ForEach(urlstore, (url) =>
            {
                _lookup.TryAdd(url.ShortAddress, url.Address);
            });
            
            _cache.Set(_lookup, cachedTime, cacheEntryOptions);

        }

    }
}
