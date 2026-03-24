using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using UrlShortener.Data;
using UrlShortener.Data.Entities.Identity;
using UrlShortener.Data.Interfaces;
using UrlShortener.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// 1. DATABASE - 2 DbContext
// =============================================
builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        }
    )
);

builder.Services.AddDbContext<UrlShortenerIdentityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("IdentityConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        }
    )
);

// =============================================
// 2. IDENTITY
// =============================================
builder.Services.AddIdentity<UrlShortenerUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(10);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<UrlShortenerIdentityDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// =============================================
// 3. COOKIE / AUTH
// =============================================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Authentication/Login";
    options.AccessDeniedPath = "/Home/Error";
});

// =============================================
// 4. REDIS CACHE
// =============================================
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "UrlShortener_";
});

// =============================================
// 5. SESSION
// =============================================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =============================================
// 6. REGISTER SERVICES
// =============================================
builder.Services.AddScoped<IShortenedUrlService, ShortenedUrlService>();
builder.Services.AddScoped<IUrlClickService, UrlClickService>();
builder.Services.AddHttpContextAccessor();

// =============================================
// 7. AUTH & MVC
// =============================================
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// =============================================
// 8. RATE LIMITER
// =============================================
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", opt =>
    {
        opt.PermitLimit = 20;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 5;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please try again later.",
            cancellationToken: token
        );
    };
});

// =============================================
// BUILD APP
// =============================================
var app = builder.Build();

// =============================================
// 🔥 AUTO MIGRATION (QUAN TRỌNG)
// =============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var dbContext = services.GetRequiredService<UrlShortenerDbContext>();
        dbContext.Database.Migrate();

        var identityContext = services.GetRequiredService<UrlShortenerIdentityDbContext>();
        identityContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex.Message}");
    }
}

// =============================================
// 9. MIDDLEWARE PIPELINE
// =============================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();

    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();

// =============================================
// 10. ROUTES
// =============================================

// Route rút gọn URL
app.MapControllerRoute(
    name: "redirect",
    pattern: "r/{shortCode}",
    defaults: new { controller = "UrlShortener", action = "RedirectToOriginal" })
    .RequireRateLimiting("global");

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets()
    .RequireRateLimiting("global");

app.MapRazorPages();

app.Run();