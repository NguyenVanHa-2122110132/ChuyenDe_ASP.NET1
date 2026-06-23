/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 16/05/2026
    Mô tả    : Thực thể Khách hàng (Customer) - ánh xạ tới bảng Customers trong database
              - Id      : Khóa chính, tự tăng
              - FullName: Họ tên khách hàng, bắt buộc nhập
              - Email   : Địa chỉ email, bắt buộc nhập và đúng định dạng
              - Phone   : Số điện thoại, cho phép null
              - Address : Địa chỉ giao hàng, cho phép null
              - Password: Mật khẩu đăng nhập, bắt buộc nhập
              - Orders  : Danh sách đơn hàng của khách hàng (quan hệ 1-Nhiều)
*/
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }          // Khóa chính, tự tăng

        [Required]
        public string? FullName { get; set; } // Họ tên khách hàng, bắt buộc nhập

        [Required]
        [EmailAddress]
        public string? Email { get; set; }    // Địa chỉ email, bắt buộc và đúng định dạng

        public string? Phone { get; set; }    // Số điện thoại, cho phép null

        public string? Address { get; set; }  // Địa chỉ giao hàng, cho phép null

        [Required]
        public string? Password { get; set; } // Mật khẩu đăng nhập, bắt buộc nhập
        public virtual ICollection<Order>? Orders { get; set; }// danh sách đơn hàng của khách hàng
        public virtual ICollection<Comment>? Comments { get; set; }      
        public virtual ICollection<Review>? Reviews { get; set; }       
        public virtual ICollection<Wishlist>? Wishlists { get; set; }    
        public virtual ICollection<CouponUsage>? CouponUsages { get; set; }
        public string? AvatarUrl { get; set; } // Đường dẫn ảnh đại diện, cho phép null

    }
}