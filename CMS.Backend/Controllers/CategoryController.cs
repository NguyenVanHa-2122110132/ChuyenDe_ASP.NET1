/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : Controller quản lý Danh mục (Category)
              - Index : Hiển thị danh sách danh mục
              - Create: Hiển thị form và lưu danh mục mới vào database
              - Edit  : Hiển thị form và cập nhật danh mục đã có
              - Delete: Xóa danh mục (có kiểm tra ràng buộc bài viết trước khi xóa)
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== INDEX ==========
        public IActionResult Index()
        {
            var data = _context.Categories.ToList();
            return View(data);
        }

        // ========== CREATE ==========
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // ========== EDIT ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();

            bool cobaiviet = _context.Posts.Any(p => p.CategoryId == id);
            if (cobaiviet)
            {
                TempData["ErrorMessage"] = "Không thể xóa! Danh mục này đang có bài viết bên trong. Hãy xóa hết bài viết trước.";
                return RedirectToAction("Index");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}