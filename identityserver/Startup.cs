using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static IdentityModel.OidcConstants;

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
            services.AddIdentityServer()
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
                            AllowedGrantTypes={GrantTypes.ClientCredentials},
                            AllowedScopes={"webapi","webclient"}
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
