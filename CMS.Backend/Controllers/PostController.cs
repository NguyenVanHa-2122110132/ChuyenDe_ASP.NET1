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
              - Phân quyền: Administrator, Admin toàn quyền. Sales, Cashier chỉ được xem
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Thêm namespace để dùng [Authorize]
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Backend.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public PostController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX ==========
        // Administrator, Admin, Sales, Cashier đều được xem danh sách bài viết
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                // Lấy tất cả bài viết, sắp xếp mới nhất lên đầu, kèm thông tin danh mục
                var allPosts = _context.Posts
                                .OrderByDescending(p => p.CreatedDate)
                                .Include(p => p.Category)
                                .ToList();
                return View(allPosts);
            }

            // Lọc bài viết theo danh mục nếu có truyền id
            var posts = _context.Posts
                        .Where(p => p.CategoryId == id)
                        .OrderByDescending(p => p.CreatedDate)
                        .Include(p => p.Category)
                        .ToList();
            return View(posts);
        }

        // ========== DETAILS ==========
        // Administrator, Admin, Sales, Cashier đều được xem chi tiết bài viết
        [Authorize(Roles = "Administrator,Admin,Sales,Cashier")]
        public IActionResult Details(int id)
        {
            var post = _context.Posts
                .Include(p => p.Category)           // Join bảng Category để lấy tên danh mục
                .FirstOrDefault(p => p.Id == id);   // Tìm bài viết theo ID

            if (post == null) return NotFound(); // Không tìm thấy trả về 404
            return View(post);
        }

        // ========== CREATE GET ==========
        // Chỉ Administrator và Admin mới được thêm bài viết
        [Authorize(Roles = "Administrator,Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            // Load danh sách danh mục xuống dropdown
            ViewBag.Categories = new SelectList(
                _context.Categories.ToList(), "Id", "Name"
            );
            return View();
        }

        // ========== EDIT GET ==========
        // Chỉ Administrator và Admin mới được sửa bài viết
        [Authorize(Roles = "Administrator,Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var post = _context.Posts.Find(id);   // Tìm bài viết theo ID
            if (post == null) return NotFound();   // Không tìm thấy trả về 404

            // Load danh sách danh mục, tự chọn đúng danh mục cũ
            ViewBag.Categories = new SelectList(
                _context.Categories.ToList(), "Id", "Name", post.CategoryId
            );
            return View(post);
        }

        // ========== CREATE POST ==========
        [Authorize(Roles = "Administrator,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Post model, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = ImageFile.FileName;
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    if (!System.IO.File.Exists(savePath))
                    {
                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                    }
                    model.ImageUrl = "/images/" + fileName;
                }

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

        // ========== EDIT POST ==========
        [Authorize(Roles = "Administrator,Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Post model, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = ImageFile.FileName;
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    if (!System.IO.File.Exists(savePath))
                    {
                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                    }
                    model.ImageUrl = "/images/" + fileName;
                }

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
        // Chỉ Administrator và Admin mới được xóa bài viết
        [Authorize(Roles = "Administrator,Admin")]
        public IActionResult Delete(int id)
        {
            var post = _context.Posts.Find(id); // Tìm bài viết theo ID
            if (post != null)
            {
                _context.Posts.Remove(post);    // Xóa bài viết khỏi database
                _context.SaveChanges();          // Lưu thay đổi
            }
            return RedirectToAction("Index");   // Quay về danh sách
        }
    }
}