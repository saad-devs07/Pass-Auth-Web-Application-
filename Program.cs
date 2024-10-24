using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PassAuthWebApp_.Areas.Identity.Data;
using PassAuthWebApp_.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection string
var connectionString = builder.Configuration.GetConnectionString("DbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'DbContextConnection' not found.");

builder.Services.AddDbContext<PassAuthWebApp_.Areas.Identity.Data.DbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Identity with custom ApplicationUser and roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<PassAuthWebApp_.Areas.Identity.Data.DbContext>()
.AddDefaultTokenProviders();

//builder.Services.AddTransient<IEmailSender>();

// Configure Identity options for password requirements and lockout
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    options.Lockout.MaxFailedAccessAttempts = 7;
});

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // Redirect to login
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // In case of unauthorized access

    options.Cookie.Name = "SessionTimeOutCookie";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.SlidingExpiration = true;
});

// Register controllers and Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//builder.Services.AddTransient<IEmailSender>();

var app = builder.Build();

// Create a scope to access services (roles, users)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        var roleNames = new[] { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    logger.LogError($"Error creating role: {roleName}");
                }
            }
        }

        var adminEmail = "admin@pronet-tech.net";
        var adminPassword = "Admin@123";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                LastPasswordChangedDate = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (!addToRoleResult.Succeeded)
                {
                    logger.LogError("Failed to add admin user to Admin role.");
                }
            }
            else
            {
                logger.LogError("Failed to create admin user.");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating roles and admin account.");
    }
}

// Configure error handling and security headers
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    // Allow anonymous access to Login, Register, and other public pages
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");

//    endpoints.MapRazorPages()
//        .RequireAuthorization(); // This ensures all Razor Pages require authorization by default
//});

// Map routes
app.MapRazorPages().RequireAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=ManageUsers}/{id?}");

await app.RunAsync();
