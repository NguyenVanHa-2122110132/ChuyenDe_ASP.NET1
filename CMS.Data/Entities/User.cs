/*
Họ Và Tên: Nguyễn Văn Hà
MSSV: 2122110132
Lớp: CCQ2211D
Ngày Tạo:16/05/2026
Mô Tả: Thực Thể Danh Người dùng*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Data.Entities
{
    public class User
    {
        public int Id { get; set; } // Mã danh mục
        public string? Username { get; set; }//Tên Đăng Nhập
        public string? PasswordHash { get; set; }// mã giả
        public string? FullName { get; set; }// Họ Tên người dùng
        public string? Role { get; set; } // Quản trị viên hoặc Biên tập viên

    }
}
