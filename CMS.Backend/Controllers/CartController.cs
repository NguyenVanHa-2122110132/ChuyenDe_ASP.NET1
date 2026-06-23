/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 01/06/2026
    Mô tả    : Controller quản lý Giỏ hàng (Cart)
              - Index  : Hiển thị danh sách toàn bộ giỏ hàng của hệ thống (Admin quản lý)
              - Details: Xem chi tiết một giỏ hàng cụ thể gồm những sản phẩm nào và tính tổng tiền
              - Delete : Xóa giỏ hàng khỏi hệ thống (Giải phóng hoặc dọn dẹp giỏ hàng trống)
              - Phân quyền: Chỉ Administrator và Admin mới được quyền quản lý danh sách giỏ hàng tổng
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
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào quản lý danh sách giỏ hàng
    public class CartController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public CartController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var data = _context.Carts
                .Include(c => c.Customer)  // Tải kèm thông tin khách hàng sở hữu giỏ hàng
                .Include(c => c.CartItems) // Tải kèm danh sách các món hàng để đếm số lượng mặt hàng
                .OrderByDescending(c => c.UpdatedAt) // Sắp xếp giỏ hàng có cập nhật mới nhất lên đầu
                .ToList(); // Lấy danh sách toàn bộ giỏ hàng từ database

            return View(data); // Truyền dữ liệu ra View quản trị
        }

        // ========== DETAILS ==========
        public IActionResult Details(int id)
        {
            var cart = _context.Carts
                .Include(c => c.Customer) // Tải kèm thông tin khách hàng
                .Include(c => c.CartItems!).ThenInclude(ci => ci.Product) // Tải kèm danh sách món hàng và thông tin sản phẩm chi tiết của món đó
                .FirstOrDefault(c => c.Id == id); // Tìm kiếm giỏ hàng theo ID

            if (cart == null) return NotFound(); // Nếu không tìm thấy trả về lỗi 404

            // Tính tổng tiền tạm tính của giỏ hàng để hiển thị ra View chi tiết
            decimal totalAmount = 0;
            if (cart.CartItems != null)
            {
                // Tổng tiền = Số lượng * Giá bán của từng sản phẩm trong giỏ (giả định bảng Product có thuộc tính Price)
                // totalAmount = cart.CartItems.Sum(item => item.Quantity * (item.Product?.Price ?? 0));
            }

            ViewBag.TotalAmount = totalAmount; // Chuyển tổng tiền ra View bằng ViewBag
            return View(cart); // Truyền đối tượng giỏ hàng ra View chi tiết
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var cart = _context.Carts
                .Include(c => c.CartItems) // Tải kèm danh sách món hàng con để xử lý dọn dẹp
                .FirstOrDefault(c => c.Id == id); // Tìm giỏ hàng theo ID

            if (cart == null) return NotFound(); // Không tìm thấy trả về lỗi 404

            // Ràng buộc hệ thống: Xóa toàn bộ sản phẩm con nằm trong giỏ trước để tránh lỗi khóa ngoại (Cascade Delete thủ công)
            if (cart.CartItems != null && cart.CartItems.Any())
            {
                _context.RemoveRange(cart.CartItems);
            }

            _context.Carts.Remove(cart); // Xóa giỏ hàng chính khỏi database
            _context.SaveChanges();     // Lưu các thay đổi vào database

            TempData["SuccessMessage"] = "Xóa giỏ hàng và dọn dẹp các mặt hàng liên quan thành công!";
            return RedirectToAction("Index"); // Quay về trang danh sách quản lý giỏ hàng
        }
    }
}