/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Thực thể Sản phẩm THỜI TRANG (Product)
              Thêm các trường đặc thù thời trang:
              - Gender        : Giới tính (nam/nu/treem)
              - Sizes         : Danh sách size (S,M,L,XL,XXL)
              - Colors        : Màu sắc có sẵn
              - Material      : Chất liệu vải
              - Brand         : Thương hiệu
              - OriginalPrice : Giá gốc (để hiển thị gạch ngang khi sale)
              - IsNew/IsHot/IsSale : Badge hiển thị trên frontend
              - Stock         : Số lượng tồn kho
              - CategoryName  : Tên danh mục (computed từ join)
*/
using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }

        // ── Thông tin cơ bản ──
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // ── Giá ──
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }   // Giá gốc trước khi giảm, null = không sale

        // ── Thời trang đặc thù ──
        public string? Gender { get; set; }           // "nam" | "nu" | "treem"
        public string? Sizes { get; set; }            // Lưu JSON string: "[\"S\",\"M\",\"L\",\"XL\"]"
        public string? Colors { get; set; }           // Lưu JSON string: "[\"Đen\",\"Trắng\",\"Xanh\"]"
        public string? Material { get; set; }         // Chất liệu: "Cotton", "Polyester", "Linen"...
        public string? Brand { get; set; }            // Thương hiệu

        // ── Badge hiển thị ──
        public bool IsNew { get; set; } = false;
        public bool IsHot { get; set; } = false;
        public bool IsSale { get; set; } = false;
        public bool IsActive { get; set; } = true;   // Ẩn/hiện sản phẩm

        // ── Kho ──
        public int Stock { get; set; } = 0;

        // ── Navigation properties ──
        public virtual ICollection<CategoryProduct>? CategoryProducts { get; set; }
        public  ICollection<ProductImage>? ProductImages { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
