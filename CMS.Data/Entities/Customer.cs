/*
Họ Và Tên: Nguyễn Văn Hà
MSSV: 2122110132
Lớp: CCQ2211D
Ngày Tạo:16/05/2026
Mô Tả: Thực Thể Khách Hàng*/

using System;
using System.Collections.Generic;
using System.Text;


using System.ComponentModel.DataAnnotations;
namespace CMS.Data.Entities
{
    //Khách hàng
    public class Customer
    {
        [Key]//khoá chính
        public int Id { get; set; }//mã duy nhất của mõi khách hàng

        [Required]//họ tên bắt buộc
        public string FullName { get; set; }//họ tên khách hàng

        [Required]//email bắt buộc
        [EmailAddress]//kiểm tra định dạng email hợp lệ
        public string Email { get; set; }//địa chỉ mail của khách hàng

        public string? Phone { get; set; }//số điện thoại(NULL)

        public string? Address { get; set; }//địa chỉ (NULL)

        [Required]//mật khẩu bắt buộc
        public string Password { get; set; } // Lưu mật khẩu thô theo yêu cầu tối giản

        public virtual ICollection<Order>? Orders { get; set; }




    }
}
