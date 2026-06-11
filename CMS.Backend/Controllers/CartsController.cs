/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Web API quản lý Giỏ hàng (Cart) và Chi tiết mặt hàng (CartItem)
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CartsController(ApplicationDbContext context) => _context = context;

        [HttpGet("{customerId}")]
        public IActionResult GetCart(int customerId)
        {
            var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.CustomerId == customerId);
            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }
            return Ok(cart);
        }

        [HttpPost("add-item")]
        public IActionResult AddItem(CartItem item)
        {
            var existing = _context.CartItems.FirstOrDefault(ci => ci.CartId == item.CartId && ci.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                _context.CartItems.Update(existing);
            }
            else
            {
                _context.CartItems.Add(item);
            }
            _context.SaveChanges();
            return Ok();
        }
    }
}