/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Web API quản lý Mã giảm giá (Coupon) và lịch sử sử dụng (CouponUsage)
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
    public class CouponsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CouponsController(ApplicationDbContext context) => _context = context;

        [HttpGet("validate/{code}")]
        public IActionResult ValidateCoupon(string code)
        {
            var coupon = _context.Coupons.FirstOrDefault(c => c.Code == code && c.IsActive);
            if (coupon == null) return NotFound("Mã giảm giá không tồn tại.");
            if (coupon.ExpiryDate < DateTime.UtcNow) return BadRequest("Mã giảm giá đã hết hạn.");
            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value) return BadRequest("Mã giảm giá đã hết lượt sử dụng.");

            return Ok(coupon);
        }

        [HttpPost]
        public ActionResult<Coupon> CreateCoupon(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            _context.SaveChanges();
            return Ok(coupon);
        }
    }
}