﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using urlshorten.Models;

namespace urlshorten
{
    public interface IUrlCache<TItem>
    {
        Task<string> GetOrCreate(string key, URLShortenDBContext context);
        Task<bool> Remove(string key);
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
                    var cacheEntries = context.UrlViewModels
                    .Where(c => c.Active == true)
                    .Select(c => new { c.ShortAddress, c.Address })
                    .ToList();

                    foreach (var entry in cacheEntries)
                    {
                        _cache.Set(entry.ShortAddress, entry.Address);
                    }

                }

            }

        }

        public async Task<string> GetOrCreate(string key, URLShortenDBContext context)
        {

            var _channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions()
            {
                SingleWriter = true,
                SingleReader = false
            });

            var _crequest = new CacheRequest(_channel.Writer, 1);
            var _cset = new CacheSet(_channel.Reader, 1);
            var _cset2 = new CacheSet(_channel.Reader, 2);
            var _cset3 = new CacheSet(_channel.Reader, 3);

            await _crequest.GetCache(key)
            .ContinueWith(_ => _channel.Writer.Complete());

            var address = await Task.WhenAny(_cset.GetOrCreate(context), _cset2.GetOrCreate(context), _cset3.GetOrCreate(context));

            return await address;
        }

        public async Task<bool> Remove(string key)
        {
            bool success = false;

            await Task.Run(() =>
            {
                try
                {
                    _cache.Remove(key);
                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });

            return success;
        }

        internal class CacheRequest
        {
            private readonly ChannelWriter<string> _cache_req;
            private readonly int _cache_id;

            public CacheRequest(ChannelWriter<string> cache_req, int cache_id)
            {
                _cache_req = cache_req;
                _cache_id = cache_id;
            }

            public async Task GetCache(string key)
            {

                while (await _cache_req.WaitToWriteAsync())
                {
                    if (_cache_req.TryWrite(key)) break;
                }

            }
        }

        internal class CacheSet
        {
            private readonly ChannelReader<string> _cache_req;
            private readonly int _cache_id;

            public CacheSet(ChannelReader<string> cache_req, int cache_id)
            {
                _cache_req = cache_req;
                _cache_id = cache_id;
            }

            public async Task<string> GetOrCreate(URLShortenDBContext context)
            {

                while (await _cache_req.WaitToReadAsync())
                {
                    while (_cache_req.TryRead(out var key))
                    {

                        if (!_cache.TryGetValue(key, out string cacheEntry))// Look for cache key.
                        {

                            // Key not in cache, so get data.
                            //TODO: Handle null values way more elegantly
                            var urlViewModel = context?.UrlViewModels?
                            .Where(x => x.ShortAddress == key && x.Active == true)?
                            .FirstOrDefault();

                            cacheEntry = urlViewModel?.Address ?? "not found";

                            if (cacheEntry != "not found")
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
