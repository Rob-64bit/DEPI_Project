using Bookify.wep.Data;
using Bookify.Data.Data;
using Bookify.service.Repositories;
using Bookify.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using Bookify.wep.Services.Auth;

// Alias DbContexts to avoid ambiguity
using IdentityDbContext = Bookify.wep.Data.ApplicationDbContext;
using HotelDbContext = Bookify.Data.Data.ApplicationDbContext;

namespace Bookify.wep
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            var hotelConnection = builder.Configuration.GetConnectionString("HotelConnection") ?? throw new InvalidOperationException("Connection string 'HotelConnection' not found.");
            builder.Services.AddDbContext<HotelDbContext>(options =>
                options.UseSqlServer(hotelConnection));

            // Repositories
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IGuestRepository, GuestRepository>();
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

            // --- Temporarily comment out Identity registration while we add custom auth
            // builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<IdentityDbContext>();

            // Keep MVC + FluentValidation
            builder.Services.AddControllersWithViews();
            builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

            // Add Session support
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromHours(4);
            });



            // Register our custom auth service (we will create these files)
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                // keep migration endpoint if you use it; okay to leave.
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            // Use Session (important)
            app.UseSession();

            // If you later enable Identity/Authentication uncomment:
            // app.UseAuthentication();
            // app.UseAuthorization();

            // Map routes for Areas first (important)
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            // Default (fallback) route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // MapRazorPages(); // uncomment if you re-enable Identity UI

            //للسترايب
            var stripeSettings = builder.Configuration.GetSection("Stripe");
            Stripe.StripeConfiguration.ApiKey = stripeSettings["SecretKey"];



            app.Run();
        }
    }
}
