/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Sản phẩm điện thoại (Product)
              - Index  : Hiển thị danh sách tất cả sản phẩm kèm giá tiền
              - Details: Hiển thị chi tiết cấu hình và giá bán một sản phẩm
              - Create : Hiển thị form và lưu sản phẩm mới vào database
              - Edit   : Hiển thị form và cập nhật thông tin sản phẩm đã có
              - Delete : Xóa sản phẩm theo ID
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public ProductController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var products = _context.Products.ToList(); // Lấy toàn bộ sản phẩm từ database
            return View(products);                     // Truyền danh sách ra View
        }

        // ========== DETAILS ==========
        public IActionResult Details(int id)
        {
            var product = _context.Products
                          .FirstOrDefault(p => p.Id == id); // Tìm sản phẩm theo ID
            if (product == null) return NotFound();          // Không tìm thấy trả về 404
            return View(product);
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Hiển thị form thêm sản phẩm mới
        }

        // ========== CREATE POST ==========
        [HttpPost]
        public IActionResult Create(Product model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                _context.Products.Add(model);     // Thêm sản phẩm mới vào database
                _context.SaveChanges();            // Lưu thay đổi
                return RedirectToAction("Index");  // Quay về danh sách
            }
            return View(model); // Nếu lỗi thì hiển thị lại form
        }

        // ========== EDIT GET ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id); // Tìm sản phẩm theo ID
            if (product == null) return NotFound();    // Không tìm thấy trả về 404
            return View(product); // Hiển thị form với dữ liệu cũ
        }

        // ========== EDIT POST ==========
        [HttpPost]
        public IActionResult Edit(Product model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                _context.Products.Update(model);   // Cập nhật sản phẩm trong database
                _context.SaveChanges();             // Lưu thay đổi
                return RedirectToAction("Index");   // Quay về danh sách
            }
            return View(model); // Nếu lỗi thì hiển thị lại form
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id); // Tìm sản phẩm theo ID
            if (product == null) return NotFound();    // Không tìm thấy trả về 404

            _context.Products.Remove(product); // Xóa sản phẩm khỏi database
            _context.SaveChanges();             // Lưu thay đổi
            return RedirectToAction("Index");   // Quay về danh sách
        }
    }
}