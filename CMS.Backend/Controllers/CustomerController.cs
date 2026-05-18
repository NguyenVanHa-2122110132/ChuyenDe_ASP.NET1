/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Khách hàng (Customer) - BÀI TẬP 5
              - Lấy dữ liệu khách hàng THẬT từ SQL Server (Bảng Customers)
              - Index: Hiển thị danh sách khách hàng dạng bảng admin
*/

using Microsoft.AspNetCore.Mvc;
using CMS.Data; // Kết nối Database
using System.Linq;

namespace CMS.Backend.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // "Tiêm" kết nối Database vào Controller (Constructor Injection)
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Customer
        public IActionResult Index()
        {
            // Lấy toàn bộ danh sách khách hàng thật từ SQL Server
            var customers = _context.Customers.ToList();
            return View(customers);
        }
    }
}