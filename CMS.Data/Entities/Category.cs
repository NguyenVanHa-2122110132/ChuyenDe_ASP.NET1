/*
Họ Và Tên: Nguyễn Văn Hà
MSSV: 2122110132
Lớp: CCQ2211D
Ngày Tạo:16/05/2026
Mô Tả: Thực Thể Danh Mục Bài Viết*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }// mã danh mục
        public string Name { get; set; } // Tên danh mục 
        public string Description { get; set; }//Mô tả danh mục

        // Quan hệ: Một danh mục có nhiều bài viết
        public virtual ICollection<Post> Posts { get; set; }

    }
}
