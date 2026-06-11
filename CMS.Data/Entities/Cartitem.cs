/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể Chi tiết giỏ hàng (CartItem) - ánh xạ tới bảng CartItems trong database
              - Id        : Khóa chính, tự tăng
              - CartId    : Khóa ngoại liên kết tới Cart
              - ProductId : Khóa ngoại liên kết tới Product
              - Quantity  : Số lượng sản phẩm trong giỏ
              - UnitPrice : Giá tại thời điểm thêm vào giỏ
              - AddedAt   : Thời điểm thêm vào giỏ hàng
              - Cart      : Giỏ hàng chứa item này (quan hệ Nhiều-1)
              - Product   : Sản phẩm được thêm vào giỏ (quan hệ Nhiều-1)
*/
using System;
namespace CMS.Data.Entities
{
    public class CartItem
    {
        public int Id { get; set; }                                         // Khóa chính, tự tăng
        public int CartId { get; set; }                                     // Khóa ngoại liên kết tới Cart
        public int ProductId { get; set; }                                  // Khóa ngoại liên kết tới Product
        public int Quantity { get; set; }                                   // Số lượng sản phẩm trong giỏ
        public decimal UnitPrice { get; set; }                              // Giá tại thời điểm thêm vào giỏ
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;           // Thời điểm thêm vào giỏ hàng
        public virtual Cart? Cart { get; set; }                            // Giỏ hàng chứa item này — cho phép null
        public virtual Product? Product { get; set; }                      // Sản phẩm được thêm vào giỏ — cho phép null
    }
}