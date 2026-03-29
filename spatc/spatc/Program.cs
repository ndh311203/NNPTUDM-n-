using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using spatc.Models;
using spatc.Data;
using spatc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        // Thêm view locations cho Admin area
        options.ViewLocationFormats.Add("/Views/Admin/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Admin/Shared/{0}.cshtml");
    });

// Cấu hình Authentication với Cookie và OAuth
var authBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// Chỉ thêm Google OAuth nếu đã cấu hình credentials
var googleClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";

if (!string.IsNullOrEmpty(googleClientId) && googleClientId != "YOUR_GOOGLE_CLIENT_ID" &&
    !string.IsNullOrEmpty(googleClientSecret) && googleClientSecret != "YOUR_GOOGLE_CLIENT_SECRET")
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.CallbackPath = "/Login/GoogleCallback";
    });
}

// Chỉ thêm Facebook OAuth nếu đã cấu hình credentials
var facebookAppId = builder.Configuration["Authentication:Facebook:AppId"] ?? "";
var facebookAppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? "";

if (!string.IsNullOrEmpty(facebookAppId) && facebookAppId != "YOUR_FACEBOOK_APP_ID" &&
    !string.IsNullOrEmpty(facebookAppSecret) && facebookAppSecret != "YOUR_FACEBOOK_APP_SECRET")
{
    authBuilder.AddFacebook(options =>
    {
        options.AppId = facebookAppId;
        options.AppSecret = facebookAppSecret;
        options.CallbackPath = "/Login/FacebookCallback";
    });
}

// Register ApplicationDbContext with SQL Server using the DefaultConnection from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Register Services
builder.Services.AddScoped<VietQRService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ChatService>();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Kiểm tra kết nối database và seed data khi khởi động
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var canConnect = context.Database.CanConnect();
        if (canConnect)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("✅ Kết nối database thành công!");
            logger.LogInformation($"Database: {context.Database.GetDbConnection().Database}");
            logger.LogInformation($"Server: {context.Database.GetDbConnection().DataSource}");
            
            // Seed data cho admin
            DbInitializer.Initialize(context);
            logger.LogInformation("✅ Đã khởi tạo dữ liệu admin!");
        }
        else
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("⚠️ Không thể kết nối đến database!");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Lỗi khi kiểm tra kết nối database!");
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

// Authentication & Authorization phải được gọi theo thứ tự này
app.UseAuthentication();
app.UseAuthorization();

// Map SignalR Hubs
app.MapHub<spatc.Hubs.NotificationHub>("/notificationHub");
app.MapHub<spatc.Hubs.ChatHub>("/chatHub");

// Map controllers với attribute routing trước
app.MapControllers();

// Convention routing cho các controller không có attribute routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
