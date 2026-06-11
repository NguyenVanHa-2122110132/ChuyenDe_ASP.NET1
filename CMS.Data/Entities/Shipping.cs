using CMS.Data.Entities;
using System;

namespace CMS.Data.Entities
{
    public enum ShippingStatus
    {
        Pending = 0,
        Confirmed = 1,
        PickedUp = 2,
        InTransit = 3,
        OutForDelivery = 4,
        Delivered = 5,
        Failed = 6,
        Returned = 7
    }

    public class Shipping
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;         // Phường/Xã
        public string District { get; set; } = string.Empty;     // Quận/Huyện
        public string Province { get; set; } = string.Empty;     // Tỉnh/Thành phố
        public string Country { get; set; } = "Vietnam";
        public string? ShippingCarrier { get; set; }             // VD: GHN, GHTK, ViettelPost
        public string? TrackingNumber { get; set; }
        public decimal ShippingFee { get; set; }
        public ShippingStatus Status { get; set; } = ShippingStatus.Pending;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Order Order { get; set; }
    }
}