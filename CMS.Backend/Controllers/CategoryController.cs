/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 14/06/2026
    Mô tả    : Controller quản lý Danh mục (Category)
              - Index : Hiển thị danh sách danh mục
              - Create: Hiển thị form và lưu danh mục mới vào database
                        Bổ sung validate Gender bắt buộc để danh mục
                        hiển thị đúng tab trên frontend React (/thoi-trang)
              - Edit  : Hiển thị form và cập nhật danh mục đã có
              - Delete: Xóa danh mục (có kiểm tra ràng buộc bài viết trước khi xóa)
              - Phân quyền: Chỉ Administrator và Admin mới được thêm/sửa/xóa
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào
    public class CategoryController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public CategoryController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var data = _context.Categories.ToList(); // Lấy toàn bộ danh mục từ database
            return View(data);                        // Truyền danh sách ra View
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Hiển thị form thêm danh mục mới
        }

        // ========== CREATE POST ==========
        [HttpPost]
        public IActionResult Create(Category model)
        {
            // Kiểm tra Gender bắt buộc phải chọn
            // Vì frontend React lọc danh mục theo gender (?gender=nam/nu/treem)
            // Nếu không có gender → danh mục sẽ không hiện trên trang /thoi-trang
            if (string.IsNullOrEmpty(model.Gender))
            {
                ModelState.AddModelError("Gender", "Vui lòng chọn giới tính cho danh mục!");
            }

            if (ModelState.IsValid) // Kiểm tra toàn bộ dữ liệu hợp lệ
            {
                model.IsActive = true;              // Mặc định hiển thị ngay khi thêm mới
                _context.Categories.Add(model);     // Thêm danh mục mới vào database
                _context.SaveChanges();              // Lưu thay đổi vào SQL Server

                // Thông báo thành công hiển thị ở trang Index
                TempData["SuccessMessage"] = $"✅ Đã thêm danh mục '{model.Name}' thành công!";
                return RedirectToAction("Index");    // Quay về danh sách
            }
            return View(model); // Nếu có lỗi thì hiển thị lại form kèm thông báo lỗi
        }

        // ========== EDIT GET ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id); // Tìm danh mục theo ID
            if (category == null) return NotFound();       // Không tìm thấy trả về 404
            return View(category); // Hiển thị form với dữ liệu cũ
        }

        // ========== EDIT POST ==========
        [HttpPost]
        public IActionResult Edit(Category model)
        {
            // Kiểm tra Gender bắt buộc khi cập nhật
            if (string.IsNullOrEmpty(model.Gender))
            {
                ModelState.AddModelError("Gender", "Vui lòng chọn giới tính cho danh mục!");
            }

            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                _context.Categories.Update(model); // Cập nhật danh mục trong database
                _context.SaveChanges();             // Lưu thay đổi

                TempData["SuccessMessage"] = $"✅ Đã cập nhật danh mục '{model.Name}' thành công!";
                return RedirectToAction("Index");   // Quay về danh sách
            }
            return View(model); // Nếu lỗi thì hiển thị lại form
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id); // Tìm danh mục theo ID
            if (category == null) return NotFound(); // Không tìm thấy trả về 404

            // Kiểm tra xem danh mục có bài viết không trước khi xóa
            bool cobaiviet = _context.Posts.Any(p => p.CategoryId == id);
            if (cobaiviet)
            {
                // Nếu có bài viết thì không cho xóa, báo lỗi
                TempData["ErrorMessage"] = "Không thể xóa! Danh mục này đang có bài viết bên trong. Hãy xóa hết bài viết trước.";
                return RedirectToAction("Index");
            }

            _context.Categories.Remove(category); // Xóa danh mục khỏi database
            _context.SaveChanges();                // Lưu thay đổi
            return RedirectToAction("Index");      // Quay về danh sách
        }
    }
}