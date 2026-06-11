/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026 (Cập nhật cấu hình CORS Buổi 7: 02/06/2026)
    Mô tả    : File khởi động ứng dụng chính (Program.cs) tích hợp kết nối ReactJS Frontend
              - [BẢO MẬT] Cookie: HttpOnly, Secure, SameSite=Strict
              - [BẢO MẬT] CORS: Cho phép domain ReactJS (http://localhost:3000) gọi API qua chính sách AllowReactApp
              - [BẢO MẬT] Rate Limiting chống brute force và tấn công từ chối dịch vụ DDoS
              - [BẢO MẬT] Security Headers chống các lỗ hổng XSS, Clickjacking, MIME sniffing
              - [BẢO MẬT] Swagger UI hỗ trợ kiểm thử API tự động trong môi trường Development
*/
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using CMS.Data;
using CMS.Backend.Services;
var builder = WebApplication.CreateBuilder(args);

// ĐĂNG KÝ DỊCH VỤ CONTROLLER VÀ PHƯƠNG THỨC CẤU HÌNH JSON (Tránh lỗi vòng lặp tham chiếu)
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// ĐĂNG KÝ SWAGGER (Tự động tạo tài liệu và giao diện kiểm thử API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ĐĂNG KÝ MEMORY CACHE (Dùng làm bộ nhớ đệm lưu trữ lượt truy cập cho Rate Limiting)
builder.Services.AddMemoryCache();

// ĐĂNG KÝ ENTITY FRAMEWORK CORE (Kết nối cơ sở dữ liệu SQL Server qua chuỗi DefaultConnection)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// ĐĂNG KÝ DỊCH VỤ XÁC THỰC (COOKIE CHO BACKEND + JWT CHO REACT)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
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
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("MaiTrinhSecretKey2026MaiTrinhSecretKey2026")),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

// [BẢO MẬT] RATE LIMITING — Giới hạn tần suất gửi yêu cầu nhằm chống brute force và tấn công DDoS
builder.Services.AddRateLimiter(options =>
{
    // Giới hạn đăng nhập hệ thống: Tối đa cho phép 5 lần / mốc 5 phút dựa trên địa chỉ IP Client
    options.AddFixedWindowLimiter("LoginLimit", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(5);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Giới hạn toàn bộ hệ thống API: Tối đa cho phép 100 request / 1 phút dựa trên địa chỉ IP Client
    options.AddFixedWindowLimiter("GlobalLimit", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Thiết lập trả về mã trạng thái lỗi 429 (Too Many Requests) khi Client vượt ngưỡng giới hạn
    options.RejectionStatusCode = 429;
});

// CẤU HÌNH DỊCH VỤ CORS (MỞ CỬA CHO FRONTEND REACTJS)
builder.Services.AddCors(options => {
    // Đổi tên chính sách sang "AllowReactApp" để đồng bộ theo tài liệu hướng dẫn Buổi 7
    options.AddPolicy("AllowReactApp", policy => {
        // Cập nhật đường dẫn chuẩn sang Port 3000 của ReactJS ứng dụng Thương mại điện tử
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyMethod()                     // Cho phép ReactJS sử dụng mọi phương thức (GET, POST, PUT, DELETE)
              .AllowAnyHeader()                     // Cho phép ReactJS gửi lên các Header tùy biến (Content-Type, Authorization...)
              .AllowCredentials();                  // Bảo lưu cơ chế hỗ trợ truyền kèm thông tin Cookie/Session an toàn
    });
});

// ĐĂNG KÝ EMAIL SERVICE - Gửi email qua Gmail SMTP
builder.Services.AddScoped<IEmailService, EmailService>();
var app = builder.Build();

// CẤU HÌNH ĐƯỜNG ỐNG XỬ LÝ REQUEST (MIDDLEWARE PIPELINE)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Hiển thị trang báo lỗi chi tiết trong môi trường lập trình
    app.UseSwagger();                // Kích hoạt sinh tài liệu Swagger JSON
    app.UseSwaggerUI();              // Kích hoạt giao diện đồ họa tương tác Swagger UI
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Chuyển hướng tới trang báo lỗi chung khi chạy production
    app.UseHsts();                          // Kích hoạt cơ chế bảo mật HTTP Strict Transport Security
}

app.UseHttpsRedirection(); // Tự động chuyển hướng toàn bộ request từ HTTP sang HTTPS an toàn
app.UseStaticFiles();      // Cho phép truy cập vào các tệp tin tĩnh (Hình ảnh, CSS, JS trong thư mục wwwroot)
app.UseRouting();          // Kích hoạt hệ thống định tuyến phân tích URL

//  KÍCH HOẠT CORS ĐÚNG VỊ TRÍ BẢO MẬT NGHIÊM NGẶT
// Đặt ngay sau UseRouting và bắt buộc phải đứng trước UseAuthorization để trình duyệt chấp nhận quyền gọi API
app.UseCors("AllowReactApp");

// [BẢO MẬT] SECURITY HEADERS — Thiết lập các cờ Header chống tấn công XSS, Clickjacking và MIME Sniffing
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "DENY"); // Ngăn chặn trang web bị nhúng vào thẻ iframe (Chống Clickjacking)
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff"); // Ép trình duyệt tuân thủ đúng định dạng MIME tệp tin
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block"); // Kích hoạt bộ lọc chống mã độc XSS trên trình duyệt cũ
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin"); // Cấu hình chính sách gửi thông tin nguồn gốc yêu cầu

    // Xử lý xóa bỏ bộ nhớ đệm (Cache) đối với các trang thuộc khu vực quản lý tài khoản bảo mật
    if (context.Request.Path.StartsWithSegments("/Account"))
    {
        context.Response.Headers.Append("Cache-Control", "no-store, no-cache");
        context.Response.Headers.Append("Pragma", "no-cache");
    }
    await next(); // Chuyển tiếp yêu cầu sang Middleware tiếp theo trong đường ống
});

