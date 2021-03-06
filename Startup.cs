using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using urlshorten.Extensions;


namespace urlshorten
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureIISIntegration();
            services.ConfigureSqlContext(Configuration);
            services.AddSingleton<IUrlCache<string>, UrlCache<string>>();
            services.AddControllersWithViews();

            //ADFS config for auth
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
            })
            .AddWsFederation(options =>
            {
                options.Wtrealm = "urn:adfs:site";
                options.MetadataAddress = "https://adfs.mydomain.com/federationmetadata/2007-06/federationmetadata.xml";
                
                options.CallbackPath = "/_trust";
                
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://adfs.mydomain.com/adfs/services/trust"
                };
                options.CorrelationCookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
            })
            .AddCookie(cookieOptions => {
                
                cookieOptions.Cookie.Name = "FedAuth";
                cookieOptions.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                cookieOptions.Cookie.HttpOnly = true;
                cookieOptions.LoginPath = "/Home";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy =>
                policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims", "Name"));

                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
     
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseBrowserLink();
            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            

            //app.UseADFS();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
