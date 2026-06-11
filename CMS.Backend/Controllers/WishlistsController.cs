/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Web API quản lý sản phẩm yêu thích (Wishlist & WishlistItem)
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public WishlistsController(ApplicationDbContext context) => _context = context;

        [HttpGet("user/{userId}")]
        public IActionResult GetWishlist(int userId)
        {
            var wishlist = _context.Wishlists.Include(w => w.WishlistItems).FirstOrDefault(w => w.CustomerId == userId);
            if (wishlist == null) return NotFound();
            return Ok(wishlist);
        }

        [HttpPost("add")]
        public IActionResult AddToWishlist(WishlistItem item)
        {
            _context.WishlistItems.Add(item);
            _context.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("item/{id}")]
        public IActionResult RemoveItem(int id)
        {
            var item = _context.WishlistItems.Find(id);
            if (item == null) return NotFound();
            _context.WishlistItems.Remove(item);
            _context.SaveChanges();
            return NoContent();
        }
    }
}