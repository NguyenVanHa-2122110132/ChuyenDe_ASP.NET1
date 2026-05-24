/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Khách hàng (Customer)
              - Index  : Hiển thị danh sách tất cả khách hàng
              - Create : Hiển thị form và lưu khách hàng mới vào database
              - Edit   : Hiển thị form và cập nhật thông tin khách hàng đã có
                         Nếu không nhập mật khẩu mới thì giữ nguyên mật khẩu cũ
              - Delete : Xóa khách hàng theo ID
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;

namespace CMS.Backend.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public CustomerController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList(); // Lấy toàn bộ khách hàng từ database
            return View(customers);                      // Truyền danh sách ra View
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Hiển thị form thêm khách hàng mới
        }

        // ========== CREATE POST ==========
        [HttpPost]
        public IActionResult Create(Customer model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                _context.Customers.Add(model);    // Thêm khách hàng mới vào database
                _context.SaveChanges();            // Lưu thay đổi
                return RedirectToAction("Index");  // Quay về danh sách
            }
            return View(model); // Nếu lỗi thì hiển thị lại form
        }

        // ========== EDIT GET ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer = _context.Customers.Find(id); // Tìm khách hàng theo ID
            if (customer == null) return NotFound();     // Không tìm thấy trả về 404
            return View(customer); // Hiển thị form với dữ liệu cũ
        }

        // ========== EDIT POST ==========
        [HttpPost]
        public IActionResult Edit(int id, Customer model)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Customers.Find(id); // Lấy khách hàng cũ từ database
                if (existing == null) return NotFound();

                existing.FullName = model.FullName; // Cập nhật họ tên
                existing.Email = model.Email;    // Cập nhật email
                existing.Phone = model.Phone;    // Cập nhật số điện thoại
                existing.Address = model.Address;  // Cập nhật địa chỉ

                // Chỉ đổi mật khẩu nếu người dùng có nhập mới
                // Nếu để trống thì giữ nguyên mật khẩu cũ trong database
                if (!string.IsNullOrEmpty(model.Password))
                {
                    existing.Password = model.Password;
                }

                _context.SaveChanges();           // Lưu thay đổi vào database
                return RedirectToAction("Index"); // Quay về danh sách
            }
            return View(model); // Nếu lỗi thì hiển thị lại form
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Find(id); // Tìm khách hàng theo ID
            if (customer == null) return NotFound();     // Không tìm thấy trả về 404

            _context.Customers.Remove(customer); // Xóa khách hàng khỏi database
            _context.SaveChanges();               // Lưu thay đổi
            return RedirectToAction("Index");     // Quay về danh sách
        }
    }
}