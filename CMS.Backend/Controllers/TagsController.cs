/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Web API quản lý Thẻ bài viết và liên kết bài viết (Tag & PostTag)
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TagsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetTags() => Ok(_context.Tags.ToList());

        [HttpPost]
        public IActionResult CreateTag(Tag tag)
        {
            _context.Tags.Add(tag);
            _context.SaveChanges();
            return Ok(tag);
        }

        [HttpPost("attach-to-post")]
        public IActionResult AttachTagToPost(PostTag postTag)
        {
            if (_context.PostTags.Any(pt => pt.PostId == postTag.PostId && pt.TagId == postTag.TagId))
                return BadRequest("Thẻ này đã được gắn cho bài viết.");

            _context.PostTags.Add(postTag);
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("detach-from-post")]
        public IActionResult DetachTagFromPost(int postId, int tagId)
        {
            var pt = _context.PostTags.FirstOrDefault(x => x.PostId == postId && x.TagId == tagId);
            if (pt == null) return NotFound();
            _context.PostTags.Remove(pt);
            _context.SaveChanges();
            return NoContent();
        }
    }
}