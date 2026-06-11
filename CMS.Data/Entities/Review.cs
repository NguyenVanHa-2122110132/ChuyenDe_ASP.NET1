using CMS.Data.Entities;
using System;
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int? OrderDetailId { get; set; }          // Liên kết với đơn hàng đã mua
        public int Rating { get; set; }                  // 1 - 5 sao
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool IsVerifiedPurchase { get; set; } = false;
        public bool IsApproved { get; set; } = false;    // Admin duyệt trước khi hiển thị
        public int HelpfulCount { get; set; } = 0;       // Số người thấy hữu ích
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Product Product { get; set; }
        public Customer Customer { get; set; }
        public OrderDetail? OrderDetail { get; set; }
        public ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();
    }

    public class ReviewImage
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;

        // Navigation properties
        public Review Review { get; set; }
    }
}