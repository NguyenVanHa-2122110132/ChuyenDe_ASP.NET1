/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 01/06/2026
    Mô tả    : Controller quản lý Đánh giá sản phẩm (Review)
              - Index : Hiển thị danh sách đánh giá sản phẩm (Admin quản lý)
              - Create: Lưu đánh giá mới từ khách hàng sau khi mua sản phẩm
              - Approve: Admin duyệt đánh giá hợp lệ để hiển thị lên trang sản phẩm
              - Delete: Xóa đánh giá khỏi hệ thống
              - Phân quyền: Chỉ Administrator và Admin mới được vào quản lý và duyệt đánh giá
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
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public ReviewController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var data = _context.Reviews
                .Include(r => r.Product) // Lấy kèm thông tin sản phẩm được đánh giá
                .Include(r => r.Customer)    // Lấy kèm thông tin tài khoản người đánh giá
                .OrderByDescending(r => r.CreatedAt) // Sắp xếp đánh giá mới nhất lên đầu
                .ToList();

            return View(data); // Truyền danh sách đánh giá ra View quản trị
        }

        // ========== CREATE POST (Dành cho Client gửi đánh giá sản phẩm) ==========
        [HttpPost]
        [AllowAnonymous] // Cho phép khách hàng gửi đánh giá (Thường sẽ kiểm tra đăng nhập ở View)
        public IActionResult Create(Review model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                // Kiểm tra xem người dùng đã thực sự mua sản phẩm này chưa để xác thực mua hàng
                if (model.OrderDetailId != null)
                {
                    model.IsVerifiedPurchase = true; // Đánh dấu đã mua hàng thực tế
                }

                model.CreatedAt = DateTime.UtcNow; // Gán thời gian tạo hiện tại
                model.UpdatedAt = DateTime.UtcNow; // Gán thời gian cập nhật hiện tại
                model.IsApproved = false;          // Mặc định đánh giá mới phải chờ Admin duyệt

                _context.Reviews.Add(model); // Thêm đánh giá mới vào database
                _context.SaveChanges();      // Lưu thay đổi vào database

                // Quay lại trang chi tiết sản phẩm vừa đánh giá ở phía Client
                return RedirectToAction("Details", "Product", new { id = model.ProductId });
            }

            // Nếu dữ liệu bị lỗi, quay về trang chi tiết sản phẩm cũ
            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        // ========== APPROVE (Admin duyệt đánh giá) ==========
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id); // Tìm đánh giá theo ID
            if (review == null) return NotFound(); // Không tìm thấy trả về 404

            review.IsApproved = true;           // Chuyển trạng thái sang đã duyệt
            review.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian chỉnh sửa cuối

            _context.Reviews.Update(review); // Cập nhật thông tin vào database
            _context.SaveChanges();          // Lưu thay đổi vào database

            return RedirectToAction("Index"); // Quay lại trang danh sách quản lý đánh giá
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var review = _context.Reviews
                .Include(r => r.ReviewImages) // Load kèm danh sách ảnh của đánh giá này (nếu có)
                .FirstOrDefault(r => r.Id == id); // Tìm đánh giá theo ID

            if (review == null) return NotFound(); // Không tìm thấy trả về 404

            // Xóa toàn bộ ảnh đính kèm của đánh giá này trong database trước để tránh lỗi ràng buộc (Cascade)
            if (review.ReviewImages != null && review.ReviewImages.Any())
            {
                _context.ReviewImages.RemoveRange(review.ReviewImages);
            }

            _context.Reviews.Remove(review); // Xóa đánh giá chính khỏi database
            _context.SaveChanges();          // Lưu thay đổi vào database

            return RedirectToAction("Index"); // Quay lại trang danh sách quản lý đánh giá
        }
    }
}