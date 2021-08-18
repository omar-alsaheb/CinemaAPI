using CinemaAPI.Models;
using CinemaAPI.Repository.Admin;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI
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
            services.AddTransient<IAdminRepository, AdminRepository>();
            // This method to add cookis
            services.Configure<CookiePolicyOptions>(op =>
            {
                op.CheckConsentNeeded = context => true;
                op.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllers();
            services.AddDbContext<ApplicationDb>(option => option.UseSqlServer(Configuration.GetConnectionString("myCon")));
            services.AddIdentity<ApplicationUser, ApplicationRole>(o=>
            {
                o.Lockout.MaxFailedAccessAttempts = 5;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            }).AddEntityFrameworkStores<ApplicationDb>().AddDefaultTokenProviders();

            services.AddAuthentication(op =>
            {
                op.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                op.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(op =>
            {
                op.Cookie.HttpOnly = true;
                op.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                op.LogoutPath = "/Account/Logout";
                op.SlidingExpiration = true;
                //op.Cookie.SameSite = SameSiteMode.Lax;
                //op.Cookie.IsEssential = true;
            });

            services.AddCors();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(option =>
            {
                option.WithOrigins("http://localhost:4200", "http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
