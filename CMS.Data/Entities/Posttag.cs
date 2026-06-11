using CMS.Data.Entities;

namespace CMS.Data.Entities
{
    // Bảng trung gian Post <-> Tag (Many-to-Many)
    public class PostTag
    {
        public int PostId { get; set; }
        public int TagId { get; set; }

        // Navigation properties
        public Post Post { get; set; }
        public Tag Tag { get; set; }
    }
}