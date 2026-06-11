/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Controller quản lý Danh sách yêu thích (Wishlist & WishlistItem)
              - Index         : Hiển thị toàn bộ các danh sách yêu thích của hệ thống (Admin quản lý)
              - Details       : Xem chi tiết các sản phẩm nằm bên trong một Wishlist cụ thể
              - AddToWishlist : Thêm một sản phẩm vào danh sách yêu thích của người dùng
              - RemoveItem    : Xóa một sản phẩm ra khỏi danh sách yêu thích
              - Phân quyền    : Chỉ Administrator và Admin mới được vào trang Index tổng, các chức năng thêm/xóa dùng chung
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
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public WishlistController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        [Authorize(Roles = "Administrator,Admin")] // Chỉ ban quản trị mới được xem danh sách tổng của tất cả các user
        public IActionResult Index()
        {
            var data = _context.Wishlists
                .Include(w => w.Customer)          // Tải kèm thông tin người dùng sở hữu danh sách
                .Include(w => w.WishlistItems) // Tải kèm danh sách các item để đếm số lượng mặt hàng
                .OrderByDescending(w => w.CreatedAt) // Sắp xếp danh sách mới tạo lên đầu
                .ToList();

            return View(data); // Truyền dữ liệu ra View quản trị
        }

        // ========== DETAILS ==========
        [AllowAnonymous] // Cho phép hiển thị nếu danh sách được cấu hình ở trạng thái công khai (IsPublic = true)
        public IActionResult Details(int id)
        {
            var wishlist = _context.Wishlists
                .Include(w => w.Customer)
                .Include(w => w.WishlistItems).ThenInclude(wi => wi.Product) // Tải kèm chi tiết thông tin sản phẩm trong list
                .FirstOrDefault(w => w.Id == id); // Tìm kiếm Wishlist theo ID

            if (wishlist == null) return NotFound(); // Không tìm thấy trả về lỗi 404

            return View(wishlist); // Truyền đối tượng Wishlist ra View chi tiết sản phẩm yêu thích
        }

        // ========== ADD TO WISHLIST ==========
        [HttpPost]
        [Authorize] // Yêu cầu người dùng phải đăng nhập tài khoản mới được dùng tính năng yêu thích này
        public IActionResult AddToWishlist(int wishlistId, int productId, string? note)
        {
            // Kiểm tra xem sản phẩm này đã được thêm vào danh sách yêu thích này chưa (tránh trùng lặp sản phẩm)
            bool isExist = _context.WishlistItems.Any(wi => wi.WishlistId == wishlistId && wi.ProductId == productId);

            if (isExist)
            {
                TempData["ErrorMessage"] = "Sản phẩm này đã nằm trong danh sách yêu thích của bạn rồi!";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            // Tạo mới một bản ghi chi tiết sản phẩm yêu thích (WishlistItem)
            var newItem = new WishlistItem
            {
                WishlistId = wishlistId,
                ProductId = productId,
                Note = note,
                AddedAt = DateTime.UtcNow
            };

            _context.WishlistItems.Add(newItem); // Lưu thực thể mới vào database
            _context.SaveChanges();             // Lưu thay đổi

            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào danh sách yêu thích thành công!";
            return RedirectToAction("Details", new { id = wishlistId }); // Quay lại xem chi tiết Wishlist
        }

        // ========== REMOVE ITEM ==========
        [HttpPost]
        [Authorize] // Yêu cầu đăng nhập để thực hiện xóa sản phẩm yêu thích của mình
        public IActionResult RemoveItem(int id)
        {
            // Tìm kiếm món hàng yêu thích cần xóa dựa theo ID chi tiết (WishlistItemId)
            var item = _context.WishlistItems.FirstOrDefault(wi => wi.Id == id);
            if (item == null) return NotFound(); // Trả về trang lỗi 404 nếu không tồn tại món hàng

            int currentWishlistId = item.WishlistId; // Giữ lại ID danh sách tổng để lát điều hướng quay về

            _context.WishlistItems.Remove(item); // Tiến hành xóa bản ghi khỏi database
            _context.SaveChanges();             // Lưu thay đổi vào database

            TempData["SuccessMessage"] = "Đã xóa sản phẩm ra khỏi danh sách yêu thích!";
            return RedirectToAction("Details", new { id = currentWishlistId }); // Quay lại trang chi tiết Wishlist hiện tại
        }
    }
}