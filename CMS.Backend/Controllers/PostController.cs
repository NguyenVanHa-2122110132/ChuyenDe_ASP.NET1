/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : Controller quản lý Bài viết/Điện thoại (Post) - Lấy dữ liệu THẬT từ SQL
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data; // Quan trọng: Để dùng ApplicationDbContext
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        // 1. "Tiêm" ApplicationDbContext vào Controller (Constructor Injection)
        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 2. Viết Action Index() để lấy danh sách bài viết từ Database
        public IActionResult Index()
        {
            var posts = _context.Posts.ToList(); // Lấy tất cả bài viết THẬT
            return View(posts);
        }

        // Hàm Details (giữ nguyên logic tìm kiếm nhưng đổi sang dùng _context)
        public IActionResult Details(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null) return NotFound();
            return View(post);
        }
    }
}