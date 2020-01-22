using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
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
            /*
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
            */

            var channel = Channel.CreateUnbounded<string>();

            var request1 = new CacheRequest(channel.Writer, 1, 0);
            var response1 = new CacheSet(channel.Reader, 1, 0);
            //var response2 = new CacheSet(channel.Reader, 2, 0);
            //var response3 = new CacheSet(channel.Reader, 3, 0);

            Task cacheTask1 = response1.GetOrCreate(context);  
            //Task cacheTask2 = response2.GetOrCreate(context);
            //Task cacheTask3 = response3.GetOrCreate(context);

            Task cacheRequest = request1.GetCache(key);

            await cacheRequest.ContinueWith(_ => channel.Writer.Complete());  

            return await response1.GetOrCreate(context);
        }

        internal class CacheRequest
        {
            private readonly ChannelWriter<string> _cache_req;
            private readonly int _cache_id;
            private readonly int _cache_delay;

            public CacheRequest(ChannelWriter<string> cache_req, int cache_id, int cache_delay)
            {
                _cache_req = cache_req;
                _cache_id = cache_id;
                _cache_delay = cache_delay;
            }

            public async Task GetCache(string key)
            {

                await Task.Delay(_cache_delay);

                await _cache_req.WriteAsync(key);

            }
        }

        internal class CacheSet
        {
            private readonly ChannelReader<string> _cache_req;

            private readonly int _cache_id;

            private readonly int _cache_delay;

            public CacheSet(ChannelReader<string> cache_req, int cache_id, int cache_delay)
            {
                _cache_req = cache_req;
                _cache_id = cache_id;
                _cache_delay = cache_delay;
            }

            public async Task<string> GetOrCreate(URLShortenDBContext context)
            {
                
                while (await _cache_req.WaitToReadAsync())
                {
                    if (_cache_req.TryRead(out var key))
                    {
                        await Task.Delay(_cache_delay);

                        if (!_cache.TryGetValue(key, out string cacheEntry))// Look for cache key.
                        {

                            // Key not in cache, so get data.
                            cacheEntry = context.UrlViewModels.Where(x => x.ShortAddress == key).FirstOrDefault().Address;
                            _cache.Set(key, cacheEntry);

                        }

                        return cacheEntry;
                    }

                }

                return "error";
                
            }

        }

    }

}
