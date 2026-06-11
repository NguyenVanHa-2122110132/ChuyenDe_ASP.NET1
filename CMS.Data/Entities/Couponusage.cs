using CMS.Data.Entities;
using System;

namespace CMS.Data.Entities
{
    public class CouponUsage
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public decimal DiscountApplied { get; set; }   // Số tiền thực tế được giảm
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Coupon Coupon { get; set; }
        public Customer Customer { get; set; }
        public Order Order { get; set; }
    }
}