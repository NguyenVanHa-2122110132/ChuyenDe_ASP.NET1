/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 01/06/2026
    Mô tả    : Controller quản lý Chi tiết món hàng trong giỏ (CartItem)
              - AddToCart : Thêm sản phẩm vào giỏ hàng (tự động cộng dồn số lượng nếu trùng)
              - UpdateQuantity: Cập nhật tăng/giảm số lượng của một sản phẩm trong giỏ hàng
              - RemoveFromCart: Xóa bỏ hoàn toàn một mặt hàng ra khỏi giỏ
              - Phân quyền: Sử dụng [AllowAnonymous] vì chức năng giỏ hàng cho phép khách vãng lai/người dùng thao tác
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
    [AllowAnonymous] // Cho phép tất cả người mua hàng thao tác với giỏ hàng của họ
    public class CartItemController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public CartItemController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== ADD TO CART (Thêm sản phẩm vào giỏ) ==========
        [HttpPost]
        public IActionResult AddToCart(int cartId, int productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Số lượng sản phẩm thêm vào giỏ phải lớn hơn 0!";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            // Kiểm tra xem sản phẩm này đã từng được thêm vào giỏ hàng này trước đó chưa
            var existingItem = _context.CartItems
                .FirstOrDefault(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (existingItem != null)
            {
                // Nếu đã có sản phẩm này trong giỏ, tiến hành cộng dồn số lượng lên
                existingItem.Quantity += quantity;
                existingItem.UnitPrice = unitPrice; // Cập nhật lại giá bán mới nhất tại thời điểm hiện tại
                _context.CartItems.Update(existingItem);
            }
            else
            {
                // Nếu chưa có, tạo mới một bản ghi chi tiết giỏ hàng (CartItem)
                var newItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    AddedAt = DateTime.UtcNow
                };
                _context.CartItems.Add(newItem); // Thêm món mới vào database
            }

            _context.SaveChanges(); // Lưu tất cả thay đổi vào database
            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";

            // Điều hướng người dùng đến trang hiển thị chi tiết giỏ hàng của họ
            return RedirectToAction("Details", "Cart", new { id = cartId });
        }

        // ========== UPDATE QUANTITY (Cập nhật số lượng) ==========
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int newQuantity)
        {
            // Tìm món hàng cần sửa số lượng dựa theo ID chi tiết giỏ hàng
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.Id == id);
            if (cartItem == null) return NotFound(); // Trả về trang lỗi 404 nếu không tìm thấy

            if (newQuantity <= 0)
            {
                // Nếu số lượng đưa về bằng 0 hoặc âm, coi như hành động xóa món hàng khỏi giỏ
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                // Cập nhật số lượng mới do người dùng thay đổi trên giao diện
                cartItem.Quantity = newQuantity;
                _context.CartItems.Update(cartItem);
            }

            _context.SaveChanges(); // Lưu thay đổi vào hệ thống database
            TempData["SuccessMessage"] = "Cập nhật số lượng giỏ hàng thành công!";

            // Quay trở lại trang chi tiết giỏ hàng hiện tại
            return RedirectToAction("Details", "Cart", new { id = cartItem.CartId });
        }

        // ========== REMOVE FROM CART (Xóa món hàng khỏi giỏ) ==========
        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            // Tìm món hàng cần xóa trong database
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.Id == id);
            if (cartItem == null) return NotFound(); // Không tìm thấy món hàng trả về 404

            int currentCartId = cartItem.CartId; // Giữ lại CartId để lát nữa Redirect về đúng giỏ

            _context.CartItems.Remove(cartItem); // Xóa bản ghi món hàng khỏi database
            _context.SaveChanges();             // Lưu thay đổi

            TempData["SuccessMessage"] = "Đã xóa sản phẩm ra khỏi giỏ hàng!";

            // Quay trở lại hiển thị giỏ hàng sau khi dọn dẹp mặt hàng vừa xóa
            return RedirectToAction("Details", "Cart", new { id = currentCartId });
        }
    }
}