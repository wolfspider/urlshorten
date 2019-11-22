using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace urlshorten
{
    public class UrlCache<TItem>
    {
        private static ConcurrentDictionary<string, string> _lookup;
        private static ConcurrentDictionary<object, SemaphoreSlim> _spinlock = new ConcurrentDictionary<object, SemaphoreSlim>();
        private static IMemoryCache _cache;

        private readonly URLShortenDBContext _context;

        public UrlCache(IMemoryCache memcache, URLShortenDBContext context)
        {     
            //constructor should init cache and set it
            
            _cache = memcache;
            _context = context;
            _lookup = new ConcurrentDictionary<string, string>(_context.UrlViewModels.ToDictionary(d => d.ShortAddress, d => d.Address));

            Parallel.ForEach( _lookup, (url) => { _cache.Set(url.Key, url.Value); });
      
        }

        public async Task<TItem> GetOrCreate(object key, Func<Task<TItem>> createItem)
        {
            //check if the item is cached or not and wait on semaphore if blocking...
            if (!_cache.TryGetValue(key, out TItem cacheEntry))// Look for cache key.
            {
                SemaphoreSlim mylock = _spinlock.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

                await mylock.WaitAsync();
                try
                {
                    if (!_cache.TryGetValue(key, out cacheEntry))
                    {
                        // Key not in cache, so get data.
                        cacheEntry = await createItem();
                        _cache.Set(key, cacheEntry);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            return cacheEntry;
        }

    }
}
