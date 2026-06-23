/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Khách hàng (Customer)
              - Index  : Hiển thị danh sách tất cả khách hàng
              - Detail : Xem chi tiết khách hàng
              - Create : Thêm khách hàng mới
              - Edit   : Cập nhật thông tin khách hàng
              - Delete : Xóa khách hàng theo ID
              [BẢO MẬT] Password luôn được hash trước khi lưu
              [BẢO MẬT] Tất cả POST có ValidateAntiForgeryToken
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CMS.Data;
using CMS.Data.Entities;

namespace CMS.Backend.Controllers
{
    public class CustomerController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== INDEX ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier,Technician,Shipper")]
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }

        // ========== DETAIL ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier,Technician,Shipper")]
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // ========== CREATE GET ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ========== CREATE POST ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        [HttpPost]
        [ValidateAntiForgeryToken] //  Chống CSRF
        public IActionResult Create(Customer model)
        {
            if (ModelState.IsValid)
            {
                //  Hash password trước khi lưu
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var hasher = new PasswordHasher<object>();
                    model.Password = hasher.HashPassword(null, model.Password);
                }

                _context.Customers.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // ========== EDIT GET ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            // ✅ Không truyền password hash ra form
            customer.Password = "";
            return View(customer);
        }

        // ========== EDIT POST ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống CSRF
        public IActionResult Edit(int id, Customer model)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Customers.Find(id);
                if (existing == null) return NotFound();

                existing.FullName = model.FullName;
                existing.Email = model.Email;
                existing.Phone = model.Phone;
                existing.Address = model.Address;

                //  Chỉ đổi password nếu nhập mới + hash trước khi lưu
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var hasher = new PasswordHasher<object>();
                    existing.Password = hasher.HashPassword(null, model.Password);
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }


        // ========== DELETE ==========
        [Authorize(Roles = "Administrator,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống CSRF
        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            // Xóa tài khoản Identity theo Email để người đó không đăng nhập được nữa
            // Người dùng mới có thể tạo lại tài khoản với email đó
            if (!string.IsNullOrEmpty(customer.Email))
            {
                var user = _context.Users
                           .FirstOrDefault(u => u.Email == customer.Email);
                if (user != null)
                {
                    _context.Users.Remove(user);
                }
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    
    }
}