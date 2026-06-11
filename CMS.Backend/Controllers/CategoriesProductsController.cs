/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : API Controller Danh mục Thời Trang (CategoriesProductsController)
              - GetAll()        : Lấy tất cả danh mục đang active
              - GetByGender()   : Lấy danh mục theo giới tính
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── API 1: Lấy tất cả danh mục ──
        // GET api/categoriesproducts
        // GET api/categoriesproducts?gender=nam
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? gender = null)
        {
            try
            {
                var query = _context.Categories
                    .Where(c => c.IsActive)
                    .AsQueryable();

                // Lọc theo giới tính nếu có
                if (!string.IsNullOrEmpty(gender))
                    query = query.Where(c => c.Gender == gender || c.Gender == "all");

                var categories = await query
                    .OrderBy(c => c.SortOrder)
                    .ThenBy(c => c.Name)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Description,
                        c.Gender,
                        c.ImageUrl,
                        c.SortOrder,
                        // Đếm số sản phẩm trong danh mục
                        ProductCount = c.CategoryProducts!
                            .Count(cp => cp.Product!.IsActive)
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Lỗi kết nối cơ sở dữ liệu",
                    detail = ex.Message
                });
            }
        }

        // ── API 2: Lấy danh mục theo giới tính ──
        // GET api/categoriesproducts/gender/nam
        [HttpGet("gender/{gender}")]
        public async Task<IActionResult> GetByGender(string gender)
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive && (c.Gender == gender || c.Gender == "all"))
                    .OrderBy(c => c.SortOrder)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Description,
                        c.Gender,
                        c.ImageUrl
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }
    }
}
