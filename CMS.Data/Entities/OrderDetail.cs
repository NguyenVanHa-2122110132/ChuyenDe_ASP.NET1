/*
Họ Và Tên: Nguyễn Văn Hà
MSSV: 2122110132
Lớp: CCQ2211D
Ngày Tạo:16/05/2026
Mô Tả: Thực Thể chi tiết đơn hàng*/
using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CMS.Data.Entities
{
    public class OrderDetail
    {
        [Key]// khoá chính
        public int Id { get; set; }//MÃ DUY NHẤT cho mỗi CHI TIẾT ĐƠN HÀNG

        public int OrderId { get; set; }//KHÓA NGOẠI (FOREIGN KEY) tới ĐƠN HÀNG (Order)

        public int ProductId { get; set; }// KHÓA NGOẠI (FOREIGN KEY) tới SẢN PHẨM (Product)

        public int Quantity { get; set; }//SỐ LƯỢNG SẢN PHẨM CỦA CHI TIẾT NÀY

        [Column(TypeName = "decimal(18,2)")]//KIỂU DỮ LIỆU CSDL CỤ THỂ (decimal cho tiền tệ)
        public decimal UnitPrice { get; set; } // Giá tại thời điểm mua

        [ForeignKey("OrderId")]// LIÊN KẾT KHÓA NGOẠI `OrderId` VỚI ĐỐI TƯỢNG `Order`
        public virtual Order? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }



    }
}
