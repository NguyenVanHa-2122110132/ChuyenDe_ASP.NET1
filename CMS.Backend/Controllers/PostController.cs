/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : Controller quản lý Bài viết/Điện thoại (Post)
              - Index  : Hiển thị danh sách tất cả bài viết, có thể lọc theo danh mục
              - Details: Hiển thị chi tiết một bài viết theo ID
              - Create : Hiển thị form và lưu bài viết mới vào database
              - Edit   : Hiển thị form và cập nhật bài viết đã có
              - Delete : Xóa bài viết theo ID
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Backend.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== INDEX ==========
        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                var allPosts = _context.Posts
                                .OrderByDescending(p => p.CreatedDate)
                                .Include(p => p.Category)
                                .ToList();
                return View(allPosts);
            }

            var posts = _context.Posts
                        .Where(p => p.CategoryId == id)
                        .OrderByDescending(p => p.CreatedDate)
                        .Include(p => p.Category)
                        .ToList();
            return View(posts);
        }

        // ========== DETAILS ==========
        public IActionResult Details(int id)
        {
            var post = _context.Posts
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (post == null) return NotFound();
            return View(post);
        }

        // ========== CREATE ==========
        [HttpGet]
        public IActionResult Create()
        {
            // Load danh sách danh mục xuống dropdown
            ViewBag.Categories = new SelectList(
                _context.Categories.ToList(), "Id", "Name"
            );
            return View();
        }

        [HttpPost]
        public IActionResult Create(Post model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                _context.Posts.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(
                _context.Categories.ToList(), "Id", "Name"
            );
            return View(model);
        }

        // ========== EDIT ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound();

            // Load danh sách danh mục, tự chọn đúng danh mục cũ
            ViewBag.Categories = new SelectList(
                _context.Categories.ToList(), "Id", "Name", post.CategoryId
            );
            return View(post);
        }

        [HttpPost]
        public IActionResult Edit(Post model)
        {
            if (ModelState.IsValid)
            {
                _context.Posts.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(
                _context.Categories.ToList(), "Id", "Name", model.CategoryId
            );
            return View(model);
        }

        // ========== DELETE ==========
        // Dùng GET để thẻ <a> gọi được trực tiếp
        public IActionResult Delete(int id)
        {
            var post = _context.Posts.Find(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}