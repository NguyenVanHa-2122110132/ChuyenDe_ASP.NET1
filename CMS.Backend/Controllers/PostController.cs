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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Backend.Controllers
{
    // ✅ Thêm AuthenticationSchemes = Cookie cho toàn bộ controller MVC admin
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
               Roles = "Administrator,Admin,Sales,Cashier")]
    public class PostController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== INDEX ==========
        // Administrator, Admin, Sales, Cashier đều được xem danh sách bài viết
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

        // ========== CREATE GET ==========
        // ✅ Chỉ ghi đè Roles cho action cần quyền cao hơn
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
                   Roles = "Administrator,Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(
                _context.Categories.ToList(), "Id", "Name"
            );
            return View();
        }

        // ========== EDIT GET ==========
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
                   Roles = "Administrator,Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound();

            ViewBag.Categories = new SelectList(
                _context.Categories.ToList(), "Id", "Name", post.CategoryId
            );
            return View(post);
        }

        // ========== CREATE POST ==========
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
                   Roles = "Administrator,Admin")]
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
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
                   Roles = "Administrator,Admin")]
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
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
                   Roles = "Administrator,Admin")]
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