app.UseRateLimiter();    // Kích hoạt bộ kiểm soát tần suất truy cập
app.UseAuthentication(); // Kích hoạt Middleware xác thực danh tính người dùng
app.UseAuthorization();  // Kích hoạt Middleware kiểm tra quyền hạn truy cập tài nguyên (Role-based)

// [BẢO MẬT] MIDDLEWARE CHẶN VÀ PHÂN QUYỀN ĐIỀU HƯỚNG DÀNH RIÊNG CHO KHÁCH HÀNG (CUSTOMER)
app.Use(async (context, next) =>
{
    var user = context.User;

    // Kiểm tra nếu tài khoản đã đăng nhập thành công và giữ vai trò là "Customer" (Khách mua hàng)
    if (user.Identity != null && user.Identity.IsAuthenticated && user.IsInRole("Customer"))
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // Danh sách các đường dẫn công khai mà tài khoản Customer được phép truy cập tự do ở Frontend/Giao diện gốc
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

        // Kiểm tra xem URL hiện tại có nằm trong danh sách được phép hay không
        bool isAllowed = allowedPaths.Any(p =>
            path == p || path.StartsWith(p + "/"));

        // Nếu Customer cố tình truy cập vào các đường dẫn quản trị (Admin/nhân viên), lập tức điều hướng về trang chủ công cộng
        if (!isAllowed)
        {
            context.Response.Redirect("/");
            return; // Ngắt luồng request, không cho phép đi sâu vào hệ thống
        }
    }

    await next(); // Nếu hợp lệ, cho phép đi tiếp
});

// CẤU HÌNH ĐỊNH TUYẾN MẶC ĐỊNH CHO MÔ HÌNH MVC CONTROLLER (Hệ thống trang View giao diện)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// CẤU HÌNH ĐỊNH TUYẾN CHO CÁC WEB API CONTROLLER (Hệ thống API phục vụ Swagger và ReactJS)
app.MapControllers();

// Khởi chạy ứng dụng toàn hệ thống
app.Run();