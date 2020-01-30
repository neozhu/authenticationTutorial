using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using identityserver.Models;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace identityserver
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StoreDbContext>(config =>
            {
                config.UseInMemoryDatabase("db");
            });
            services.AddIdentity<IdentityUser, IdentityRole>(config =>
            {
                config.Password.RequireDigit = false;
                config.Password.RequiredLength = 1;
                config.Password.RequiredUniqueChars = 0;
                config.Password.RequireUppercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<StoreDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Login";
                config.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                config.ExpireTimeSpan = TimeSpan.FromDays(30);
            });
            services.AddIdentityServer()
                //.AddAspNetIdentity<IdentityUser>()
                //.AddConfigurationStore(config => {
                //    config.ConfigureDbContext = b => b.UseInMemoryDatabase("db");
                //})
                //.AddOperationalStore(config => {
                //    config.ConfigureDbContext = b => b.UseInMemoryDatabase("db");
                //})
                .AddInMemoryIdentityResources(
                new IdentityResource[]
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile()
                }
                )
                .AddInMemoryApiResources(
                   new IdentityServer4.Models.ApiResource[] {
                       new IdentityServer4.Models.ApiResource("webapi"),
                       new IdentityServer4.Models.ApiResource("webclient")
                   }
                )
                .AddInMemoryClients(
                   new IdentityServer4.Models.Client[]
                   {
                       new IdentityServer4.Models.Client()
                       {
                            ClientId="client_id",
                            ClientSecrets={
                               new IdentityServer4.Models.Secret(
                                   "client_secret".ToSha256() )
                           },
                            AllowedGrantTypes=GrantTypes.ClientCredentials,
                            AllowedScopes={"webapi"}
                       },
                       new IdentityServer4.Models.Client()
                       {
                            ClientId="client_page",
                            ClientSecrets={
                               new IdentityServer4.Models.Secret(
                                   "client_secret".ToSha256() )
                           },
                            RedirectUris={"https://localhost:5004/signin-oidc"},
                            AllowedGrantTypes=GrantTypes.Code,
                            AllowedScopes={"webapi",
                               "webclient",
                             IdentityServerConstants.StandardScopes.Profile,
                             IdentityServerConstants.StandardScopes.OpenId
                             }
                       }
                   }
                )
                .AddDeveloperSigningCredential();
            services.AddRazorPages();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
