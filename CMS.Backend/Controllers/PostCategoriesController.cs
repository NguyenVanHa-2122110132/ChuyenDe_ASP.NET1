/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Web API quản lý danh mục bài viết (PostCategory & PostPostCategory)
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PostCategoriesController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetCategories() => Ok(_context.PostCategories.OrderBy(pc => pc.SortOrder).ToList());

        [HttpPost]
        public IActionResult CreateCategory(PostCategory category)
        {
            _context.PostCategories.Add(category);
            _context.SaveChanges();
            return Ok(category);
        }

        [HttpPost("assign-to-post")]
        public IActionResult AssignToPost(PostPostCategory rel)
        {
            if (_context.PostPostCategories.Any(x => x.PostId == rel.PostId && x.PostCategoryId == rel.PostCategoryId))
                return BadRequest("Bài viết đã thuộc danh mục này.");
            _context.PostPostCategories.Add(rel);
            _context.SaveChanges();
            return Ok();
        }
    }
}