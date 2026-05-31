/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : File khởi động ứng dụng chính (Program.cs)
              [BẢO MẬT] Cookie: HttpOnly, Secure, SameSite=Strict
              [BẢO MẬT] CORS chỉ cho phép domain nội bộ
              [BẢO MẬT] Rate Limiting chống brute force
              [BẢO MẬT] Security Headers chống XSS, Clickjacking
              [BẢO MẬT] Swagger chỉ chạy ở Development
*/
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using CMS.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// ĐĂNG KÝ SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ĐĂNG KÝ MEMORY CACHE (dùng cho Rate Limiting)
builder.Services.AddMemoryCache();

// ĐĂNG KÝ ENTITY FRAMEWORK CORE
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// ĐĂNG KÝ DỊCH VỤ XÁC THỰC COOKIE
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;
        // [BẢO MẬT] Cookie không đọc được bằng JavaScript
        options.Cookie.HttpOnly = true;
        // [BẢO MẬT] Cookie chỉ gửi qua HTTPS
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        // [BẢO MẬT] Chống CSRF
        options.Cookie.SameSite = SameSiteMode.Strict;
        // [BẢO MẬT] Đặt tên cookie không lộ thông tin framework
        options.Cookie.Name = "MaiTrinh.Session";
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

// [BẢO MẬT] RATE LIMITING — chống brute force và DDoS
builder.Services.AddRateLimiter(options =>
{
    // Giới hạn đăng nhập: tối đa 5 lần / 5 phút theo IP
    options.AddFixedWindowLimiter("LoginLimit", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(5);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Giới hạn toàn bộ API: tối đa 100 request / phút theo IP
    options.AddFixedWindowLimiter("GlobalLimit", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Trả về 429 khi bị giới hạn
    options.RejectionStatusCode = 429;
});

// [BẢO MẬT] CORS chỉ cho phép domain nội bộ
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins("https://localhost:7038")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");

// [BẢO MẬT] SECURITY HEADERS — chống XSS, Clickjacking, sniffing
app.Use(async (context, next) =>
{
    // Chống Clickjacking
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    // Chống MIME sniffing
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    // Chống XSS trên trình duyệt cũ
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    // Chỉ gửi referrer trong cùng domain
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    // Không cache trang đăng nhập
    if (context.Request.Path.StartsWithSegments("/Account"))
    {
        context.Response.Headers.Append("Cache-Control", "no-store, no-cache");
        context.Response.Headers.Append("Pragma", "no-cache");
    }
    await next();
});

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// [BẢO MẬT] MIDDLEWARE CHẶN CUSTOMER
app.Use(async (context, next) =>
{
    var user = context.User;

    if (user.Identity.IsAuthenticated && user.IsInRole("Customer"))
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        var allowedPaths = new[]
        {
            "/",
            "/home",
            "/home/index",
            "/product",
            "/product/index",
            "/product/detail",
            "/cart",
            "/cart/index",
            "/checkout",
            "/checkout/index",
            "/account/logout",
            "/account/login",
            "/account/register"
        };

        bool isAllowed = allowedPaths.Any(p =>
            path == p || path.StartsWith(p + "/"));

        if (!isAllowed)
        {
            context.Response.Redirect("/");
            return;
        }
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.Run();