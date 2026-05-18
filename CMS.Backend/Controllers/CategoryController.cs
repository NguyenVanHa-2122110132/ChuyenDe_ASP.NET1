/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : Controller quản lý Danh mục điện thoại (Lấy dữ liệu thật từ SQL)
*/

using Microsoft.AspNetCore.Mvc;
using CMS.Data; // Quan trọng: để dùng ApplicationDbContext
using Microsoft.EntityFrameworkCore; // Để dùng lệnh lấy danh sách

namespace CMS.Backend.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        // "Tiêm" kết nối Database vào Controller (Dependency Injection)
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy dữ liệu THẬT từ bảng Categories trong SQL Server
            var data = _context.Categories.ToList();

            return View(data); // Gửi dữ liệu thật sang giao diện
        }
    }
}