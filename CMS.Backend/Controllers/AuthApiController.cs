/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 05/06/2026
    Mô tả    : API xử lý đăng ký, xác nhận OTP, đăng nhập, quên mật khẩu
              - SendOtp        : Gửi mã OTP về email khi đăng ký
              - VerifyOtp      : Xác nhận mã OTP và tạo tài khoản
              - Login          : Đăng nhập bằng email + mật khẩu
              - ForgotPassword : Gửi mã OTP đặt lại mật khẩu (đã đổi từ link sang OTP)
              - VerifyResetOtp : Xác nhận mã OTP quên mật khẩu, cấp token tạm (MỚI)
              - ResetPassword  : Đặt lại mật khẩu mới (dùng token tạm)
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CMS.Data;
using CMS.Data.Entities;
using CMS.Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthApiController(ApplicationDbContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        // ========== BƯỚC 1: GỬI OTP (ĐĂNG KÝ) ==========
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto)
        {
            // Kiểm tra email đã tồn tại chưa
            if (_context.Customers.Any(c => c.Email == dto.Email))
                return BadRequest(new { message = "Email này đã được sử dụng." });

            // Tạo mã OTP 6 số ngẫu nhiên
            var otp = new Random().Next(100000, 999999).ToString();

            // Xóa OTP cũ của email này (nếu có)
            var oldOtps = _context.OtpCodes.Where(o => o.Email == dto.Email);
            _context.OtpCodes.RemoveRange(oldOtps);

            // Lưu OTP mới vào database
            _context.OtpCodes.Add(new OtpCode
            {
                Email = dto.Email,
                Otp = otp,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5), // Hết hạn sau 5 phút
                IsUsed = false,
            });
            await _context.SaveChangesAsync();

            // Gửi OTP qua email
            await _emailService.SendOtpEmailAsync(dto.Email, dto.FullName, otp);

            return Ok(new { message = "Mã OTP đã được gửi về email của bạn!" });
        }

        // ========== BƯỚC 2: XÁC NHẬN OTP + TẠO TÀI KHOẢN (ĐĂNG KÝ) ==========
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            // Tìm OTP hợp lệ
            var otpRecord = _context.OtpCodes
                .FirstOrDefault(o =>
                    o.Email == dto.Email &&
                    o.Otp == dto.Otp &&
                    !o.IsUsed &&
                    o.ExpiredAt > DateTime.UtcNow);

            if (otpRecord == null)
                return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn." });

            // Đánh dấu OTP đã dùng
            otpRecord.IsUsed = true;

            // Hash password
            var hasher = new PasswordHasher<object>();
            var hashedPassword = hasher.HashPassword(null, dto.Password);

            // Tạo tài khoản khách hàng
            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = hashedPassword,
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công! Vui lòng đăng nhập." });
        }

        // ========== ĐĂNG NHẬP ==========
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // Tìm customer theo email
            var customer = _context.Customers
                .FirstOrDefault(c => c.Email == dto.Email);

            if (customer == null)
                return Unauthorized(new { message = "Email không tồn tại." });

            // Verify password
            var hasher = new PasswordHasher<object>();
            var result = hasher.VerifyHashedPassword(null, customer.Password!, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Mật khẩu không đúng." });

            // Tạo JWT token
            var token = GenerateToken(customer);

            return Ok(new { token, fullName = customer.FullName, email = customer.Email });
        }

        // ========== QUÊN MẬT KHẨU - BƯỚC 1: GỬI MÃ OTP ==========
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Email == dto.Email);

            // Không báo lộ email có tồn tại không (bảo mật)
            if (customer == null)
                return Ok(new { message = "Nếu email tồn tại, mã xác nhận đã được gửi." });

            // Tạo mã OTP 6 ký tự chữ + số (viết hoa, bỏ ký tự dễ nhầm)
            var otp = GenerateAlphaNumericOtp(6);

            // Xóa OTP/token cũ của email này (cả OTP quên mật khẩu và token tạm, nếu có)
            var oldCodes = _context.OtpCodes.Where(o => o.Email == dto.Email);
            _context.OtpCodes.RemoveRange(oldCodes);

            // Lưu OTP mới vào database
            _context.OtpCodes.Add(new OtpCode
            {
                Email = dto.Email,
                Otp = otp,
                ExpiredAt = DateTime.UtcNow.AddMinutes(10), // Hết hạn sau 10 phút
                IsUsed = false,
                FailedAttempts = 0,
            });
            await _context.SaveChangesAsync();

            // Gửi mã OTP qua email
            await _emailService.SendResetOtpEmailAsync(dto.Email, customer.FullName!, otp);

            return Ok(new { message = "Nếu email tồn tại, mã xác nhận đã được gửi." });
        }

        // ========== QUÊN MẬT KHẨU - BƯỚC 2: XÁC NHẬN MÃ OTP ==========
        [HttpPost("verify-reset-otp")]
        public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyResetOtpDto dto)
        {
            // Tìm OTP còn hiệu lực (chưa dùng) của email này
            var otpRecord = _context.OtpCodes
                .FirstOrDefault(o => o.Email == dto.Email && !o.IsUsed);

            if (otpRecord == null || otpRecord.ExpiredAt <= DateTime.UtcNow)
                return BadRequest(new { message = "Mã xác nhận không hợp lệ hoặc đã hết hạn." });

            // Đã khóa do nhập sai quá nhiều lần
            if (otpRecord.FailedAttempts >= 5)
                return BadRequest(new { message = "Bạn đã nhập sai quá nhiều lần. Vui lòng yêu cầu mã mới." });

            // So sánh mã OTP (không phân biệt hoa thường để tránh người dùng gõ nhầm)
            if (!string.Equals(otpRecord.Otp, dto.Otp, StringComparison.OrdinalIgnoreCase))
            {
                otpRecord.FailedAttempts += 1;
                await _context.SaveChangesAsync();
                var remaining = 5 - otpRecord.FailedAttempts;
                return BadRequest(new { message = $"Mã xác nhận không đúng. Còn {remaining} lần thử." });
            }

            // OTP đúng -> đánh dấu mã này đã dùng
            otpRecord.IsUsed = true;

            // Sinh token tạm để dùng cho bước đặt mật khẩu mới (sống 10 phút)
            var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("+", "-").Replace("/", "_").Replace("=", "");

            _context.OtpCodes.Add(new OtpCode
            {
                Email = dto.Email,
                Otp = resetToken,
                ExpiredAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                FailedAttempts = 0,
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Xác nhận thành công.", token = resetToken });
        }

        // ========== QUÊN MẬT KHẨU - BƯỚC 3: ĐẶT LẠI MẬT KHẨU (dùng token tạm) ==========
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            // Tìm token hợp lệ
            var otpRecord = _context.OtpCodes
                .FirstOrDefault(o =>
                    o.Email == dto.Email &&
                    o.Otp == dto.Token &&
                    !o.IsUsed &&
                    o.ExpiredAt > DateTime.UtcNow);

            if (otpRecord == null)
                return BadRequest(new { message = "Yêu cầu đặt lại mật khẩu không hợp lệ hoặc đã hết hạn." });

            // Tìm customer
            var customer = _context.Customers.FirstOrDefault(c => c.Email == dto.Email);
            if (customer == null) return NotFound();

            // Hash mật khẩu mới
            var hasher = new PasswordHasher<object>();
            customer.Password = hasher.HashPassword(null, dto.NewPassword);

            // Đánh dấu token đã dùng
            otpRecord.IsUsed = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập." });
        }

        // ========== TẠO JWT TOKEN ==========
        private string GenerateToken(Customer customer)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config["Jwt:Key"] ?? "MaiTrinhSecretKey2026MaiTrinhSecretKey2026"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Name, customer.FullName ?? ""),
                new Claim(ClaimTypes.Email, customer.Email ?? ""),
                new Claim(ClaimTypes.Role, "Customer"),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ========== HÀM PHỤ: SINH MÃ OTP 6 KÝ TỰ CHỮ + SỐ ==========
        private static string GenerateAlphaNumericOtp(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Bỏ I, O, 0, 1 vì dễ nhầm lẫn
            var random = new Random();
            var result = new char[length];
            for (int i = 0; i < length; i++)
                result[i] = chars[random.Next(chars.Length)];
            return new string(result);
        }
    }

    // ========== DTO ==========
    public class SendOtpDto
    {
        public string FullName { get; set; } = ""; // Họ tên khách hàng
        public string Email { get; set; } = "";    // Email nhận OTP
    }

    public class VerifyOtpDto
    {
        public string FullName { get; set; } = "";  // Họ tên khách hàng
        public string Email { get; set; } = "";     // Email xác nhận
        public string Otp { get; set; } = "";       // Mã OTP nhập vào
        public string Password { get; set; } = "";  // Mật khẩu muốn đặt
    }

    public class LoginDto
    {
        public string Email { get; set; } = "";    // Email đăng nhập
        public string Password { get; set; } = ""; // Mật khẩu
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; } = ""; // Email cần reset
    }

    public class VerifyResetOtpDto
    {
        public string Email { get; set; } = ""; // Email cần xác nhận
        public string Otp { get; set; } = "";   // Mã OTP nhập vào
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; } = "";       // Email
        public string Token { get; set; } = "";       // Token tạm nhận từ bước verify-reset-otp
        public string NewPassword { get; set; } = ""; // Mật khẩu mới
    }
}