using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theia.Data;

namespace Theia
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

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
            })
                .AddIdentity<User, Role>(options =>
                {
                    options.Password.RequiredLength = Configuration.GetValue<int>("Security:PasswordPolicy:RequiredLength");
                    options.Password.RequireDigit = Configuration.GetValue<bool>("Security:PasswordPolicy:RequireDigit");
                    options.Password.RequireLowercase = Configuration.GetValue<bool>("Security:PasswordPolicy:RequireLowercase");
                    options.Password.RequireNonAlphanumeric = Configuration.GetValue<bool>("Security:PasswordPolicy:RequireNonAlphanumeric");
                    options.Password.RequireUppercase = Configuration.GetValue<bool>("Security:PasswordPolicy:RequireUppercase");

                    options.Lockout.MaxFailedAccessAttempts = Configuration.GetValue<int>("Security:Lockout:MaxFailedAccessAttempts");
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(Configuration.GetValue<int>("Security:Lockout:DefaultLockoutTimeSpanInMinutes"));

                    options.User.RequireUniqueEmail = true;

                    options.SignIn.RequireConfirmedEmail = Configuration.GetValue<bool>("Security:SignIn:RequireConfirmedEmail");
                })
                .AddEntityFrameworkStores<AppDbContext>();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext context, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            context.Database.Migrate();

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

            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            new[]
            {
                new Role { DiplayName = "Yöneticiler" , Name = "Administrators" },
                new Role { DiplayName = "Ürün Yöneticileri" , Name = "ProductAdministrators" },
                new Role { DiplayName = "Sipariş Yöneticileri" , Name = "FinanceAdministrators" },
                new Role { DiplayName = "Üyeler" , Name = "Members" },
            }
            .ToList()
            .ForEach(_ =>
            {
                roleManager.CreateAsync(_).Wait();
            });

            var user = new User { Name = Configuration.GetValue<string>("Security:DefaultAdmin:Name"), UserName = Configuration.GetValue<string>("Security:DefaultAdmin:UserName"), Email = Configuration.GetValue<string>("Security:DefaultAdmin:UserName") };
            userManager.CreateAsync(user, Configuration.GetValue<string>("Security:DefaultAdmin:Password")).Wait();
            userManager.AddToRolesAsync(user, new[] { "Administrators", "ProductAdministrators", "FinanceAdministrators", "Members" }).Wait();
        }
    }
}
