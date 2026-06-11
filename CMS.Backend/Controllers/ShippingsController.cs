/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Web API quản lý Vận chuyển (Shipping)
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using System;
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ShippingsController(ApplicationDbContext context) => _context = context;

        [HttpGet("order/{orderId}")]
        public IActionResult GetShippingByOrder(int orderId)
        {
            var shipping = _context.Shippings.FirstOrDefault(s => s.OrderId == orderId);
            if (shipping == null) return NotFound();
            return Ok(shipping);
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] int statusId)
        {
            var shipping = _context.Shippings.Find(id);
            if (shipping == null) return NotFound();

            shipping.Status = (ShippingStatus)statusId;
            shipping.UpdatedAt = DateTime.UtcNow;
            if (shipping.Status == ShippingStatus.Delivered) shipping.DeliveredAt = DateTime.UtcNow;

            _context.Shippings.Update(shipping);
            _context.SaveChanges();
            return Ok(shipping);
        }
    }
}