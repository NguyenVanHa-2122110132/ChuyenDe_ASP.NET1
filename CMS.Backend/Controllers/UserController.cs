/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : Controller quản lý Người dùng hệ thống (User)
              - Index  : Hiển thị danh sách tất cả thành viên
              - Create : Hiển thị form và lưu thành viên mới vào database
              - Edit   : Hiển thị form và cập nhật thông tin thành viên
                         Nếu không nhập mật khẩu mới thì giữ nguyên mật khẩu cũ
              - Delete : Xóa thành viên theo ID
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public UserController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var users = _context.Users.ToList(); // Lấy toàn bộ danh sách thành viên từ database
            return View(users);                  // Truyền danh sách ra View
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Hiển thị form thêm mới
        }

        // ========== CREATE POST ==========
        [HttpPost]
        public IActionResult Create(User model)
        {
            // Kiểm tra xem tên đăng nhập đã tồn tại chưa
            var checkExist = _context.Users.Any(u => u.Username == model.Username);
            if (checkExist)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã có người dùng!");
                return View(model);
            }

            _context.Users.Add(model);        // Thêm thành viên mới vào database
            _context.SaveChanges();            // Lưu thay đổi
            return RedirectToAction("Index");  // Quay về danh sách
        }

        // ========== EDIT GET ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id); // Tìm thành viên theo ID
            if (user == null) return NotFound(); // Nếu không tìm thấy thì trả về 404
            return View(user); // Hiển thị form với dữ liệu cũ
        }

        // ========== EDIT POST ==========
        [HttpPost]
        public IActionResult Edit(User model, string NewPassword)
        {
            // Lấy thành viên cũ từ database (AsNoTracking để tránh conflict với EF)
            var existingUser = _context.Users.AsNoTracking()
                                             .FirstOrDefault(u => u.Id == model.Id);
            if (existingUser == null) return NotFound();

            // Cập nhật các thông tin mới
            existingUser.FullName = model.FullName; // Cập nhật họ tên
            existingUser.Role = model.Role;          // Cập nhật quyền hạn

            // Nếu nhập mật khẩu mới thì dùng mới, không thì giữ mật khẩu cũ
            if (!string.IsNullOrEmpty(NewPassword))
                model.PasswordHash = NewPassword;
            else
                model.PasswordHash = existingUser.PasswordHash;

            _context.Users.Update(model);     // Cập nhật vào database
            _context.SaveChanges();            // Lưu thay đổi
            return RedirectToAction("Index");  // Quay về danh sách
        }

        // ========== DELETE ==========
        // Dùng GET để thẻ <a> gọi được trực tiếp (giống PostController)
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id); // Tìm thành viên theo ID
            if (user != null)
            {
                _context.Users.Remove(user); // Xóa thành viên khỏi database
                _context.SaveChanges();       // Lưu thay đổi
            }
            return RedirectToAction("Index"); // Quay về danh sách
        }
    }
}