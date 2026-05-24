/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Controller quản lý Danh mục Sản phẩm (ProductCategory)
              - Index  : Hiển thị danh sách liên kết giữa Danh mục và Sản phẩm
              - Create : Hiển thị form chọn Danh mục và Sản phẩm để tạo liên kết mới
              - Delete : Xóa liên kết giữa Danh mục và Sản phẩm theo cặp khóa
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Backend.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.CategoriesProducts          // ← đúng tên DbSet
                        .Include(cp => cp.Category)
                        .Include(cp => cp.Product)
                        .ToList();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(
                _context.Categories.ToList(), "Id", "Name"
            );
            ViewBag.Products = new SelectList(
                _context.Products.ToList(), "Id", "Name"
            );
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryProduct model)
        {
            bool exists = _context.CategoriesProducts       // ← đúng tên DbSet
                          .Any(cp => cp.CategoryId == model.CategoryId
                                  && cp.ProductId == model.ProductId);
            if (!exists)
            {
                _context.CategoriesProducts.Add(model);     // ← đúng tên DbSet
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int categoryId, int productId)
        {
            var item = _context.CategoriesProducts          // ← đúng tên DbSet
                       .FirstOrDefault(cp => cp.CategoryId == categoryId
                                          && cp.ProductId == productId);
            if (item == null) return NotFound();

            _context.CategoriesProducts.Remove(item);       // ← đúng tên DbSet
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}