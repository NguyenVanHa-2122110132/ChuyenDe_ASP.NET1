/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Controller quản lý Thanh toán (Payment)
              - Index         : Hiển thị danh sách toàn bộ các giao dịch thanh toán trên hệ thống
              - Details       : Xem chi tiết thông tin thanh toán (mã giao dịch, phản hồi từ gateway)
              - ProcessPayment: Xử lý tạo mới lượt thanh toán (khi người dùng tiến hành thanh toán)
              - UpdateStatus  : Admin cập nhật trạng thái thanh toán (Hoàn thành, Thất bại, Hoàn tiền)
              - Phân quyền    : Chỉ Administrator và Admin mới được kiểm duyệt và thay đổi trạng thái tài chính
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
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào ban quản lý thanh toán
    public class PaymentController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public PaymentController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX (Danh sách giao dịch thanh toán) ==========
        public IActionResult Index()
        {
            var data = _context.Payments
                .Include(p => p.Order) // Tải kèm thông tin đơn hàng liên kết
                .OrderByDescending(p => p.CreatedAt) // Sắp xếp giao dịch mới nhất lên trên đầu
                .ToList(); // Lấy toàn bộ danh sách thanh toán từ database

            return View(data); // Truyền danh sách ra View quản trị tài chính
        }

        // ========== DETAILS (Xem chi tiết giao dịch) ==========
        public IActionResult Details(int id)
        {
            var payment = _context.Payments
                .Include(p => p.Order) // Tải kèm thông tin đơn hàng để đối chiếu giá trị đơn
                .FirstOrDefault(p => p.Id == id); // Tìm kiếm giao dịch thanh toán theo ID

            if (payment == null) return NotFound(); // Nếu không tồn tại trả về lỗi 404
            return View(payment); // Truyền đối tượng giao dịch ra View chi tiết để kiểm tra mã và chuỗi JSON gateway
        }

        // ========== PROCESS PAYMENT (Tạo mới lượt thanh toán từ Client) ==========
        [HttpPost]
        [AllowAnonymous] // Cho phép hệ thống hoặc người mua hàng tạo yêu cầu thanh toán mà không cần quyền Admin
        public IActionResult ProcessPayment(Payment model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                model.CreatedAt = DateTime.UtcNow; // Gán thời gian tạo bản ghi hiện tại
                model.UpdatedAt = DateTime.UtcNow; // Gán thời gian cập nhật bản ghi hiện tại

                // Nếu trạng thái gửi sang ghi nhận là đã hoàn thành luôn (ví dụ: thanh toán qua cổng tự động thành công)
                if (model.Status == PaymentStatus.Completed)
                {
                    model.PaidAt = DateTime.UtcNow; // Ghi nhận mốc thời gian hoàn tất thanh toán thực tế
                }

                _context.Payments.Add(model); // Thêm bản ghi thanh toán vào database
                _context.SaveChanges();      // Lưu lại vào hệ thống database

                return RedirectToAction("Details", "Order", new { id = model.OrderId }); // Điều hướng về xem đơn hàng
            }
            return RedirectToAction("Index", "Home"); // Nếu lỗi cấu trúc điều hướng về trang chủ
        }

        // ========== UPDATE STATUS (Admin duyệt trạng thái giao dịch thủ công) ==========
        [HttpPost]
        public IActionResult UpdateStatus(int id, int statusId)
        {
            var payment = _context.Payments.Find(id); // Tìm kiếm giao dịch cần cập nhật trong database
            if (payment == null) return NotFound();   // Không tồn tại trả về lỗi 404

            payment.Status = (PaymentStatus)statusId; // Chuyển đổi số nguyên ID nhận về sang dạng Enum trạng thái tương ứng
            payment.UpdatedAt = DateTime.UtcNow;       // Lưu mốc thời gian cập nhật chỉnh sửa

            // Trường hợp 1: Chuyển sang trạng thái "Completed" (Thanh toán thành công)
            if (payment.Status == PaymentStatus.Completed)
            {
                payment.PaidAt = DateTime.UtcNow; // Tự động ghi nhận mốc thời gian thanh toán thành công
            }
            // Trường hợp 2: Chuyển sang các trạng thái khác (Failed, Cancelled, Refunded)
            else
            {
                payment.PaidAt = null; // Reset mốc thời gian hoàn tất nếu không phải trạng thái thành công
            }

            _context.Payments.Update(payment); // Cập nhật dữ liệu thực thể vào database
            _context.SaveChanges();           // Lưu các thay đổi vào hệ thống

            TempData["SuccessMessage"] = "Cập nhật trạng thái giao dịch thanh toán thành công!";
            return RedirectToAction("Index"); // Quay về danh mục quản lý lịch sử thanh toán tổng
        }
    }
}