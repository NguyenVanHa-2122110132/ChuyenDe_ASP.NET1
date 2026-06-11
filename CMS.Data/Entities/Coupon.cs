/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể Mã giảm giá (Coupon) - ánh xạ tới bảng Coupons trong database
              - Id                 : Khóa chính, tự tăng
              - Code               : Mã coupon (VD: SALE50), duy nhất
              - Description        : Mô tả coupon, cho phép null
              - DiscountType       : Loại giảm giá (theo % hoặc số tiền cố định)
              - DiscountValue      : Giá trị giảm (% hoặc số tiền)
              - MinOrderAmount     : Giá trị đơn hàng tối thiểu để áp dụng, cho phép null
              - MaxDiscountAmount  : Giới hạn số tiền giảm tối đa (dùng cho %), cho phép null
              - UsageLimit         : Tổng số lần sử dụng tối đa, cho phép null
              - UsedCount          : Số lần đã sử dụng
              - UsageLimitPerUser  : Giới hạn số lần dùng mỗi user, cho phép null
              - StartDate          : Ngày bắt đầu hiệu lực
              - ExpiryDate         : Ngày hết hạn
              - IsActive           : Trạng thái kích hoạt
              - CreatedAt          : Thời điểm tạo bản ghi
              - CouponUsages       : Lịch sử sử dụng coupon (quan hệ 1-Nhiều)
*/
using System;
using System.Collections.Generic;
using CMS.Data.Entities;
namespace CMS.Data.Entities
{
    public enum DiscountType
    {
        Percentage = 0,     // Giảm theo phần trăm
        FixedAmount = 1     // Giảm số tiền cố định
    }

    public class Coupon
    {
        public int Id { get; set; }                                                     // Khóa chính, tự tăng
        public string? Code { get; set; }                                              // Mã coupon, duy nhất — cho phép null
        public string? Description { get; set; }                                       // Mô tả coupon — cho phép null
        public DiscountType DiscountType { get; set; }                                  // Loại giảm giá
        public decimal DiscountValue { get; set; }                                      // Giá trị giảm (% hoặc số tiền)
        public decimal? MinOrderAmount { get; set; }                                   // Giá trị đơn tối thiểu — cho phép null
        public decimal? MaxDiscountAmount { get; set; }                                // Giới hạn giảm tối đa — cho phép null
        public int? UsageLimit { get; set; }                                           // Tổng số lần dùng tối đa — cho phép null
        public int UsedCount { get; set; } = 0;                                         // Số lần đã sử dụng
        public int? UsageLimitPerUser { get; set; }                                    // Giới hạn mỗi user — cho phép null
        public DateTime StartDate { get; set; }                                         // Ngày bắt đầu hiệu lực
        public DateTime ExpiryDate { get; set; }                                        // Ngày hết hạn
        public bool IsActive { get; set; } = true;                                      // Trạng thái kích hoạt
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;                     // Thời điểm tạo bản ghi
        public virtual ICollection<CouponUsage>? CouponUsages { get; set; }           // Lịch sử sử dụng coupon — cho phép null
    }
}