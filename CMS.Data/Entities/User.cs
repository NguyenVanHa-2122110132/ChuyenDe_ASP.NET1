/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 16/05/2026
    Mô tả    : Thực thể Người dùng (User) - ánh xạ tới bảng Users trong database
              - Id          : Khóa chính, tự tăng
              - Username    : Tên đăng nhập, cho phép null
              - PasswordHash: Mật khẩu đã được mã hóa, cho phép null
              - FullName    : Họ và tên đầy đủ, cho phép null
              - Role        : Vai trò người dùng (Administrator, Admin, Sales...), cho phép null
              - Email       : Địa chỉ email để lấy lại mật khẩu, cho phép null
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Data.Entities
{
    public class User
    {
        public int Id { get; set; }                    // Khóa chính, tự tăng
        public string? Username { get; set; }          // Tên đăng nhập
        public string? PasswordHash { get; set; }      // Mật khẩu đã mã hóa
        public string? FullName { get; set; }          // Họ và tên đầy đủ
        public string? Role { get; set; }              // Vai trò: Administrator, Admin, Sales...
        public string? Email { get; set; }             // Email để lấy lại mật khẩu
    }
}