/*
Họ Và Tên: Nguyễn Văn Hà
MSSV: 2122110132
Lớp: CCQ2211D
Ngày Tạo:16/05/2026
Mô Tả: Thực Thể Bài Viết*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }// Mã danh mục
        public string Title { get; set; } // Tiêu đề bài viết
        public string Content { get; set; } // Nội dung chi tiết
        public string ImageUrl { get; set; } // Hình ảnh đại diện
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Khóa ngoại liên kết tới Category
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

    }
}
