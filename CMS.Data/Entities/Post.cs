/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 16/05/2026
    Mô tả    : Thực thể Bài viết (Post) - ánh xạ tới bảng Posts trong database
              - Id          : Khóa chính, tự tăng
              - Title       : Tiêu đề bài viết, cho phép null
              - Content     : Nội dung chi tiết bài viết, cho phép null
              - ImageUrl    : Đường dẫn ảnh đại diện, cho phép null
              - CreatedDate : Ngày tạo, tự động gán thời điểm hiện tại nếu không nhập
              - CategoryId  : Khóa ngoại liên kết tới bảng Category
              - Category    : Navigation property để truy xuất thông tin danh mục (Join bảng)
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }                              // Khóa chính, tự tăng

        public string? Title { get; set; }                      // Tiêu đề bài viết, cho phép null

        public string? Content { get; set; }                    // Nội dung chi tiết, cho phép null

        public string? ImageUrl { get; set; }                   // Đường dẫn ảnh đại diện, cho phép null

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo, mặc định là thời điểm hiện tại

        public int CategoryId { get; set; }                     // Khóa ngoại liên kết tới bảng Category

        public virtual Category? Category { get; set; }         // Navigation property — dùng để Join lấy tên danh mục
    }
}