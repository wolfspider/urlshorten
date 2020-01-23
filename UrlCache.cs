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
            
            var _channel = Channel.CreateUnbounded<string>();
            var _crequest = new CacheRequest(_channel.Writer, 1, 0);
            var _cset = new CacheSet(_channel.Reader, 1, 0);

            var address = await _crequest.GetCache(key)
            .ContinueWith(_ => _channel.Writer.Complete())
            .ContinueWith(_ => _cset.GetOrCreate(context));
            
            return await address;
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
                //TODO: Think of something better than this
                return "error";
                
            }

        }

    }

}
