/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Controller quản lý Danh mục bài viết Blog (PostCategory)
              - Index : Hiển thị danh sách toàn bộ danh mục bài viết (gồm cả thông tin danh mục cha nếu có)
              - Create: Hiển thị form và lưu danh mục bài viết mới (có tải danh sách danh mục cha)
              - Edit  : Hiển thị form và cập nhật thông tin danh mục đã tồn tại
              - Delete: Xóa danh mục bài viết (kiểm tra ràng buộc bài viết và danh mục con trước khi xóa)
              - Phân quyền: Chỉ Administrator và Admin mới được thay đổi cấu trúc danh mục blog
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Thêm namespace để dùng [Authorize]
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào hệ thống quản lý danh mục blog
    public class PostCategoryController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public PostCategoryController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var data = _context.PostCategories
                .Include(pc => pc.Parent) // Tải thông tin danh mục cha để hiển thị phân cấp dạng cây (nếu có)
                .OrderBy(pc => pc.SortOrder) // Sắp xếp theo thứ tự ưu tiên hiển thị của Admin
                .ToList(); // Lấy toàn bộ danh mục bài viết ra list

            return View(data); // Truyền dữ liệu ra View quản trị
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách các danh mục đang hoạt động để làm dropdown chọn Danh mục cha (Parent)
            ViewBag.ParentCategories = _context.PostCategories.Where(pc => pc.IsActive).ToList();
            return View(); // Hiển thị form tạo mới danh mục bài viết
        }

        // ========== CREATE POST ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PostCategory model)
        {
            if (ModelState.IsValid) // Kiểm tra tính hợp lệ của dữ liệu nhập
            {
                model.CreatedAt = DateTime.UtcNow; // Thiết lập mốc thời gian tạo hiện tại

                _context.PostCategories.Add(model); // Thêm danh mục mới vào database
                _context.SaveChanges();             // Lưu thay đổi vào hệ thống

                TempData["SuccessMessage"] = "Thêm danh mục bài viết mới thành công!";
                return RedirectToAction("Index");   // Quay về danh sách quản lý
            }

            // Nếu lỗi dữ liệu, nạp lại danh sách danh mục cha cho dropdown hiển thị lại form
            ViewBag.ParentCategories = _context.PostCategories.Where(pc => pc.IsActive).ToList();
            return View(model);
        }

        // ========== EDIT GET ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var postCategory = _context.PostCategories.Find(id); // Tìm danh mục bài viết theo ID
            if (postCategory == null) return NotFound();        // Không tìm thấy trả về lỗi 404

            // Lấy danh sách danh mục cha, loại trừ chính nó (id) để tránh logic vòng lặp vô tận (cha chọn con làm cha)
            ViewBag.ParentCategories = _context.PostCategories
                .Where(pc => pc.IsActive && pc.Id != id)
                .ToList();

            return View(postCategory); // Hiển thị form edit đi kèm dữ liệu cũ
        }

        // ========== EDIT POST ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PostCategory model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                _context.PostCategories.Update(model); // Cập nhật thông tin danh mục vào database
                _context.SaveChanges();               // Lưu thay đổi vào hệ thống

                TempData["SuccessMessage"] = "Cập nhật danh mục bài viết thành công!";
                return RedirectToAction("Index");     // Quay về danh sách quản lý
            }

            // Nếu lỗi dữ liệu, nạp lại danh sách loại trừ chính nó ra
            ViewBag.ParentCategories = _context.PostCategories.Where(pc => pc.IsActive && pc.Id != model.Id).ToList();
            return View(model);
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var postCategory = _context.PostCategories
                .Include(pc => pc.Children)           // Tải kèm danh mục con
                .Include(pc => pc.PostPostCategories) // Tải kèm liên kết bài viết
                .FirstOrDefault(pc => pc.Id == id);   // Tìm danh mục theo ID khóa chính

            if (postCategory == null) return NotFound(); // Trả về lỗi 404 nếu không tìm thấy

            // Kiểm tra ràng buộc 1: Nếu danh mục này đang chứa các danh mục con bên trong
            if (postCategory.Children != null && postCategory.Children.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa! Danh mục này đang có các danh mục con bên trong. Hãy di dời hoặc xóa danh mục con trước.";
                return RedirectToAction("Index");
            }

            // Kiểm tra ràng buộc 2: Nếu danh mục này đang chứa bài viết (thông qua bảng trung gian)
            if (postCategory.PostPostCategories != null && postCategory.PostPostCategories.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa! Đang có bài viết thuộc danh mục này. Hãy xóa hoặc chuyển bài viết sang danh mục khác trước.";
                return RedirectToAction("Index");
            }

            _context.PostCategories.Remove(postCategory); // Xóa danh mục khỏi hệ thống database
            _context.SaveChanges();                      // Lưu thay đổi

            TempData["SuccessMessage"] = "Xóa danh mục bài viết thành công!";
            return RedirectToAction("Index"); // Quay về trang danh mục chính
        }
    }
}