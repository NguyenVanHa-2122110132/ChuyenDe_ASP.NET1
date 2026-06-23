/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Controller quản lý Giao hàng/Vận chuyển (Shipping)
              - Index         : Hiển thị danh sách toàn bộ các vận đơn giao hàng trên hệ thống
              - Details       : Xem chi tiết thông tin địa chỉ người nhận và lịch trình giao hàng
              - Edit          : Hiển thị form cập nhật thông tin nhà vận chuyển và ngày dự kiến giao
              - UpdateStatus  : Thay đổi trạng thái vận đơn (Đang giao, Thất bại, Hoàn thành...)
              - Phân quyền    : Chỉ Administrator và Admin mới được thao tác điều phối giao hàng
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
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào ban quản lý giao hàng
    public class ShippingController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public ShippingController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX (Danh sách vận đơn) ==========
        public IActionResult Index()
        {
            var data = _context.Shippings
                .Include(s => s.Order) // Tải kèm thông tin đơn hàng gốc
                .OrderByDescending(s => s.CreatedAt) // Sắp xếp vận đơn mới nhất lên trên đầu
                .ToList(); // Lấy toàn bộ danh sách vận đơn giao hàng từ database

            return View(data); // Truyền danh sách ra View quản trị vận chuyển
        }

        // ========== DETAILS (Xem chi tiết vận đơn) ==========
        public IActionResult Details(int id)
        {
            var shipping = _context.Shippings
                .Include(s => s.Order) // Tải kèm thông tin đơn hàng để đối chiếu
                .FirstOrDefault(s => s.Id == id); // Tìm kiếm vận đơn theo ID khóa chính

            if (shipping == null) return NotFound(); // Nếu không tồn tại trả về lỗi 404
            return View(shipping); // Truyền đối tượng vận đơn ra View chi tiết
        }

        // ========== EDIT GET (Giao diện cập nhật vận đơn) ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var shipping = _context.Shippings.Find(id); // Tìm vận đơn cần cập nhật thông tin vận chuyển
            if (shipping == null) return NotFound();   // Không tìm thấy trả về lỗi 404
            return View(shipping);                     // Hiển thị form cập nhật kèm dữ liệu cũ
        }

        // ========== EDIT POST (Xử lý cập nhật thông tin nhà vận chuyển) ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string shippingCarrier, string trackingNumber, DateTime? estimatedDeliveryDate, string? note)
        {
            var shipping = _context.Shippings.Find(id); // Đối chiếu thực thể trong database
            if (shipping == null) return NotFound();

            if (ModelState.IsValid)
            {
                shipping.ShippingCarrier = shippingCarrier;       // Cập nhật tên hãng vận chuyển (GHTK, GHN...)
                shipping.TrackingNumber = trackingNumber;         // Cập nhật mã định danh đơn hàng (Mã vận đơn)
                shipping.EstimatedDeliveryDate = estimatedDeliveryDate; // Cập nhật ngày dự kiến khách nhận hàng
                shipping.Note = note;                             // Cập nhật ghi chú phát sinh nếu có
                shipping.UpdatedAt = DateTime.UtcNow;             // Cập nhật mốc thời gian sửa đổi gần nhất

                _context.Shippings.Update(shipping); // Cập nhật thông tin vào hệ thống
                _context.SaveChanges();             // Lưu thay đổi vào database

                TempData["SuccessMessage"] = "Cập nhật thông tin hãng vận chuyển thành công!";
                return RedirectToAction("Details", new { id = shipping.Id }); // Quay lại trang xem chi tiết
            }
            return View(shipping); // Nếu dữ liệu không hợp lệ hiển thị lại giao diện form
        }

        // ========== UPDATE STATUS (Cập nhật tiến độ giao hàng) ==========
        [HttpPost]
        public IActionResult UpdateStatus(int id, int statusId)
        {
            var shipping = _context.Shippings.Find(id); // Tìm kiếm vận đơn cần cập nhật trạng thái trong database
            if (shipping == null) return NotFound();   // Không tồn tại trả về lỗi 404

            shipping.Status = (ShippingStatus)statusId; // Chuyển đổi ID số nguyên sang Enum trạng thái vận chuyển tương ứng
            shipping.UpdatedAt = DateTime.UtcNow;       // Lưu mốc thời gian cập nhật tiến độ

            // Nếu trạng thái được chuyển sang "Delivered" (Đã giao hàng thành công)
            if (shipping.Status == ShippingStatus.Delivered)
            {
                shipping.DeliveredAt = DateTime.UtcNow; // Tự động ghi nhận mốc thời gian giao hàng thành công thực tế
            }

            _context.Shippings.Update(shipping); // Cập nhật thực thể vào database
            _context.SaveChanges();             // Lưu thay đổi vào hệ thống

            TempData["SuccessMessage"] = "Cập nhật trạng thái tiến độ giao đơn hàng thành công!";
            return RedirectToAction("Index"); // Quay về danh mục quản lý giao hàng tổng
        }
    }
}