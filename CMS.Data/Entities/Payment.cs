/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể Thanh toán (Payment) - ánh xạ tới bảng Payments trong database
              - Id                     : Khóa chính, tự tăng
              - OrderId                : Khóa ngoại liên kết tới Order
              - Method                 : Phương thức thanh toán (Cash, Momo, VNPay, ...)
              - Status                 : Trạng thái thanh toán (Pending, Completed, Failed, ...)
              - Amount                 : Số tiền thanh toán
              - TransactionId          : Mã giao dịch từ cổng thanh toán, cho phép null
              - PaymentGatewayResponse : Phản hồi JSON từ cổng thanh toán, cho phép null
              - PaidAt                 : Thời điểm thanh toán thành công, cho phép null
              - CreatedAt              : Thời điểm tạo bản ghi
              - UpdatedAt              : Thời điểm cập nhật bản ghi
              - Order                  : Đơn hàng liên kết (quan hệ Nhiều-1)
*/
using System;
namespace CMS.Data.Entities
{
    public enum PaymentMethod
    {
        Cash = 0,           // Tiền mặt
        CreditCard = 1,     // Thẻ tín dụng
        DebitCard = 2,      // Thẻ ghi nợ
        BankTransfer = 3,   // Chuyển khoản ngân hàng
        Momo = 4,           // Ví Momo
        ZaloPay = 5,        // Ví ZaloPay
        VNPay = 6,          // Cổng VNPay
        PayPal = 7          // PayPal
    }

    public enum PaymentStatus
    {
        Pending = 0,        // Chờ thanh toán
        Processing = 1,     // Đang xử lý
        Completed = 2,      // Hoàn thành
        Failed = 3,         // Thất bại
        Refunded = 4,       // Đã hoàn tiền
        Cancelled = 5       // Đã hủy
    }

    public class Payment
    {
        public int Id { get; set; }                                                     // Khóa chính, tự tăng
        public int OrderId { get; set; }                                                // Khóa ngoại liên kết tới Order
        public PaymentMethod Method { get; set; }                                       // Phương thức thanh toán
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;             // Trạng thái thanh toán
        public decimal Amount { get; set; }                                             // Số tiền thanh toán
        public string? TransactionId { get; set; }                                     // Mã giao dịch từ cổng thanh toán — cho phép null
        public string? PaymentGatewayResponse { get; set; }                            // Phản hồi JSON từ cổng thanh toán — cho phép null
        public DateTime? PaidAt { get; set; }                                          // Thời điểm thanh toán thành công — cho phép null
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;                     // Thời điểm tạo bản ghi
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;                     // Thời điểm cập nhật bản ghi
        public virtual Order? Order { get; set; }                                      // Đơn hàng liên kết — cho phép null
    }
}