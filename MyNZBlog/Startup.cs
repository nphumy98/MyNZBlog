using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyNZBlog.Data;

namespace MyNZBlog
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
            services.AddControllersWithViews();
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                services.AddDbContext<MyNZBlogContext>(options =>
                  options.UseSqlServer(Configuration.GetConnectionString("MyNZBlogContext")));
            }
            else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local")
            {
                services.AddDbContext<MyNZBlogContext>(options =>
                  //options.UseSqlServer(Configuration.GetConnectionString("MyNZBlogContext")));
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyNZBlogContext-1;Trusted_Connection=True;MultipleActiveResultSets=true"));
            }
            else
            { // intend to delete , need to find the way set up aspcore environment to production or development
                services.AddDbContext<MyNZBlogContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("MyNZBlogContext")));
            }
                
            services.AddAuthentication(o => {
                    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    //o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
                {
                    config.Cookie.Name = "MyNZBlog.Cookie";
                    config.LoginPath = "/Admin/Login";
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
