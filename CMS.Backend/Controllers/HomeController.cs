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
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using System.Diagnostics;

namespace CMS.Backend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var latestPosts = _context.Posts
                              .Include(p => p.Category)
                              .OrderByDescending(p => p.CreatedDate)
                              .Take(3)
                              .ToList();
            return View(latestPosts);
        }

        // ← THÊM MỚI: action Dashboard cho khu vực Admin
        public IActionResult Dashboard()
        {
            ViewBag.TotalCategories = _context.Categories.Count();
            ViewBag.TotalPosts = _context.Posts.Count();
            ViewBag.TotalUsers = _context.Users.Count();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}