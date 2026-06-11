/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Web API quản lý Đánh giá (Review) và Hình ảnh đánh giá
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ReviewsController(ApplicationDbContext context) => _context = context;

        [HttpGet("product/{productId}")]
        public ActionResult<IEnumerable<Review>> GetReviewsByProduct(int productId)
            => _context.Reviews.Include(r => r.ReviewImages).Where(r => r.ProductId == productId).ToList();

        [HttpPost]
        public ActionResult<Review> PostReview(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return Ok(review);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review == null) return NotFound();
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return NoContent();
        }
    }
}