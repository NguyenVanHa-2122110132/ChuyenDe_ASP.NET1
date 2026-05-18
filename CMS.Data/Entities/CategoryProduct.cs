/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể trung gian liên kết Danh mục (Category) và Sản phẩm (Product)
*/
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Data.Entities
{
    public class CategoryProduct
    {
        public int CategoryId { get; set; } // Khóa ngoại nối sang bảng Category
        public int ProductId { get; set; }  // Khóa ngoại nối sang bảng Product

        public virtual Category Category { get; set; }
        public virtual Product Product { get; set; }
    }
}