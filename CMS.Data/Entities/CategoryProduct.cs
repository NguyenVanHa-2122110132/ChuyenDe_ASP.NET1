/*
Họ Và Tên: Nguyễn Văn Hà
MSSV: 2122110132
Lớp: CCQ2211D
Ngày Tạo:16/05/2026
Mô Tả: Thực Thể Danh Mục sản phẩm*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

using System.ComponentModel.DataAnnotations;
namespace CMS.Data.Entities
{
    public class CategoryProduct
    {
        [Key]//khoá chính
        public int Id { get; set; }// mã danh mục

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100)] // giới hạn độ dài chuôi
        public string Name { get; set; }// tên danh mục

        public string? Description { get; set; }// mô tả danh mục

        // Quan hệ: Một danh mục có nhiều sản phẩm
        public virtual ICollection<Product>? Products { get; set; }

    }
}
