/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Controller độc lập quản lý mối liên kết Giữa Bài viết và Thẻ tag (PostTag)
              - Index   : Hiển thị danh sách toàn bộ các liên kết Post-Tag trong database
              - Create  : Gán một hoặc nhiều thẻ tag vào bài viết cụ thể
              - Delete  : Gỡ bỏ thẻ tag ra khỏi bài viết (Xóa bản ghi trung gian)
              - Phân quyền: Chỉ Administrator và Admin mới được can thiệp cấu trúc liên kết nội dung Blog
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
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào điều phối liên kết thẻ
    public class PostTagController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public PostTagController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX (Danh sách tất cả các liên kết Post - Tag) ==========
        public IActionResult Index(int? postId)
        {
            // Truy vấn lấy danh sách bảng trung gian PostTag, nạp kèm dữ liệu Post và Tag liên quan
            var query = _context.PostTags
                .Include(pt => pt.Post)
                .Include(pt => pt.Tag)
                .AsQueryable();

            // Nếu Admin muốn lọc xem danh sách thẻ của riêng 1 bài viết
            if (postId.HasValue)
            {
                query = query.Where(pt => pt.PostId == postId.Value);
                ViewBag.SelectedPostId = postId.Value; // Đẩy ngược ID bài viết ra View để phục vụ tính năng gán thẻ nhanh
            }

            var data = query.ToList();
            return View(data); // Truyền dữ liệu ra View quản trị liên kết
        }

        // ========== CREATE POST (Gán thẻ tag vào bài viết) ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int postId, int tagId)
        {
            // Kiểm tra ràng buộc: Bài viết này đã được gán thẻ này trước đó chưa (Tránh trùng khóa chính kép)
            bool isExist = _context.PostTags.Any(pt => pt.PostId == postId && pt.TagId == tagId);

            if (isExist)
            {
                TempData["ErrorMessage"] = "Thẻ này đã được gán cho bài viết từ trước rồi!";
                return RedirectToAction("Index", new { postId = postId });
            }

            // Tạo mới bản ghi liên kết trung gian PostTag
            var postTag = new PostTag
            {
                PostId = postId,
                TagId = tagId
            };

            if (ModelState.IsValid)
            {
                _context.PostTags.Add(postTag); // Thêm liên kết mới vào database
                _context.SaveChanges();         // Lưu lại thay đổi

                TempData["SuccessMessage"] = "Gán thẻ tag cho bài viết thành công!";
                return RedirectToAction("Index", new { postId = postId });
            }

            return RedirectToAction("Index");
        }

        // ========== DELETE (Gỡ thẻ tag ra khỏi bài viết) ==========
        [HttpPost]
        public IActionResult Delete(int postId, int tagId)
        {
            // Tìm chính xác bản ghi liên kết trung gian dựa theo bộ đôi khóa ngoại (PostId và TagId)
            var postTag = _context.PostTags
                .FirstOrDefault(pt => pt.PostId == postId && pt.TagId == tagId);

            if (postTag == null) return NotFound(); // Nếu không tìm thấy liên kết, trả về trang lỗi 404

            _context.PostTags.Remove(postTag); // Tiến hành xóa bản ghi trung gian khỏi database
            _context.SaveChanges();            // Lưu thay đổi dứt điểm

            TempData["SuccessMessage"] = "Đã gỡ bỏ thẻ tag ra khỏi bài viết thành công!";

            // Điều hướng quay lại danh sách liên kết của chính bài viết đó
            return RedirectToAction("Index", new { postId = postId });
        }
    }
}