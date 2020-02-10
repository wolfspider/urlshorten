using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using urlshorten.Models;

namespace urlshorten
{
    public class URLShortenDBContext : DbContext
    {
        
        public URLShortenDBContext(DbContextOptions<URLShortenDBContext> options)
            :base(options)
        {

        }

        public DbSet<Models.UrlViewModel> UrlViewModels { get; set; }

        public DbSet<urlshorten.Models.WhiteListModel> WhiteListModel { get; set; }
    }
}
