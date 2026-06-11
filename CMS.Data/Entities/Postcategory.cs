using CMS.Data.Entities;
using System;
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    // Danh mục riêng dành cho bài viết/blog (tách biệt với Category của Product)
    public class PostCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int? ParentId { get; set; }               // Hỗ trợ danh mục con (nested)
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public PostCategory? Parent { get; set; }
        public ICollection<PostCategory> Children { get; set; } = new List<PostCategory>();
        public ICollection<PostPostCategory> PostPostCategories { get; set; } = new List<PostPostCategory>();
    }

    // Bảng trung gian Post <-> PostCategory (1 bài có thể thuộc nhiều danh mục)
    public class PostPostCategory
    {
        public int PostId { get; set; }
        public int PostCategoryId { get; set; }

        // Navigation properties
        public Post Post { get; set; }
        public PostCategory PostCategory { get; set; }
    }
}