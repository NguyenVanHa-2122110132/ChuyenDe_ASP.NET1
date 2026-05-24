/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể trung gian liên kết Danh mục (Category) và Sản phẩm (Product)
              - CategoryId : Khóa ngoại nối sang bảng Category
              - ProductId  : Khóa ngoại nối sang bảng Product
              - Category   : Navigation property để truy xuất thông tin danh mục
              - Product    : Navigation property để truy xuất thông tin sản phẩm
*/
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Data.Entities
{
    public class CategoryProduct
    {
        public int CategoryId { get; set; }              // Khóa ngoại nối sang bảng Category
        public int ProductId { get; set; }               // Khóa ngoại nối sang bảng Product
        public virtual Category? Category { get; set; } // Navigation property — truy xuất danh mục, cho phép null
        public virtual Product? Product { get; set; }   // Navigation property — truy xuất sản phẩm, cho phép null
    }
}