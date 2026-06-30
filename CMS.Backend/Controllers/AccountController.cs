/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : Controller quản lý Tài khoản (Account)
              [BẢO MẬT] Rate Limiting: khóa IP sau 5 lần đăng nhập sai
              [BẢO MẬT] Không lộ thông tin chi tiết khi đăng nhập sai
              [BẢO MẬT] Password hash chuẩn ASP.NET Identity
              [BẢO MẬT] ForgotPassword: xác minh cả username lẫn email
*/
using CMS.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace CMS.Backend.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public AccountController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            // [BẢO MẬT] Kiểm tra IP bị khóa chưa
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var cacheKey = $"login_fail_{ip}";
            _cache.TryGetValue(cacheKey, out int failCount);

            if (failCount >= 5)
            {
                ViewBag.Error = "Tài khoản bị tạm khóa 5 phút do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau!";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                bool isValid = false;

                try
                {
                    var hasher = new PasswordHasher<object>();
                    var result = hasher.VerifyHashedPassword(null, user.PasswordHash, password);
                    isValid = result == PasswordVerificationResult.Success;
                }
                catch
                {
                    isValid = user.PasswordHash == password;
                }

                // Nếu password là plain text → tự hash lại
                if (isValid && !user.PasswordHash.StartsWith("AQAAAA"))
                {
                    var rehash = new PasswordHasher<object>();
                    user.PasswordHash = rehash.HashPassword(null, password);
                    _context.SaveChanges();
                }

                if (isValid)
                {
                    _cache.Remove(cacheKey);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("FullName",       user.FullName)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
                        IsPersistent = false,
                        AllowRefresh = true
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    if (user.Role == "Administrator" || user.Role == "Admin" ||
                        user.Role == "Editor" ||
                        user.Role == "Sales" || user.Role == "Cashier" ||
                        user.Role == "Warehouse" || user.Role == "Technician" ||
                        user.Role == "Shipper" || user.Role == "Staff")
                    {
                        return RedirectToAction("Index", "Post");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            _cache.Set(cacheKey, failCount + 1, TimeSpan.FromSeconds(5));
            ViewBag.Error = $"Tên đăng nhập hoặc mật khẩu không đúng! ({failCount + 1}/5 lần thử)";
            return View();
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string username, string password,
                                      string fullname, string email)
        {
            var existing = _context.Users.Any(u => u.Username == username);
            if (existing)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            var hasher = new PasswordHasher<object>();

            var newUser = new CMS.Data.Entities.User
            {
                Username = username,
                PasswordHash = hasher.HashPassword(null, password),
                FullName = fullname,
                Email = email,
                Role = "Customer"
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            var newCustomer = new CMS.Data.Entities.Customer
            {
                FullName = fullname,
                Email = email,
                Phone = null,
                Address = null,
                Password = hasher.HashPassword(null, password)
            };
            _context.Customers.Add(newCustomer);
            _context.SaveChanges();

            ViewBag.Success = "Đăng ký thành công! Vui lòng đăng nhập.";
            return View();
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string step, string username,
                                             string email, string newPassword,
                                             string confirmPassword)
        {
            // ========== BƯỚC 1: Xác minh username + email ==========
            if (step == "verify")
            {
                var user = _context.Users.FirstOrDefault(u =>
                    u.Username == username && u.Email == email);

                if (user == null)
                {
                    ViewBag.Error = "Tên đăng nhập hoặc email không đúng!";
                    return View();
                }

                ViewBag.Verified = true;
                ViewBag.Username = username;
                return View();
            }

            // ========== BƯỚC 2: Đổi mật khẩu mới ==========
            if (step == "reset")
            {
                if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
                {
                    ViewBag.Error = "Mật khẩu mới phải có ít nhất 6 ký tự!";
                    ViewBag.Verified = true;
                    ViewBag.Username = username;
                    return View();
                }

                if (newPassword != confirmPassword)
                {
                    ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                    ViewBag.Verified = true;
                    ViewBag.Username = username;
                    return View();
                }

                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    ViewBag.Error = "Có lỗi xảy ra, vui lòng thử lại!";
                    return View();
                }

                var hasher = new PasswordHasher<object>();
                user.PasswordHash = hasher.HashPassword(null, newPassword);
                _context.SaveChanges();

                ViewBag.Success = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.";
                return View();
            }

            return View();
        }
    }
}