/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể Danh mục (Category)
*/
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        // Thêm dòng này để đồng bộ mối quan hệ Nhiều - Nhiều với bảng trung gian
        public virtual ICollection<CategoryProduct> CategoryProducts { get; set; }
    }
}