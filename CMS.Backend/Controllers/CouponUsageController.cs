/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 01/06/2026
    Mô tả    : Controller quản lý Lịch sử sử dụng mã giảm giá (CouponUsage)
              - Index : Hiển thị danh sách toàn bộ các lượt áp dụng mã giảm giá trên hệ thống
              - Details: Xem chi tiết một lượt sử dụng mã giảm giá cụ thể
              - Delete: Hủy ghi nhận lịch sử áp dụng mã (thường dùng khi hoàn tác/hủy đơn hàng)
              - Phân quyền: Chỉ Administrator và Admin mới được vào xem báo cáo, lịch sử
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
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào xem lịch sử khuyến mãi
    public class CouponUsageController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public CouponUsageController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX (Danh sách lịch sử sử dụng) ==========
        public IActionResult Index()
        {
            var data = _context.CouponUsages
                .Include(cu => cu.Coupon) // Load thông tin mã giảm giá (Code, loại giảm...)
                .Include(cu => cu.Customer)   // Load thông tin tài khoản người dùng đã áp dụng mã
                .Include(cu => cu.Order)  // Load thông tin đơn hàng được áp dụng mã
                .OrderByDescending(cu => cu.UsedAt) // Sắp xếp lượt sử dụng mới nhất lên đầu
                .ToList(); // Lấy toàn bộ danh sách lịch sử từ database

            return View(data); // Truyền danh sách ra View báo cáo thống kê
        }

        // ========== DETAILS (Xem chi tiết lượt sử dụng) ==========
        public IActionResult Details(int id)
        {
            var usage = _context.CouponUsages
                .Include(cu => cu.Coupon)
                .Include(cu => cu.Customer)
                .Include(cu => cu.Order)
                .FirstOrDefault(cu => cu.Id == id); // Tìm kiếm lịch sử sử dụng theo ID

            if (usage == null) return NotFound(); // Nếu không tồn tại bản ghi trả về trang lỗi 404
            return View(usage); // Truyền đối tượng tìm được ra View chi tiết
        }

        // ========== DELETE (Hủy/Xóa lịch sử sử dụng mã) ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var usage = _context.CouponUsages
                .Include(cu => cu.Coupon) // Khởi tạo kèm Coupon để cập nhật số lần dùng
                .FirstOrDefault(cu => cu.Id == id); // Tìm bản ghi lịch sử sử dụng theo ID

            if (usage == null) return NotFound(); // Không tìm thấy trả về lỗi 404

            // Hoàn tác dữ liệu: Giảm số lần đã sử dụng (UsedCount) của mã giảm giá đi 1 đơn vị
            if (usage.Coupon != null && usage.Coupon.UsedCount > 0)
            {
                usage.Coupon.UsedCount--;
                _context.Coupons.Update(usage.Coupon); // Cập nhật lại số lần dùng của Coupon
            }

            _context.CouponUsages.Remove(usage); // Xóa bản ghi lịch sử sử dụng này khỏi database
            _context.SaveChanges();             // Lưu mọi thay đổi vào database

            TempData["SuccessMessage"] = "Hủy bỏ lượt áp dụng mã giảm giá thành công!";
            return RedirectToAction("Index"); // Quay về trang danh sách lịch sử
        }
    }
}