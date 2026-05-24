/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Đơn hàng (Order)
              - Index  : Hiển thị danh sách tất cả đơn hàng kèm tên khách hàng
              - Details: Hiển thị chi tiết đơn hàng kèm danh sách sản phẩm
              - Create : Hiển thị form và lưu đơn hàng mới vào database
              - Edit   : Hiển thị form và cập nhật trạng thái đơn hàng
              - Delete : Xóa đơn hàng theo ID
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Backend.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public OrderController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.Customer)      // Join bảng Customer để lấy tên khách hàng
                .Include(o => o.OrderDetails)  // Join bảng OrderDetail để đếm số sản phẩm
                .ToList();
            return View(orders); // Truyền danh sách ra View
        }

        // ========== DETAILS ==========
        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.Customer)           // Join bảng Customer
               .Include(o => o.OrderDetails!)       // Join bảng OrderDetail
                    .ThenInclude(od => od!.Product)  // Join tiếp bảng Product
                .FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();    // Không tìm thấy trả về 404
            return View(order);
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            // Load danh sách khách hàng xuống dropdown
            ViewBag.Customers = new SelectList(
                _context.Customers.ToList(), "Id", "FullName"
            );
            return View(); // Hiển thị form thêm đơn hàng mới
        }

        // ========== CREATE POST ==========
        [HttpPost]
        public IActionResult Create(Order model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                model.OrderDate = DateTime.Now;   // Tự động gán ngày đặt hàng
                _context.Orders.Add(model);        // Thêm đơn hàng mới vào database
                _context.SaveChanges();             // Lưu thay đổi
                return RedirectToAction("Index");   // Quay về danh sách
            }
            ViewBag.Customers = new SelectList(
                _context.Customers.ToList(), "Id", "FullName"
            );
            return View(model); // Nếu lỗi thì hiển thị lại form
        }

        // ========== EDIT GET ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _context.Orders.Find(id); // Tìm đơn hàng theo ID
            if (order == null) return NotFound();  // Không tìm thấy trả về 404

            // Load danh sách khách hàng, tự chọn đúng khách hàng cũ
            ViewBag.Customers = new SelectList(
                _context.Customers.ToList(), "Id", "FullName", order.CustomerId
            );
            return View(order); // Hiển thị form với dữ liệu cũ
        }

        // ========== EDIT POST ==========
        [HttpPost]
        public IActionResult Edit(Order model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                _context.Orders.Update(model);    // Cập nhật đơn hàng trong database
                _context.SaveChanges();            // Lưu thay đổi
                return RedirectToAction("Index");  // Quay về danh sách
            }
            ViewBag.Customers = new SelectList(
                _context.Customers.ToList(), "Id", "FullName", model.CustomerId
            );
            return View(model); // Nếu lỗi thì hiển thị lại form
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id); // Tìm đơn hàng theo ID
            if (order == null) return NotFound();  // Không tìm thấy trả về 404

            _context.Orders.Remove(order); // Xóa đơn hàng khỏi database
            _context.SaveChanges();         // Lưu thay đổi
            return RedirectToAction("Index"); // Quay về danh sách
        }
    }
}