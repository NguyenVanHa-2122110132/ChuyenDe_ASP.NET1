/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Controller quản lý Thẻ bài viết Blog (Tag & PostTag)
              - Index : Hiển thị danh sách toàn bộ các thẻ/tags hiện có trên hệ thống
              - Create: Hiển thị form và lưu thẻ mới vào database (kiểm tra trùng lặp tên thẻ)
              - Edit  : Hiển thị form và cập nhật tên hoặc đường dẫn Slug của thẻ
              - Delete: Xóa thẻ khỏi hệ thống (kiểm tra ràng buộc bài viết trước khi thực hiện)
              - Phân quyền: Chỉ Administrator và Admin mới được can thiệp chỉnh sửa thẻ tag
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
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào hệ thống quản lý thẻ tag
    public class TagController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public TagController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var data = _context.Tags
                .Include(t => t.PostTags) // Tải kèm bảng trung gian để đếm xem thẻ này đang được gắn cho bao nhiêu bài viết
                .OrderByDescending(t => t.CreatedAt) // Sắp xếp thẻ mới tạo lên trên đầu
                .ToList(); // Lấy toàn bộ danh sách thẻ tag từ database

            return View(data); // Truyền danh sách ra View quản trị
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Hiển thị form thêm thẻ tag mới
        }

        // ========== CREATE POST ==========
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống tấn công giả mạo yêu cầu (CSRF)
        public IActionResult Create(Tag model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu nhập từ form hợp lệ
            {
                // Kiểm tra xem tên thẻ hoặc định danh Slug này đã tồn tại trong database chưa (tránh trùng lặp)
                bool isExist = _context.Tags.Any(t => t.Name == model.Name || t.Slug == model.Slug);
                if (isExist)
                {
                    ModelState.AddModelError("Name", "Tên thẻ hoặc đường dẫn Slug này đã tồn tại!");
                    return View(model);
                }

                model.CreatedAt = DateTime.UtcNow; // Thiết lập mốc thời gian tạo hiện tại
                _context.Tags.Add(model);           // Thêm thực thể thẻ mới vào database
                _context.SaveChanges();             // Lưu thay đổi vào hệ thống database

                TempData["SuccessMessage"] = "Thêm thẻ bài viết mới thành công!";
                return RedirectToAction("Index");   // Điều hướng quay về trang danh sách
            }
            return View(model); // Nếu lỗi dữ liệu thì hiển thị lại form kèm thông báo
        }

        // ========== EDIT GET ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var tag = _context.Tags.Find(id); // Tìm kiếm thẻ bài viết theo ID khóa chính
            if (tag == null) return NotFound(); // Nếu không tìm thấy trả về lỗi 404
            return View(tag);                   // Hiển thị form sửa đổi đi kèm dữ liệu cũ của thẻ
        }

        // ========== EDIT POST ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Tag model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                _context.Tags.Update(model); // Cập nhật thông tin sửa đổi của thẻ vào database
                _context.SaveChanges();     // Lưu thay đổi vào hệ thống database

                TempData["SuccessMessage"] = "Cập nhật thông tin thẻ bài viết thành công!";
                return RedirectToAction("Index"); // Điều hướng quay về trang danh sách
            }
            return View(model); // Nếu lỗi hiển thị lại giao diện form edit
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var tag = _context.Tags
                .Include(t => t.PostTags) // Tải kèm danh sách liên kết bài viết để kiểm tra ràng buộc dữ liệu
                .FirstOrDefault(t => t.Id == id); // Tìm kiếm thẻ theo ID

            if (tag == null) return NotFound(); // Trả về lỗi 404 nếu không tìm thấy bản ghi

            // Kiểm tra ràng buộc: Nếu thẻ này đang được gắn vào ít nhất một bài viết (thông qua bảng trung gian)
            if (tag.PostTags != null && tag.PostTags.Any())
            {
                // Lưu thông báo lỗi vào TempData để hiển thị cảnh báo ngoài View Index
                TempData["ErrorMessage"] = "Không thể xóa! Thẻ này đang được gắn vào các bài viết trên Blog. Vui lòng gỡ thẻ ra khỏi bài viết trước khi xóa.";
                return RedirectToAction("Index");
            }

            _context.Tags.Remove(tag); // Tiến hành xóa thẻ ra khỏi hệ thống database
            _context.SaveChanges();    // Lưu thay đổi dứt điểm

            TempData["SuccessMessage"] = "Xóa thẻ bài viết thành công!";
            return RedirectToAction("Index"); // Quay về trang danh mục quản lý thẻ tag tổng
        }
    }
}