/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Sản phẩm điện thoại (Product)
              - Index  : Hiển thị danh sách tất cả sản phẩm kèm giá tiền
              - Details: Hiển thị chi tiết cấu hình và giá bán một sản phẩm
              - Create : Thêm sản phẩm mới
              - Edit   : Cập nhật thông tin sản phẩm
              - Delete : Xóa sản phẩm theo ID
              [BẢO MẬT] Tất cả POST có ValidateAntiForgeryToken
              [BẢO MẬT] Edit POST chỉ cập nhật đúng field cho phép
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== INDEX ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier,Warehouse,Shipper")]
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }


        // ========== INDEX ADMIN ==========
        [Authorize(Roles = "Administrator,Admin,Warehouse")]
        public IActionResult IndexAdmin()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // ========== DETAILS ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier,Warehouse,Shipper")]
        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // ========== CREATE GET ==========
        [Authorize(Roles = "Administrator,Admin,Warehouse")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ========== EDIT GET ==========
        [Authorize(Roles = "Administrator,Admin,Warehouse")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // ========== CREATE POST ==========
        [Authorize(Roles = "Administrator,Admin,Warehouse")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Dùng tên file gốc, không đổi tên
                    var fileName = ImageFile.FileName;
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    // Chỉ lưu nếu file chưa tồn tại
                    if (!System.IO.File.Exists(savePath))
                    {
                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                    }
                    model.ImageUrl = "/images/" + fileName;
                }

                _context.Products.Add(model);
                _context.SaveChanges();
                return RedirectToAction("IndexAdmin");
            }
            return View(model);
        }

        // ========== EDIT POST ==========
        [Authorize(Roles = "Administrator,Admin,Warehouse")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product model, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Products.Find(model.Id);
                if (existing == null) return NotFound();

                existing.Name = model.Name;
                existing.Price = model.Price;
                existing.Description = model.Description;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Dùng tên file gốc, không đổi tên
                    var fileName = ImageFile.FileName;
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    // Chỉ lưu nếu file chưa tồn tại
                    if (!System.IO.File.Exists(savePath))
                    {
                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                    }
                    existing.ImageUrl = "/images/" + fileName;
                }

                _context.SaveChanges();
                return RedirectToAction("IndexAdmin");
            }
            return View(model);
        }

        // ========== DELETE ==========
        [Authorize(Roles = "Administrator,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken] //  Chống CSRF
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}