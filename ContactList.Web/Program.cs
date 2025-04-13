using ContactList.Web.Common.Services;
using ContactList.Web.Models;
using ContactList.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Add DbContext with logging
builder.Services.AddDbContext<AppDbContext>(options =>
{
    try
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    catch (Exception ex)
    {
        var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
        logger.LogError($"Error while setting up the database connection: {ex.Message}");
        throw;  // Rethrow the exception after logging
    }
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IContactService, ContactService>();
// Register the RedisService as IRedisService
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddSession();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

//builder.Services.AddDistributedSqlServerCache(options =>
//{
//    options.ConnectionString = builder.Configuration.GetConnectionString("SessionDb");
//    options.SchemaName = "dbo";
//    options.TableName = "SharedSession";
//});

// Add distributed SQL Server session support
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";  // Pointing to  Redis server
    options.InstanceName = "ContactListApp_";  // Optional: to scope  Redis cache
});

// Add session services
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".ContactListApp.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Adjust based on  requirements
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use HTTPS in production
});

builder.Services.AddAuthorization();


var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();



