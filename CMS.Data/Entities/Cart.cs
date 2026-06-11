/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể Giỏ hàng (Cart) - ánh xạ tới bảng Carts trong database
              - Id         : Khóa chính, tự tăng
              - CustomerId : Khóa ngoại liên kết tới Customer
              - CreatedAt  : Thời điểm tạo giỏ hàng
              - UpdatedAt  : Thời điểm cập nhật giỏ hàng
              - Customer   : Khách hàng sở hữu giỏ hàng (quan hệ Nhiều-1)
              - CartItems  : Danh sách sản phẩm trong giỏ hàng (quan hệ 1-Nhiều)
*/
using System;
using System.Collections.Generic;
namespace CMS.Data.Entities
{
    public class Cart
    {
        public int Id { get; set; }                                         // Khóa chính, tự tăng
        public int CustomerId { get; set; }                                 // Khóa ngoại liên kết tới Customer
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;         // Thời điểm tạo giỏ hàng
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;         // Thời điểm cập nhật giỏ hàng
        public virtual Customer? Customer { get; set; }                    // Khách hàng sở hữu giỏ hàng — cho phép null
        public virtual ICollection<CartItem>? CartItems { get; set; }      // Danh sách sản phẩm trong giỏ — cho phép null
    }
}