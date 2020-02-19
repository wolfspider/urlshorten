using Microsoft.EntityFrameworkCore;
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
                
                //We need to initially set at least one admin here
                if (!context.AdminModel.Any())
                {
                    context.AdminModel.AddRange(

                        new Models.AdminModel
                        {
                            User = "jbennett@alachuacounty.us",
                            Groups = ""
                        }

                    );
                }

                if (context.UrlViewModels.Any())
                {
                    return; 
                }

                context.UrlViewModels.AddRange(

                    new Models.UrlViewModel
                    {
                        Active = true,
                        Address = "https://alachuacounty.us",
                        ShortAddress = "8q6pR",
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
