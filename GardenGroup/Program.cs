using GardenGroup.Repositories.Interfaces;
using GardenGroup.Repositories;
using MongoDB.Driver;
using GardenGroup.Services.interfaces;
using GardenGroup.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AspNetCore.Identity.Mongo;
using GardenGroup.Models;

namespace GardenGroup
{
    public class Program
    {

        public static void Main(string[] args)
        {
            // Load .env before building configuration so env vars are available
            DotNetEnv.Env.TraversePath().Load();

            var builder = WebApplication.CreateBuilder(args);

            // 1) Register MongoClient as a SINGLETON (one shared instance for the whole app)
            // WHY: MongoClient is thread-safe and internally manages a connection pool.
            // Reusing one instance is fast and efficient. Creating many clients would waste resources.
            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                // Read the connection string from configuration (env var via .env)
                var conn = builder.Configuration["Mongo_ConnectionString"];
                if (string.IsNullOrWhiteSpace(conn))
                    throw new InvalidOperationException("Mongo:ConnectionString is not configured. Did you set it in .env?");

                // Optional: tweak settings (timeouts, etc.)
                var settings = MongoClientSettings.FromConnectionString(conn);
                // settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

                return new MongoClient(settings);
            });

            // 2) Register IMongoDatabase as SCOPED (new per HTTP request)
            // WHY: Fits the ASP.NET request lifecycle and keeps each request cleanly separated.
            builder.Services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();

                var dbName = builder.Configuration["Mongo:Database"]; // from appsettings.json
                if (string.IsNullOrWhiteSpace(dbName))
                    throw new InvalidOperationException("Mongo:Database is not configured in appsettings.json.");

                return client.GetDatabase(dbName);
            });

            builder.Services
                .AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(identityOptions =>
                {
                    identityOptions.Password.RequiredLength = 8;
                    identityOptions.Password.RequireNonAlphanumeric = false;
                    identityOptions.Password.RequireDigit = true;
                    identityOptions.Password.RequireUppercase = false;
                },
                mongoIdentityOptions =>
                {
                    mongoIdentityOptions.ConnectionString = builder.Configuration["Mongo_ConnectionString"];
                    mongoIdentityOptions.UsersCollection = "AspNetUsers"; 
                    mongoIdentityOptions.RolesCollection = "AspNetRoles";
                });

            builder.Services.AddControllersWithViews(options =>
            {
               var policy = new AuthorizationPolicyBuilder()
                   .RequireAuthenticatedUser()
                   .Build();
               options.Filters.Add(new AuthorizeFilter(policy));
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/User/Login";
                options.LogoutPath = "/User/Logout";
                options.AccessDeniedPath = "/User/Login";
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<ITicketService, TicketService>();
          
            builder.Services.AddScoped<IArchiveService, ArchiveService>();
            builder.Services.AddScoped<IArchiveRepository, ArchiveRepository>();

            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
            builder.Services.AddSingleton(resolver => resolver.GetRequiredService<Microsoft.Extensions.Options.IOptions<SmtpOptions>>().Value);

            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddSession();
            builder.Services.AddDistributedMemoryCache();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
