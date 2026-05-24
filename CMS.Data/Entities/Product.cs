/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể Sản phẩm (Product) - ánh xạ tới bảng Products trong database
              - Id              : Khóa chính, tự tăng
              - Name            : Tên sản phẩm điện thoại, cho phép null
              - Price           : Giá bán sản phẩm
              - ImageUrl        : Đường dẫn ảnh sản phẩm, cho phép null
              - Description     : Mô tả chi tiết sản phẩm, cho phép null
              - CategoryProducts: Danh sách liên kết với danh mục (quan hệ Nhiều-Nhiều)
*/
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }                                          // Khóa chính, tự tăng
        public string? Name { get; set; }                                    // Tên sản phẩm, cho phép null
        public decimal Price { get; set; }                                   // Giá bán sản phẩm
        public string? ImageUrl { get; set; }                                // Đường dẫn ảnh, cho phép null
        public string? Description { get; set; }                             // Mô tả chi tiết, cho phép null
        public virtual ICollection<CategoryProduct>? CategoryProducts { get; set; } // Liên kết nhiều-nhiều với danh mục
    }
}