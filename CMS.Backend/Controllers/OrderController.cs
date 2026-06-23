/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Đơn hàng (Order)
              - Index  : Hiển thị danh sách tất cả đơn hàng kèm tên khách hàng
              - Details: Hiển thị chi tiết đơn hàng kèm danh sách sản phẩm
              - Create : Thêm đơn hàng mới
              - Edit   : Cập nhật trạng thái đơn hàng
              - Delete : Xóa đơn hàng theo ID
              [BẢO MẬT] Tất cả POST có ValidateAntiForgeryToken
              [BẢO MẬT] Edit POST chỉ cập nhật đúng field cho phép
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Backend.Controllers
{
    public class OrderController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== INDEX ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier,Technician,Shipper")]
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ToList();
            return View(orders);
        }

        // ========== DETAILS ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier,Technician,Shipper")]
        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails!)
                    .ThenInclude(od => od!.Product)
                .FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // ========== CREATE GET ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Customers = new SelectList(
                _context.Customers.ToList(), "Id", "FullName");
            return View();
        }

        // ========== CREATE POST ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        [HttpPost]
        [ValidateAntiForgeryToken] //  Chống CSRF
        public IActionResult Create(Order model)
        {
            if (ModelState.IsValid)
            {
                model.OrderDate = DateTime.Now; //  Server tự gán, không tin client
                _context.Orders.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Customers = new SelectList(
                _context.Customers.ToList(), "Id", "FullName");
            return View(model);
        }

        // ========== EDIT GET ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            ViewBag.Customers = new SelectList(
                _context.Customers.ToList(), "Id", "FullName", order.CustomerId);
            return View(order);
        }

        // ========== EDIT POST ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        [HttpPost]
        [ValidateAntiForgeryToken] //  Chống CSRF
        public IActionResult Edit(Order model)
        {
            if (ModelState.IsValid)
            {
                //  Chỉ cập nhật đúng field cho phép, không Update() toàn bộ
                var existing = _context.Orders.Find(model.Id);
                if (existing == null) return NotFound();

                existing.Status = model.Status;     // Cho phép đổi trạng thái
                existing.CustomerId = model.CustomerId; // Cho phép đổi khách hàng
                // OrderDate và TotalAmount KHÔNG cho phép đổi qua form

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Customers = new SelectList(
                _context.Customers.ToList(), "Id", "FullName", model.CustomerId);
            return View(model);
        }

        // ========== DELETE ==========
        [Authorize(Roles = "Administrator,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken] //  Chống CSRF
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}