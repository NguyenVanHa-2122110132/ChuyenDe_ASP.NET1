using CMS.Data.Entities;
using System;
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int? UserId { get; set; }                  // null nếu comment ẩn danh
        public int? CustomerId { get; set; }     // Thêm mới - cho khách hàng
        public Customer? Customer { get; set; }
        public int? ParentCommentId { get; set; }         // null nếu là comment gốc (reply)
        public string? GuestName { get; set; }            // Tên khách nếu không đăng nhập
        public string? GuestEmail { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;     // Admin duyệt trước khi hiển thị
        public int LikeCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Post Post { get; set; }
        public User? User { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}