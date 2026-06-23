/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể Đơn hàng (Order) - ánh xạ tới bảng Orders trong database
              - Id          : Khóa chính, tự tăng
              - OrderDate   : Ngày đặt hàng
              - CustomerId  : Khóa ngoại liên kết tới bảng Customer
              - Status      : Trạng thái đơn hàng (1 = Đã xác nhận, 0 = Chờ xử lý)
              - Email       : Email nhận thông báo đơn hàng
              - PaymentMethod: Phương thức thanh toán (cod hoặc bank)
              - Notes       : Ghi chú thêm của khách hàng, cho phép null
              - Customer    : Navigation property để truy xuất thông tin khách hàng
              - OrderDetails: Danh sách chi tiết sản phẩm trong đơn hàng (quan hệ 1-Nhiều)
*/
using System;
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }                                               // Khóa chính, tự tăng
        public DateTime OrderDate { get; set; }                                  // Ngày đặt hàng
        public int CustomerId { get; set; }                                      // Khóa ngoại liên kết tới Customer
        public int Status { get; set; }                                          // 1 = Đã xác nhận, 0 = Chờ xử lý

        // --- THÊM 2 TRƯỜNG MỚI Ở ĐÂY ĐỂ ĐỒNG BỘ VỚI FRONTEND ---
        public string Email { get; set; } = string.Empty;                        // Email nhận thông báo đơn hàng
        public string PaymentMethod { get; set; } = "cod";                       // 'cod' | 'bank'
        // -----------------------------------------------------

        public string? Notes { get; set; }                                       // Ghi chú thêm, cho phép null
        public virtual Customer? Customer { get; set; }                          // Navigation property — truy xuất khách hàng
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }      // Danh sách chi tiết đơn hàng
    }
}