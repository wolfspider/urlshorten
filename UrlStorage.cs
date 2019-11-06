using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace urlshorten
{
    public class URLShortenDBContext : DbContext
    {
        
        public URLShortenDBContext(DbContextOptions<URLShortenDBContext> options)
            :base(options)
        {

        }

        public DbSet<Models.UrlViewModel> UrlViewModels { get; set; }
    }
}
