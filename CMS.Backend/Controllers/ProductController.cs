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
    public class ProductController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== INDEX ==========
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier,Warehouse,Shipper")]
        public IActionResult Index(string? search, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Products
                .Include(p => p.CategoryProducts)
                    .ThenInclude(cp => cp.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Description != null && p.Description.Contains(search));
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var products = query
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Search = search;
            ViewBag.Total = totalItems;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

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

        // ========== API/ACTION LẤY DANH SÁCH SIZE THEO DANH MỤC ==========
        // Hàm này giúp trang giao diện gọi lấy danh sách size chuẩn theo từng loại ngành hàng
        [HttpGet]
        public IActionResult GetSizesByCategory(int categoryId)
        {
            // 1. Tìm tất cả sản phẩm thuộc danh mục được truyền vào thông qua bảng trung gian CategoryProducts
            var rawSizes = _context.Products
                .Include(p => p.CategoryProducts)
                .Where(p => p.CategoryProducts.Any(cp => cp.CategoryId == categoryId))
                .Select(p => p.Sizes)
                .Where(s => !string.IsNullOrEmpty(s)) // Bỏ qua sản phẩm không nhập size
                .ToList();

            // 2. Tiến hành bóc tách chuỗi gộp (ví dụ: "10ml, 50ml, 100ml") thành các ô đơn lẻ
            var finalSizes = rawSizes
                .SelectMany(s => s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Trim()) // Xóa khoảng trắng thừa 2 đầu
                .Distinct()            // Lọc trùng (Chỉ giữ lại 1 chữ 100ml, 1 chữ 50ml...)
                .OrderBy(s => s)       // Sắp xếp lại theo thứ tự ABC
                .ToList();

            return Json(finalSizes);
        }





    }
}