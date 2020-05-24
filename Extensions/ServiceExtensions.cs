using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace urlshorten.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration config)
        {
            
            //get sql connection provider type
            var connectionType = config["sqlprovider:type"];

            switch(connectionType)
            {
                case "sqlite":        
                    var connectionStringSqlite = config["sqliteconnection:connectionString"];
                    services.AddDbContext<URLShortenDBContext>(i => i.UseSqlite(connectionStringSqlite));
                    break;
                case "mssql":
                    var connectionStringMSSQL = config["mssqlconnection:connectionString"];
                    services.AddDbContext<URLShortenDBContext>(i => i.UseSqlServer(connectionStringMSSQL));
                    break;
                case "inmemory":
                    services.AddDbContext<URLShortenDBContext>(i => i.UseInMemoryDatabase(databaseName: "UrlShorten"));
                    break;
                default:
                    services.AddDbContext<URLShortenDBContext>(i => i.UseInMemoryDatabase(databaseName: "UrlShorten"));
                    break;
            }
                    
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options => {
                //options go here
            });
        }

    }

}