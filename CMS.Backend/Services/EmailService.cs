/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 05/06/2026
    Mô tả    : Service gửi email qua Gmail SMTP dùng MailKit
              - IEmailService  : Interface định nghĩa các phương thức gửi email
              - EmailService   : Triển khai gửi email thực tế
              - SendEmailAsync        : Gửi email cơ bản
              - SendOtpEmailAsync     : Gửi mã OTP xác nhận đăng ký
              - SendResetPasswordEmailAsync : Gửi link đặt lại mật khẩu
              - SendOrderConfirmEmailAsync  : Gửi xác nhận đơn hàng
*/
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CMS.Backend.Services
{
    // Interface định nghĩa các phương thức gửi email
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody);
        Task SendOtpEmailAsync(string toEmail, string toName, string otp);
        Task SendResetPasswordEmailAsync(string toEmail, string toName, string resetLink);
        Task SendOrderConfirmEmailAsync(string toEmail, string toName, string orderId, decimal total);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config; // Đọc cấu hình từ appsettings.json

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // ========== GỬI EMAIL CƠ BẢN ==========
        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var settings = _config.GetSection("EmailSettings"); // Lấy cấu hình email

            // Đọc cấu hình, trim để tránh dấu cách thừa
            var smtpHost = settings["SmtpHost"]!.Trim();
            var smtpPort = int.Parse(settings["SmtpPort"]!.Trim());
            var senderEmail = settings["SenderEmail"]!.Trim();
            var senderName = settings["SenderName"]!.Trim();
            var appPassword = settings["AppPassword"]!.Trim();

            // Tạo nội dung email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail)); // Người gửi
            message.To.Add(new MailboxAddress(toName, toEmail));           // Người nhận
            message.Subject = subject;                                      // Tiêu đề

            var builder = new BodyBuilder { HtmlBody = htmlBody };          // Nội dung HTML
            message.Body = builder.ToMessageBody();

            // Kết nối và gửi email qua Gmail SMTP
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(senderEmail, appPassword); // Xác thực
            await smtp.SendAsync(message);    // Gửi email
            await smtp.DisconnectAsync(true); // Ngắt kết nối
        }
        // ========== GỬI OTP ĐĂNG KÝ ==========
        public async Task SendOtpEmailAsync(string toEmail, string toName, string otp)
        {
            var html = $@"
            <div style='font-family:Arial,sans-serif;max-width:500px;margin:auto;border:1px solid #e4ddd4;border-radius:8px;overflow:hidden'>
                <div style='background:#1a1a1a;padding:24px;text-align:center'>
                    <h1 style='color:#b8975a;font-size:1.4rem;margin:0;letter-spacing:3px'>MAI TRINH STUDIO</h1>
                </div>
                <div style='padding:32px'>
                    <h2 style='color:#1a1a1a;font-size:1.1rem;margin:0 0 16px'>Xác nhận đăng ký tài khoản</h2>
                    <p style='color:#8a8178'>Xin chào <strong>{toName}</strong>,</p>
                    <p style='color:#8a8178'>Mã OTP xác nhận tài khoản của bạn là:</p>
                    <div style='background:#f7f3ee;border:2px dashed #b8975a;border-radius:8px;padding:20px;text-align:center;margin:20px 0'>
                        <span style='font-size:2.5rem;font-weight:bold;color:#1a1a1a;letter-spacing:8px'>{otp}</span>
                    </div>
                    <p style='color:#8a8178;font-size:0.8rem'>⏱ Mã có hiệu lực trong <strong>5 phút</strong>. Vui lòng không chia sẻ mã này với ai.</p>
                </div>
                <div style='background:#f7f3ee;padding:16px;text-align:center'>
                    <p style='color:#8a8178;font-size:0.75rem;margin:0'>© 2026 Mai Trinh Studio</p>
                </div>
            </div>";

            await SendEmailAsync(toEmail, toName, "🔐 Mã OTP xác nhận tài khoản - Mai Trinh Studio", html);
        }

        // ========== GỬI RESET PASSWORD ==========
        public async Task SendResetPasswordEmailAsync(string toEmail, string toName, string resetLink)
        {
            var html = $@"
            <div style='font-family:Arial,sans-serif;max-width:500px;margin:auto;border:1px solid #e4ddd4;border-radius:8px;overflow:hidden'>
                <div style='background:#1a1a1a;padding:24px;text-align:center'>
                    <h1 style='color:#b8975a;font-size:1.4rem;margin:0;letter-spacing:3px'>MAI TRINH STUDIO</h1>
                </div>
                <div style='padding:32px'>
                    <h2 style='color:#1a1a1a;font-size:1.1rem;margin:0 0 16px'>Đặt lại mật khẩu</h2>
                    <p style='color:#8a8178'>Xin chào <strong>{toName}</strong>,</p>
                    <p style='color:#8a8178'>Nhấn nút bên dưới để đặt lại mật khẩu. Link có hiệu lực trong <strong>15 phút</strong>.</p>
                    <div style='text-align:center;margin:28px 0'>
                        <a href='{resetLink}' style='background:#1a1a1a;color:#fff;padding:14px 32px;text-decoration:none;font-size:0.85rem;letter-spacing:2px;text-transform:uppercase'>
                            ĐẶT LẠI MẬT KHẨU
                        </a>
                    </div>
                    <p style='color:#8a8178;font-size:0.8rem'>Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>
                </div>
                <div style='background:#f7f3ee;padding:16px;text-align:center'>
                    <p style='color:#8a8178;font-size:0.75rem;margin:0'>© 2026 Mai Trinh Studio</p>
                </div>
            </div>";

            await SendEmailAsync(toEmail, toName, "🔑 Đặt lại mật khẩu - Mai Trinh Studio", html);
        }

        // ========== GỬI XÁC NHẬN ĐƠN HÀNG ==========
        public async Task SendOrderConfirmEmailAsync(string toEmail, string toName, string orderId, decimal total)
        {
            var html = $@"
            <div style='font-family:Arial,sans-serif;max-width:500px;margin:auto;border:1px solid #e4ddd4;border-radius:8px;overflow:hidden'>
                <div style='background:#1a1a1a;padding:24px;text-align:center'>
                    <h1 style='color:#b8975a;font-size:1.4rem;margin:0;letter-spacing:3px'>MAI TRINH STUDIO</h1>
                </div>
                <div style='padding:32px'>
                    <h2 style='color:#1a1a1a;font-size:1.1rem;margin:0 0 16px'>✅ Đặt hàng thành công!</h2>
                    <p style='color:#8a8178'>Xin chào <strong>{toName}</strong>,</p>
                    <p style='color:#8a8178'>Đơn hàng của bạn đã được xác nhận.</p>
                    <div style='background:#f7f3ee;border-left:4px solid #b8975a;padding:16px 20px;margin:20px 0'>
                        <p style='margin:0 0 8px;font-size:0.85rem;color:#1a1a1a'><strong>Mã đơn hàng:</strong> #{orderId}</p>
                        <p style='margin:0;font-size:0.85rem;color:#1a1a1a'><strong>Tổng tiền:</strong> {total.ToString("N0")}₫</p>
                    </div>
                    <p style='color:#8a8178;font-size:0.8rem'>Cảm ơn bạn đã tin tưởng Mai Trinh Studio!</p>
                </div>
                <div style='background:#f7f3ee;padding:16px;text-align:center'>
                    <p style='color:#8a8178;font-size:0.75rem;margin:0'>© 2026 Mai Trinh Studio</p>
                </div>
            </div>";

            await SendEmailAsync(toEmail, toName, $"✅ Xác nhận đơn hàng #{orderId} - Mai Trinh Studio", html);
        }
    }
}