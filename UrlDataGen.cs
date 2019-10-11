﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace urlshorten
{
    public class UrlDataGen
    {
        public static void Init(IServiceProvider svc)
        {
            using (var context = new URLShortenDBContext(svc.GetRequiredService<DbContextOptions<URLShortenDBContext>>()))
            {
                if (context.UrlViewModels.Any())
                {
                    return; 
                }

                context.UrlViewModels.AddRange(

                    new Models.UrlViewModel
                    {
                        Id = 1,
                        Active = true,
                        Address = new Uri("https://alachuacounty.us"),
                        ShortAddress = new Uri("https://url.acbocc.us/#/da5"),
                        UrlHash = 12345,
                        Created = DateTime.Now,
                        Title = "Alachua County",
                        User = "System"
                    }

                );

                context.SaveChanges();
            }
        }
    }
}
