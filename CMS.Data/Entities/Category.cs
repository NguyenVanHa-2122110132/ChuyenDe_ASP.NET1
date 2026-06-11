/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Thực thể Danh mục Thời Trang (Category)
              Thêm:
              - Gender    : Danh mục thuộc giới tính nào (nam/nu/treem/all)
              - ImageUrl  : Ảnh đại diện cho danh mục
              - SortOrder : Thứ tự hiển thị
              - IsActive  : Ẩn/hiện danh mục
*/
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        // ── Thêm mới cho thời trang ──
        public string? Gender { get; set; }       // "nam" | "nu" | "treem" | "all"
        public string? ImageUrl { get; set; }     // Ảnh đại diện danh mục
        public int SortOrder { get; set; } = 0;   // Thứ tự hiển thị
        public bool IsActive { get; set; } = true;

        // ── Navigation properties (giữ nguyên) ──
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<CategoryProduct>? CategoryProducts { get; set; }
    }
}
