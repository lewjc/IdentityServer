using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AssetTrackerIDP.Data;
using AssetTrackerIDP.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authorization;

namespace AssetTrackerIDP
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

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            // 
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            // DB context injection
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // ASP.Net identity configuration
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<ApplicationUserManager>()
                .AddDefaultTokenProviders();

            var requireWindowsProviderPolicy = new AuthorizationPolicyBuilder()
             .RequireClaim("http://schemas.microsoft.com/identity/claims/identityprovider", "Windows")
             .Build();

         
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Permission", "Admin"));
            });


            services.AddMvc();

            // Identity server configuration for using a database and ASP.Net identity to hold all information regarding API, Identity resources,
            // Clients that make use of this identity server, and user accounts.
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                { // This holds information such as clients and resources in the database
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                { // This adds operational data from the database, codes, tokens, consents etc.
                    options.ConfigureDbContext = builder =>
                     builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                    // This allows for automatic token cleanup
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                })

           .AddAspNetIdentity<ApplicationUser>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //app.UseAuthentication();
            app.UseIdentityServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        //private void InitializeDatabase(IApplicationBuilder app)
        //{
        //    using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        //    {
        //        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

        //        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        //        context.Database.Migrate();
        //        if (!context.Clients.Any())
        //        {
        //            foreach (var client in Config.GetClients())
        //            {
        //                context.Clients.Add(client.ToEntity());
        //            }
        //            context.SaveChanges();
        //        }

        //        if (!context.IdentityResources.Any())
        //        {
        //            foreach (var resource in Config.GetIdentityResources())
        //            {
        //                context.IdentityResources.Add(resource.ToEntity());
        //            }
        //            context.SaveChanges();
        //        }

        //        if (!context.ApiResources.Any())
        //        {
        //            foreach (var resource in Config.GetApiResources())
        //            {
        //                context.ApiResources.Add(resource.ToEntity());
        //            }
        //            context.SaveChanges();
        //        }
        //    }
        //}
    }
}
