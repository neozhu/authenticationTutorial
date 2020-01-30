using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using basic.App_Start;
using basic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace basic
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
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddDbContext<StoreDbContext>(config =>
            {
                config.UseInMemoryDatabase("db");
            });
            services.AddIdentity<IdentityUser, IdentityRole>(config=> {
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
                config.LoginPath = "/Account/Login";
                config.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                config.ExpireTimeSpan = TimeSpan.FromDays(30);
            });
            
            services.AddAuthentication(config=> {

                config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                config.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
                {
                    config.ExpireTimeSpan = TimeSpan.FromDays(30);
                    config.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                    config.LoginPath = "/Account/Login";
                    config.EventsType = typeof(CustomCookieAuthenticationEvents);
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, config => {
                    var appSettings = appSettingsSection.Get<AppSettings>();
                    var key = Encoding.ASCII.GetBytes(appSettings.Secret);

                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidIssuer = appSettings.Issuer,
                        ValidAudience = appSettings.Audiance,
                       
                    };
                    config.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {

                            if (context.Request.Query.ContainsKey("access_token"))
                            {
                                context.Token = context.Request.Query["access_token"];
                            }
                            return Task.CompletedTask;
                        }
                    };
                })
                ;
            services.AddAuthorization(config=> {
                //var defaultAuthbuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthbuilder
                //.RequireAuthenticatedUser()
                //.RequireClaim(claimType:ClaimTypes.Email)
                //.Build();
                //config.DefaultPolicy = defaultAuthPolicy;

                //config.AddPolicy("claim.email", policyBuilder =>
                //{
                //    policyBuilder.RequireClaim(ClaimTypes.Email);
                //});

                //config.AddPolicy("claim.email", policyBuilder =>
                //{
                //    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.Email));
                //});

                config.AddPolicy("claim.email", policyBuilder =>
                {
                    policyBuilder.RequireCustomClaim();
                });

                config.AddPolicy("role", policyBuilder =>
                {
                    policyBuilder.RequireClaim(ClaimTypes.Role);
                });

                config.AddPolicy("admin", policyBuilder =>
                {
                    policyBuilder.RequireClaim(ClaimTypes.Role,"admin");
                });

                config.AddPolicy("DoStuff", policyBuilder =>
                {
                    policyBuilder.RequireClaim("ToDo");
                });
            });
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
            services.AddScoped<CustomCookieAuthenticationEvents>();
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
            _ = services.AddControllersWithViews(config=> {
                //var defaultAuthbuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthbuilder
                //.RequireAuthenticatedUser()
                //.Build();
                ////global authorization filter
                //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
            });
            //services.AddRazorPages();
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
            app.UseAuthentication();
            app.UseAuthorization();
           
          
        
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                //endpoints.MapRazorPages();
            });
        }
    }
}
