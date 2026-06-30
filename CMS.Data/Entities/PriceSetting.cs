/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 25/06/2026
    Mô tả    : Entity cấu hình khoảng giá hiển thị trên trang Shop React
*/
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.Entities
{
    public class PriceSetting
    {
        [Key]
        public int Id { get; set; }

        // Giá tối thiểu gợi ý hiển thị trên trang lọc
        public decimal MinPrice { get; set; } = 0;

        // Giá tối đa gợi ý hiển thị trên trang lọc
        public decimal MaxPrice { get; set; } = 10000000;

        // Nhãn mô tả (ví dụ: "Thời trang", "Nước hoa")
        public string? Label { get; set; }

        // Ngày cập nhật gần nhất
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}