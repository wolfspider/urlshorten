# urlshorten
Url Shortener written in .Net Core utilizing some new techniques

JS is written in ES6 and have included [Url-Knife](https://github.com/Andrew-Kang-G/url-knife) to do
fuzzy matching on super long URLs.

Two levels of memory caching are used here one is the typical ASP .Net response cache and the other is a custom memory cache
service which takes advantage of System.Threading.Channels instead of using a typical C# spinlock routine for caching.

