/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể Sản phẩm (Product) - Điện thoại
*/
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } // Tên điện thoại
        public decimal Price { get; set; } // Giá tiền
        public string ImageUrl { get; set; } // Đường dẫn ảnh
        public string Description { get; set; } // Mô tả chi tiết

        // Thay thế quan hệ cũ bằng quan hệ Nhiều - Nhiều qua bảng trung gian
        public virtual ICollection<CategoryProduct> CategoryProducts { get; set; }
    }
}