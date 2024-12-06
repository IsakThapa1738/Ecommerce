using Ecommerce.Repositories;
using Ecommerce;
using Ecommerce.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure the database context.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add database exception filter for development purposes.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity services with custom password rules and account requirements.
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // Custom password requirements
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        // Other identity options
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// Add support for MVC controllers and views.
builder.Services.AddControllersWithViews();

// Register application-specific services.
builder.Services.AddTransient<IHomeRepository, HomeRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<IUserOrderRepository, UserOrderRepository>();
builder.Services.AddTransient<IStockRepository, StockRepository>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();

// Enable console logging for debugging purposes.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Optional: Seed default data during application startup.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbSeeder.SeedDefaultData(services); // Seed roles and admin user.
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Use migration endpoint during development.
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Handle exceptions in production.
    app.UseHsts(); // Enforce HTTP Strict Transport Security.
}

// Configure middleware for serving static files and routing.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization();  // Enable authorization middleware

// Configure default route mapping.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Enable Razor Pages for Identity.

app.Run();
