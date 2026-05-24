/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Chi tiết Đơn hàng (OrderDetail)
              - Index  : Hiển thị danh sách tất cả chi tiết đơn hàng kèm tên sản phẩm
              - Create : Hiển thị form và lưu chi tiết đơn hàng mới vào database
              - Edit   : Hiển thị form và cập nhật số lượng, đơn giá
              - Delete : Xóa chi tiết đơn hàng theo ID
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Backend.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public OrderDetailController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var orderDetails = _context.OrderDetails
                .Include(od => od.Order)    // Join bảng Order để lấy thông tin đơn hàng
                .Include(od => od.Product)  // Join bảng Product để lấy tên sản phẩm
                .ToList();
            return View(orderDetails); // Truyền danh sách ra View
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            // Load danh sách đơn hàng xuống dropdown
            ViewBag.Orders = new SelectList(
                _context.Orders.ToList(), "Id", "Id"
            );
            // Load danh sách sản phẩm xuống dropdown
            ViewBag.Products = new SelectList(
                _context.Products.ToList(), "Id", "Name"
            );
            return View(); // Hiển thị form thêm chi tiết đơn hàng mới
        }

        // ========== CREATE POST ==========
        [HttpPost]
        public IActionResult Create(OrderDetail model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                _context.OrderDetails.Add(model);  // Thêm chi tiết đơn hàng vào database
                _context.SaveChanges();             // Lưu thay đổi
                return RedirectToAction("Index");   // Quay về danh sách
            }
            ViewBag.Orders = new SelectList(
                _context.Orders.ToList(), "Id", "Id"
            );
            ViewBag.Products = new SelectList(
                _context.Products.ToList(), "Id", "Name"
            );
            return View(model); // Nếu lỗi thì hiển thị lại form
        }

        // ========== EDIT GET ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var orderDetail = _context.OrderDetails.Find(id); // Tìm chi tiết đơn hàng theo ID
            if (orderDetail == null) return NotFound();        // Không tìm thấy trả về 404

            ViewBag.Orders = new SelectList(
                _context.Orders.ToList(), "Id", "Id", orderDetail.OrderId
            );
            ViewBag.Products = new SelectList(
                _context.Products.ToList(), "Id", "Name", orderDetail.ProductId
            );
            return View(orderDetail); // Hiển thị form với dữ liệu cũ
        }

        // ========== EDIT POST ==========
        [HttpPost]
        public IActionResult Edit(OrderDetail model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                _context.OrderDetails.Update(model); // Cập nhật chi tiết đơn hàng
                _context.SaveChanges();               // Lưu thay đổi
                return RedirectToAction("Index");     // Quay về danh sách
            }
            ViewBag.Orders = new SelectList(
                _context.Orders.ToList(), "Id", "Id", model.OrderId
            );
            ViewBag.Products = new SelectList(
                _context.Products.ToList(), "Id", "Name", model.ProductId
            );
            return View(model); // Nếu lỗi thì hiển thị lại form
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var orderDetail = _context.OrderDetails.Find(id); // Tìm chi tiết đơn hàng theo ID
            if (orderDetail == null) return NotFound();        // Không tìm thấy trả về 404

            _context.OrderDetails.Remove(orderDetail); // Xóa chi tiết đơn hàng
            _context.SaveChanges();                     // Lưu thay đổi
            return RedirectToAction("Index");           // Quay về danh sách
        }
    }
}