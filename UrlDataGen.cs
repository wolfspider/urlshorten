using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

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
                            User = "admin@mydomain.com",
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
                        Address = "https://mydomain.com",
                        ShortAddress = "QGe8i",
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
