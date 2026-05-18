/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : File khởi động ứng dụng chính (Program.cs)
              - Đăng ký Entity Framework Core với LocalDB
              - Database: HaCMS_2122110132
              - Đã cấu hình đầy đủ HTTPS, Routing, Static Files
              
*/

using Microsoft.EntityFrameworkCore;
using CMS.Data; // Quan trọng: để nhận diện ApplicationDbContext

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ĐĂNG KÝ ENTITY FRAMEWORK CORE VỚI SQL SERVER LOCALDB 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();