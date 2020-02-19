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

        public DbSet<UrlViewModel> UrlViewModels { get; set; }

        public DbSet<WhiteListModel> WhiteListModel { get; set; }

        public DbSet<AdminModel> AdminModel { get; set; }

    }
}
