/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 05/06/2026
    Mô tả    : Thực thể OtpCode - ánh xạ tới bảng OtpCodes trong database
              - Id        : Khóa chính, tự tăng
              - Email     : Email nhận mã OTP
              - OtpCode   : Mã OTP 6 số
              - ExpiredAt : Thời gian hết hạn (5 phút)
              - IsUsed    : Đã sử dụng chưa
              - CreatedAt : Thời gian tạo
*/
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CMS.Data.Entities
{
    public class OtpCode
    {
        [Key]
        public int Id { get; set; }           // Khóa chính, tự tăng

        [Required]
        public string Email { get; set; } = ""; // Email nhận OTP

        [Required]
        [Column("OtpCode")]
        public string Otp { get; set; } = "";   // Mã OTP 6 số

        public DateTime ExpiredAt { get; set; } // Thời gian hết hạn

        public bool IsUsed { get; set; } = false; // Đã dùng chưa

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo
        public int FailedAttempts { get; set; } = 0; // Số lần nhập sai OTP, khóa khi đạt 5 lần
    }
}