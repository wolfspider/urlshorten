using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace urlshorten
{
    public class ADFS
    {
        readonly RequestDelegate next;

        public IConfiguration Configuration { get; }

        public ADFS(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            Configuration = configuration;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            if (httpContext.User.Identity.IsAuthenticated && httpContext.User.Claims.FirstOrDefault().Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && httpContext.User.Identity.Name == null)
            {
                var claims = new List<Claim>();

                var userName = httpContext.User.Claims.FirstOrDefault().Value;
                var adGroups = httpContext.User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList();

                StringBuilder adGroupsList = new StringBuilder();

                foreach (var group in adGroups)
                {
                    adGroupsList.Append(group.Value + ",");
                }

                var groupsList = adGroupsList.ToString().TrimEnd(',');

                claims.Add(new Claim("Name", userName));
                claims.Add(new Claim("Group", groupsList));

                if(groupsList.Contains("ITS"))
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }
                else
                {
                    //TODO: Make an Access Denied Page
                    httpContext.Response.Redirect("Account/Logout");
                }   

                var appIdentity = new ClaimsIdentity(claims, "Basic", "Name", "Group");

                var currentIdentities = httpContext.User.Identities as List<ClaimsIdentity>;

                var currentClaim = currentIdentities?.FirstOrDefault();

                if (currentClaim != null)
                    currentIdentities.Remove(currentClaim);

                httpContext.User.AddIdentity(appIdentity);

                var tokenHandler = new JwtSecurityTokenHandler();

                var signingKey = Configuration["ADFSJWT:guid"];

                var simpleKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = appIdentity,
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(simpleKey, SecurityAlgorithms.HmacSha256)
                };

                var jwttoken = tokenHandler.CreateToken(tokenDescriptor);

                ClaimsPrincipal cp = new ClaimsPrincipal(appIdentity);
                AuthenticationProperties authprops = new AuthenticationProperties();

                authprops.StoreTokens(new[]
                {
                        new AuthenticationToken()
                        {
                            Name = "JWT",
                            Value = jwttoken.ToString()
                        }
                });

                await httpContext.SignInAsync(cp, authprops);
            }

            await next(httpContext);
        }
        

    }

    public static class SiteExtensions
    {
        public static IApplicationBuilder UseADFS(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ADFS>();
        }
    }
}