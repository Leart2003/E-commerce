using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Punim_Diplome.Data;
using Punim_Diplome.Data.Services;
using System.Security.Claims;

namespace Punim_Diplome
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var authorizedEmail = builder.Configuration["AuthorizedUser:Email"];

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddScoped<IKomentService, KomentService>();
            builder.Services.AddScoped<IProduktService, ProduktService>();
   


            //Injektimi i servisit per admin permision
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminEmail", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var emailClaim = context.User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
                        var authorizedEmail = builder.Configuration["AuthorizedUser:Email"];
                        return emailClaim == authorizedEmail;
                    }));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Produkt}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
