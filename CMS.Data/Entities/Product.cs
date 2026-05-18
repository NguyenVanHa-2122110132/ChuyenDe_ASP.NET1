/*
Họ Và Tên: Nguyễn Văn Hà
MSSV: 2122110132
Lớp: CCQ2211D
Ngày Tạo:16/05/2026
Mô Tả: Thực Thể sản phẩm*/
using System;
using System.Collections.Generic;
using System.Text;


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CMS.Data.Entities
{
    public class Product
    {
        [Key]//khoá chính
        public int Id { get; set; }// mã danh mục

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string Name { get; set; }//tên danh mục

        public string? Description { get; set; }//mô tả sản phẩm

        [Range(0, double.MaxValue)]//giá trị tối thiểu(>=0)
        [Column(TypeName = "decimal(18,2)")]//KDL,CSDL cụ thể
        public decimal Price { get; set; }//giá sản phẩm

        public int StockQuantity { get; set; }//SL SP trong kho

        public string? ImageUrl { get; set; }//đường dẩn ảnh sản phẩm

        // Khóa ngoại nối tới CategoryProduct
        public int CategoryProductId { get; set; }//khoá ngoại

        [ForeignKey("CategoryProductId")]//địa chỉ khoá cụ thể
        public virtual CategoryProduct? CategoryProduct { get; set; }//đối tượng danh mục liên kết


    }
}
