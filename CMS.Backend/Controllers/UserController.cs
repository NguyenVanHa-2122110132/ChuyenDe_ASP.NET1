/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : Controller quản lý Người dùng hệ thống (User) - Lấy dữ liệu THẬT từ SQL
              - Index: Hiển thị danh sách thành viên (Admin, Sales, Customer)
              - Lưu ý: Không hiển thị mật khẩu (PasswordHash) ra giao diện
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data; // Quan trọng: Để dùng ApplicationDbContext
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        // 1. "Tiêm" ApplicationDbContext vào Controller
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 2. Viết Action Index() để lấy danh sách Users từ Database
        public IActionResult Index()
        {
            var users = _context.Users.ToList(); // Lấy dữ liệu THẬT từ SQL
            return View(users);
        }
    }
}