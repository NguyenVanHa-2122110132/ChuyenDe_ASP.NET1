/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : Controller trang chủ của hệ thống CMS
              - Index     : Trang chủ công khai - hiển thị 3 bài viết mới nhất
              - Dashboard : Trang bảng điều khiển Admin - thống kê số lượng danh mục, bài viết, thành viên
              - Privacy   : Trang chính sách bảo mật
              - Error     : Trang xử lý lỗi hệ thống
*/
using CMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Thêm namespace để dùng [Authorize]
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using System.Diagnostics;

namespace CMS.Backend.Controllers
{
    public class HomeController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public HomeController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // Trang chủ công khai - không cần đăng nhập
        public IActionResult Index()
        {
            // Lấy 3 bài viết mới nhất, kèm thông tin danh mục
            var latestPosts = _context.Posts
                              .Include(p => p.Category)
                              .OrderByDescending(p => p.CreatedDate)
                              .Take(3)
                              .ToList();
            return View(latestPosts);
        }

        // Dashboard: chỉ Administrator, Admin, Sales, Cashier mới được vào
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        public IActionResult Dashboard()
        {
            ViewBag.TotalCategories = _context.Categories.Count(); // Đếm tổng số danh mục
            ViewBag.TotalPosts = _context.Posts.Count();           // Đếm tổng số bài viết
            ViewBag.TotalCustomers = _context.Customers.Count();       // Đếm tổng số khách hàng
            ViewBag.TotalCoupons = _context.Coupons.Count();           // ➕ Thêm
            ViewBag.TotalInventory = _context.Inventories.Count();     // ➕ Thêm
            ViewBag.TotalComments = _context.Comments.Count();         // ➕ Thêm
            return View();
        }

        // Trang chính sách bảo mật - công khai
        public IActionResult Privacy() => View();

        // Trang xử lý lỗi hệ thống - không cache
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // ========== ACTION CỬA HÀNG (SHOP) - CÔNG KHAI ==========
        // Tuyến đường xử lý: /Home/Shop hoặc /Home/Shop?categoryId=1032
        public IActionResult Shop(int? categoryId)
        {
            // 1. Khởi tạo câu lệnh truy vấn cơ bản: Lấy sản phẩm kèm theo bảng danh mục liên kết
            var query = _context.Products
                .Include(p => p.CategoryProducts)
                .AsQueryable();

            // 2. Nếu trên URL có truyền categoryId (Ví dụ: ?categoryId=1032) thì tiến hành lọc dữ liệu
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryProducts.Any(cp => cp.CategoryId == categoryId.Value));
            }

            // 3. Thực thi câu lệnh, chuyển dữ liệu thành danh sách (List)
            var products = query.ToList();

            // 4. Trả về file giao diện Shop.cshtml kèm theo danh sách sản phẩm đã lọc
            return View(products);
        }
    }
}