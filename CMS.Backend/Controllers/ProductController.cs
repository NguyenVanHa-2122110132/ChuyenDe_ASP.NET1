/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Sản phẩm điện thoại để bán (Product)
              - Lấy dữ liệu sản phẩm THẬT từ SQL Server (Bảng Products)
              - Index: Trang danh sách sản phẩm hiển thị giá tiền
              - Details: Trang xem chi tiết cấu hình và giá bán sản phẩm
*/

using Microsoft.AspNetCore.Mvc;
using CMS.Data; // Để kết nối Database
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CMS.Backend.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        // "Tiêm" kết nối Database vào Controller (Constructor Injection)
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Product (Hiển thị danh sách sản phẩm điện thoại để bán)
        public IActionResult Index()
        {
            var products = _context.Products.ToList(); // Lấy sản phẩm thật từ SQL
            return View(products);
        }

        // GET: /Product/Details/1 (Xem chi tiết sản phẩm)
        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(); // Trả về trang 404 nếu sai ID
            }
            return View(product);
        }
    }
}