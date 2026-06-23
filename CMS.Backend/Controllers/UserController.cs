/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : Controller quản lý Người dùng hệ thống (User)
              - Index  : Chỉ hiển thị NHÂN VIÊN (không hiện Customer)
              - Create : Thêm thành viên mới (chỉ Administrator)
              - Edit   : Sửa thông tin thành viên (chỉ Administrator)
              - Delete : Xóa thành viên (chỉ Administrator)
              [BẢO MẬT] Delete dùng POST, không dùng GET
              [BẢO MẬT] Edit kiểm tra username trùng
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var users = _context.Users
                .Where(u => u.Role != "Customer")
                .ToList();
            return View(users);
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ========== CREATE POST ==========
        [HttpPost]
        [ValidateAntiForgeryToken] //  Chống CSRF
        public IActionResult Create(User model)
        {
            var checkExist = _context.Users.Any(u => u.Username == model.Username);
            if (checkExist)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã có người dùng!");
                return View(model);
            }

            var hasher = new PasswordHasher<object>();
            model.PasswordHash = hasher.HashPassword(null, model.PasswordHash);
            _context.Users.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // ========== EDIT GET ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // ========== EDIT POST ==========
        [HttpPost]
        [ValidateAntiForgeryToken] // ✅ Chống CSRF
        public IActionResult Edit(User model, string NewPassword)
        {
            var existingUser = _context.Users.AsNoTracking()
                                             .FirstOrDefault(u => u.Id == model.Id);
            if (existingUser == null) return NotFound();

            // ✅ Kiểm tra username trùng với người khác
            var duplicateUsername = _context.Users.Any(u =>
                u.Username == model.Username && u.Id != model.Id);
            if (duplicateUsername)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã có người dùng khác!");
                return View(model);
            }

            existingUser.FullName = model.FullName;
            existingUser.Role = model.Role;

            if (!string.IsNullOrEmpty(model.PasswordHash))
            {
                var hasher = new PasswordHasher<object>();
                existingUser.PasswordHash = hasher.HashPassword(null, model.PasswordHash);
            }

            _context.Users.Update(existingUser);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // ========== DELETE ==========
        // [BẢO MẬT] Dùng POST thay GET để tránh bị xóa user qua link độc hại
        [HttpPost]
        [ValidateAntiForgeryToken] //  Chống CSRF
        public IActionResult Delete(int id)
        {
            // Không cho xóa chính mình
            var currentUsername = User.Identity?.Name;
            var user = _context.Users.Find(id);

            if (user == null) return NotFound();

            if (user.Username == currentUsername)
            {
                TempData["Error"] = "Bạn không thể tự xóa tài khoản của chính mình!";
                return RedirectToAction("Index");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}