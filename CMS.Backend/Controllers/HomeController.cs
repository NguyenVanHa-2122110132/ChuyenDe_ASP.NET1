/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : Controller trang chủ của hệ thống CMS
              - Index     : Trang chủ công khai - hiển thị 3 bài viết mới nhất
              - Dashboard : Trang bảng điều khiển Admin - thống kê số lượng danh mục, bài viết, thành viên
              - Privacy   : Trang chính sách bảo mật
              - Error     : Trang xử lý lỗi hệ thống
*/
using CMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Thêm namespace để dùng [Authorize]
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using System.Diagnostics;

namespace CMS.Backend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public HomeController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // Trang chủ công khai - không cần đăng nhập
        public IActionResult Index()
        {
            // Lấy 3 bài viết mới nhất, kèm thông tin danh mục
            var latestPosts = _context.Posts
                              .Include(p => p.Category)
                              .OrderByDescending(p => p.CreatedDate)
                              .Take(3)
                              .ToList();
            return View(latestPosts);
        }

        // Dashboard: chỉ Administrator, Admin, Sales, Cashier mới được vào
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        public IActionResult Dashboard()
        {
            ViewBag.TotalCategories = _context.Categories.Count(); // Đếm tổng số danh mục
            ViewBag.TotalPosts = _context.Posts.Count();           // Đếm tổng số bài viết
            ViewBag.TotalUsers = _context.Users.Count();           // Đếm tổng số thành viên
            return View();
        }

        // Trang chính sách bảo mật - công khai
        public IActionResult Privacy() => View();

        // Trang xử lý lỗi hệ thống - không cache
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}