using CMS.Data.Entities;
using System;
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; } = "My Wishlist";        // Cho phép đặt tên list
        public bool IsPublic { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Customer Customer { get; set; }
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }

    public class WishlistItem
    {
        public int Id { get; set; }
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
        public string? Note { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Wishlist Wishlist { get; set; }
        public Product Product { get; set; }
    }
}