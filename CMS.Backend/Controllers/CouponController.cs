/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 01/06/2026
    Mô tả    : Controller quản lý Mã giảm giá (Coupon)
              - Index : Hiển thị danh sách các mã khuyến mãi hiện có
              - Create: Hiển thị form và lưu mã giảm giá mới vào database
              - Edit  : Hiển thị form và cập nhật thông tin chi tiết của mã giảm giá
              - Delete: Xóa mã giảm giá (có kiểm tra lịch sử sử dụng trước khi thực hiện)
              - Phân quyền: Chỉ Administrator và Admin mới được thao tác quản lý khuyến mãi
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
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào hệ thống khuyến mãi
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public CouponController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var data = _context.Coupons
                .OrderByDescending(c => c.CreatedAt) // Sắp xếp mã mới tạo lên trên đầu
                .ToList(); // Lấy toàn bộ danh sách mã giảm giá từ database

            return View(data); // Truyền danh sách ra View hiển thị
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Hiển thị form thêm mã giảm giá mới
        }

        // ========== CREATE POST ==========
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống tấn công giả mạo yêu cầu (CSRF)
        public IActionResult Create(Coupon model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu nhập từ form hợp lệ
            {
                // Kiểm tra xem mã giảm giá này đã tồn tại trong database chưa (tránh trùng mã Code)
                bool IsExistCode = _context.Coupons.Any(c => c.Code == model.Code);
                if (IsExistCode)
                {
                    ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại trong hệ thống!");
                    return View(model);
                }

                model.CreatedAt = DateTime.UtcNow; // Thiết lập ngày tạo bản ghi hiện tại
                _context.Coupons.Add(model);        // Thêm đối tượng mới vào database
                _context.SaveChanges();            // Lưu thay đổi vào database
                return RedirectToAction("Index");  // Quay về trang danh sách
            }
            return View(model); // Nếu dữ liệu không hợp lệ thì hiển thị lại form kèm thông báo lỗi
        }

        // ========== EDIT GET ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var coupon = _context.Coupons.Find(id); // Tìm mã giảm giá theo ID
            if (coupon == null) return NotFound();   // Không tìm thấy trả về trang lỗi 404
            return View(coupon);                     // Hiển thị form sửa kèm dữ liệu cũ
        }

        // ========== EDIT POST ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Coupon model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                _context.Coupons.Update(model); // Cập nhật thông tin mã giảm giá trong database
                _context.SaveChanges();         // Lưu thay đổi vào hệ thống
                return RedirectToAction("Index"); // Quay về trang danh sách
            }
            return View(model); // Nếu lỗi thì hiển thị lại form để Admin sửa đổi
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var coupon = _context.Coupons
                .Include(c => c.CouponUsages) // Load kèm dữ liệu lịch sử sử dụng để kiểm tra ràng buộc
                .FirstOrDefault(c => c.Id == id); // Tìm kiếm mã giảm giá theo ID

            if (coupon == null) return NotFound(); // Nếu không tồn tại trả về lỗi 404

            // Kiểm tra ràng buộc: Nếu mã này đã có khách hàng áp dụng mua hàng thì không được xóa
            bool daSuDung = coupon.CouponUsages != null && coupon.CouponUsages.Any();
            if (daSuDung)
            {
                // Lưu thông báo lỗi vào TempData để hiển thị ra View
                TempData["ErrorMessage"] = "Không thể xóa! Mã giảm giá này đã có lịch sử áp dụng đơn hàng. Bạn có thể tắt kích hoạt (IsActive = false) thay vì xóa.";
                return RedirectToAction("Index");
            }

            _context.Coupons.Remove(coupon); // Xóa mã giảm giá ra khỏi database nếu chưa từng sử dụng
            _context.SaveChanges();          // Lưu thay đổi
            return RedirectToAction("Index"); // Quay về trang danh sách quản lý
        }
    }
}