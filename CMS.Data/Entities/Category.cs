/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể Danh mục (Category) - ánh xạ tới bảng Categories trong database
              - Id                : Khóa chính, tự tăng
              - Name              : Tên danh mục, cho phép null
              - Description       : Mô tả danh mục, cho phép null
              - Posts             : Danh sách bài viết thuộc danh mục này (quan hệ 1-Nhiều)
              - CategoryProducts  : Danh sách liên kết với sản phẩm (quan hệ Nhiều-Nhiều qua bảng trung gian)
*/
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }                                           // Khóa chính, tự tăng

        public string? Name { get; set; }                                     // Tên danh mục, cho phép null

        public string? Description { get; set; }                              // Mô tả danh mục, cho phép null

        public virtual ICollection<Post>? Posts { get; set; }                // Danh sách bài viết thuộc danh mục — cho phép null

        public virtual ICollection<CategoryProduct>? CategoryProducts { get; set; } // Liên kết nhiều-nhiều với sản phẩm — cho phép null
    }
}