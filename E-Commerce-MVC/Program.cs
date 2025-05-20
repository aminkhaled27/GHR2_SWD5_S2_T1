using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Context;
using DataAccessLayer.Repository.IRepository;
using DataAccessLayer.Repository;
using BusinessLayer.Services.IServices;
using BusinessLayer.Services;
using BusinessLayer.Manager.IManager;
using BusinessLayer.Manager;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Stripe;
using AutoMapper;

namespace E_Commerce_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();

            // Register DBContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"), 
                    b => b.MigrationsAssembly("DataAccessLayer"));
            });


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
            option =>
            {
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireDigit = false;
                option.Password.RequireLowercase = false;
                option.Password.RequiredLength = 4;
                option.Password.RequireUppercase = false;

                option.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configure cookie settings
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

         



            #region Register Repositories 

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();

            #endregion

            #region Register Managers

            builder.Services.AddScoped<IProductManager,ProductManager>();
            builder.Services.AddScoped<ICategoryWithProductManager, CategoryWithProductManager>();
            builder.Services.AddScoped<IRegistrationManager,RegisterManager>();

            #endregion

            #region Register Services

            builder.Services.AddScoped<IFileServices, FileServices>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            #endregion

            // Register AutoMapper
            builder.Services.AddAutoMapper(typeof(BusinessLayer.Mapping.MappingProfile).Assembly);

            var stripeSettings = builder.Configuration.GetSection("Stripe");

            var app = builder.Build();

            // Seed admin user
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    context.SeedAdminUser(userManager, roleManager).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

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
            StripeConfiguration.ApiKey = stripeSettings["SecretKey"];
            
            // Configure session before authentication
            app.UseSession();
            
            // Add authentication middleware before authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();

        }
    }
}
