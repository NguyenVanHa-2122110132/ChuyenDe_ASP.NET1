/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 26/05/2026
    Mô tả    : API Controller quản lý Bài viết (Posts)
              - GetAll        : Lấy toàn bộ bài viết, sắp xếp mới nhất lên đầu
              - GetByCategory : Lấy bài viết theo danh mục
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CMS.Data;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // Cho phép truy cập không cần đăng nhập
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== GET ALL ==========
        // Địa chỉ: GET https://localhost:xxxx/api/posts
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var posts = _context.Posts
                    .OrderByDescending(p => p.Id)
                    .Select(p => new {
                        p.Id,
                        p.Title,
                        p.ImageUrl,
                        p.CreatedDate
                    })
                    .ToList();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // ========== GET BY CATEGORY ==========
        // Địa chỉ: GET https://localhost:xxxx/api/posts/category/1
        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            var posts = _context.Posts
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new {
                    p.Id,
                    p.Title,
                    p.ImageUrl
                })
                .ToList();
            return Ok(posts);
        }

        // ========== GET DETAIL ==========
        // Địa chỉ: GET https://localhost:xxxx/api/posts/1
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var post = _context.Posts
                .FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return NotFound(new { message = "Không tìm thấy bài viết này trong hệ thống" });
            }

            return Ok(post);
        }



    }


}