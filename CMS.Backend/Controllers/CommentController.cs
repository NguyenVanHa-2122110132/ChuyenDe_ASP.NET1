/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 01/06/2026
    Mô tả    : Controller quản lý Bình luận (Comment)
              - Index : Hiển thị danh sách bình luận (Admin quản lý)
              - Create: Lưu bình luận mới từ phía người dùng/khách
              - Approve: Admin duyệt bình luận để hiển thị lên website
              - Delete: Xóa bình luận khỏi hệ thống
              - Phân quyền: Chỉ Administrator và Admin mới được quản lý và duyệt bình luận
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Authorization; // Thêm namespace để dùng [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data.Entities;

namespace CMS.Backend.Controllers
{
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào quản lý
    public class CommentController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public CommentController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var data = _context.Comments
                .Include(c => c.Post) // Lấy thông tin bài viết kèm theo bình luận
                .Include(c => c.User) // Lấy thông tin người dùng (nếu có) kèm theo bình luận
                .OrderByDescending(c => c.CreatedAt) // Sắp xếp bình luận mới nhất lên đầu
                .ToList();

            return View(data); // Truyền danh sách ra View quản trị
        }

        // ========== CREATE POST (Dành cho Client gửi bình luận) ==========
        [HttpPost]
        [AllowAnonymous] // Cho phép tất cả người dùng hoặc khách vãng lai gửi bình luận không cần đăng nhập
        public IActionResult Create(Comment model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                model.CreatedAt = DateTime.UtcNow; // Gán thời gian tạo hiện tại
                model.UpdatedAt = DateTime.UtcNow; // Gán thời gian cập nhật hiện tại
                model.IsApproved = false;          // Mặc định bình luận mới ở trạng thái chờ Admin duyệt

                _context.Comments.Add(model); // Thêm bình luận mới vào database
                _context.SaveChanges();      // Lưu thay đổi vào database

                // Sau khi bình luận xong, điều hướng quay lại trang chi tiết của bài viết đó
                return RedirectToAction("Details", "Post", new { id = model.PostId });
            }

            // Nếu dữ liệu không hợp lệ, quay về trang bài viết cũ
            return RedirectToAction("Details", "Post", new { id = model.PostId });
        }

        // ========== APPROVE (Admin duyệt bình luận) ==========
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id); // Tìm bình luận theo ID
            if (comment == null) return NotFound(); // Không tìm thấy trả về 404

            comment.IsApproved = true;          // Đổi trạng thái thành đã duyệt
            comment.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian chỉnh sửa

            _context.Comments.Update(comment); // Cập nhật thông tin vào database
            _context.SaveChanges();           // Lưu thay đổi vào database

            return RedirectToAction("Index"); // Quay về danh sách quản lý
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id); // Tìm bình luận theo ID
            if (comment == null) return NotFound(); // Không tìm thấy trả về 404

            // Kiểm tra xem bình luận này có các bình luận con (phản hồi / reply) hay không trước khi xóa
            bool coBinhLuanCon = _context.Comments.Any(c => c.ParentCommentId == id);
            if (coBinhLuanCon)
            {
                // Nếu có bình luận con thì không cho xóa, thông báo lỗi cho Admin
                TempData["ErrorMessage"] = "Không thể xóa! Bình luận này đang có các phản hồi bên trong. Hãy xóa các phản hồi trước.";
                return RedirectToAction("Index");
            }

            _context.Comments.Remove(comment); // Xóa bình luận khỏi database
            _context.SaveChanges();           // Lưu thay đổi vào database
            return RedirectToAction("Index"); // Quay về danh sách quản lý
        }
    }
}