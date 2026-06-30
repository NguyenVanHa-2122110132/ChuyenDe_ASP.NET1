/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 25/06/2026
    Mô tả    : API Controller trả khoảng giá cho React FashionPage
*/
using CMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    [Route("api/pricesettings")]
    [ApiController]
    public class PriceSettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PriceSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/pricesettings
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var setting = await _context.PriceSettings.FirstOrDefaultAsync();

            if (setting == null)
                return Ok(new { minPrice = 0, maxPrice = 10000000 });

            return Ok(new
            {
                minPrice = setting.MinPrice,
                maxPrice = setting.MaxPrice,
                label = setting.Label
            });
        }
    }
}