using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monovi.Business.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Monovi.Web.UI
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
            services.AddRazorPages();

            #region DbContext

            services.AddCustomDbContext(Configuration["ConnectionStrings:Default"]);
            #endregion

            #region Dependency Injections
            services.AddContainerWithDependencies();
            #endregion

            #region Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(x =>
               {
                   x.LoginPath = "/";
                   x.ExpireTimeSpan = TimeSpan.FromMinutes(50);
                   x.SlidingExpiration = true;
                   x.Cookie = new CookieBuilder
                   {
                       HttpOnly = true,
                       Name = "Monovi.Cookie"
                   };
               });
            //services.AddControllersWithViews();
            #endregion
            services.AddSession();

            services.AddAutoMapper(typeof(Startup));
            services.AddHttpContextAccessor();
            services.AddTransient<ClaimsPrincipal>(s =>
              s.GetService<IHttpContextAccessor>().HttpContext.User);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

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
            app.UseForwardedHeaders();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            //use session
            app.UseSession();

            app.UseRouting();

            app.UseDefaultFiles();

            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}
