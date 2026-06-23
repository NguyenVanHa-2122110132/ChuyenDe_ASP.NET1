/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Danh mục Sản phẩm (ProductCategory)
              - Index  : Hiển thị danh sách liên kết giữa Danh mục và Sản phẩm
              - Create : Hiển thị form chọn Danh mục và Sản phẩm để tạo liên kết mới
              - Delete : Xóa liên kết giữa Danh mục và Sản phẩm theo cặp khóa
              - Phân quyền: Chỉ Administrator và Admin mới được vào
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Thêm namespace để dùng [Authorize]
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Backend.Controllers
{
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào
    public class ProductCategoryController : BaseAdminController
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public ProductCategoryController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var data = _context.CategoriesProducts      // Lấy danh sách liên kết từ database
                        .Include(cp => cp.Category)     // Join bảng Category
                        .Include(cp => cp.Product)      // Join bảng Product
                        .ToList();
            return View(data); // Truyền danh sách ra View
        }

        // ========== CREATE GET ==========
        [HttpGet]
        public IActionResult Create()
        {
            // Load danh sách danh mục xuống dropdown
            ViewBag.Categories = new SelectList(
                _context.Categories.ToList(), "Id", "Name"
            );
            // Load danh sách sản phẩm xuống dropdown
            ViewBag.Products = new SelectList(
                _context.Products.ToList(), "Id", "Name"
            );
            return View();
        }

        // ========== CREATE POST ==========
        [HttpPost]
        public IActionResult Create(CategoryProduct model)
        {
            // Kiểm tra liên kết đã tồn tại chưa để tránh trùng lặp
            bool exists = _context.CategoriesProducts
                          .Any(cp => cp.CategoryId == model.CategoryId
                                  && cp.ProductId == model.ProductId);
            if (!exists)
            {
                _context.CategoriesProducts.Add(model); // Thêm liên kết mới vào database
                _context.SaveChanges();                  // Lưu thay đổi
            }
            return RedirectToAction("Index"); // Quay về danh sách
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int categoryId, int productId)
        {
            // Tìm liên kết theo cặp khóa CategoryId và ProductId
            var item = _context.CategoriesProducts
                       .FirstOrDefault(cp => cp.CategoryId == categoryId
                                          && cp.ProductId == productId);
            if (item == null) return NotFound(); // Không tìm thấy trả về 404

            _context.CategoriesProducts.Remove(item); // Xóa liên kết khỏi database
            _context.SaveChanges();                    // Lưu thay đổi
            return RedirectToAction("Index");          // Quay về danh sách
        }
    }
}