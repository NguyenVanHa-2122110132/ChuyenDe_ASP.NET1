using System;
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;    // URL-friendly (VD: lap-trinh-web)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}