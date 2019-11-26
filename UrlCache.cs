using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace urlshorten
{

    public interface IUrlCache<TItem>
    {
        Task<string> GetOrCreate(string key, URLShortenDBContext context);
    }

    public class UrlCache<TItem> : IUrlCache<TItem>
    {
        private static readonly ConcurrentDictionary<object, SemaphoreSlim> _spinlock = new ConcurrentDictionary<object, SemaphoreSlim>();
        private static IMemoryCache _cache;

        public UrlCache(IServiceProvider svc)
        {
            if (_cache == null)
            {

                _cache = new MemoryCache(new MemoryCacheOptions());

                //constructor should init cache and set it
                using (var context = new URLShortenDBContext(svc.GetRequiredService<DbContextOptions<URLShortenDBContext>>()))
                {
                    context.UrlViewModels.ForEachAsync(c => _cache.Set(c.ShortAddress, c.Address));
                }

            }

        }

        public async Task<string> GetOrCreate(string key, URLShortenDBContext context)
        {
            //check if the item is cached or not and wait on semaphore if blocking...
            if (!_cache.TryGetValue(key, out string cacheEntry))// Look for cache key.
            {
                SemaphoreSlim mylock = _spinlock.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

                await mylock.WaitAsync();
                try
                {
                    if (!_cache.TryGetValue(key, out cacheEntry))
                    {
                        // Key not in cache, so get data.
                        cacheEntry = context.UrlViewModels.Where(x => x.ShortAddress == key).FirstOrDefault().Address;
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
