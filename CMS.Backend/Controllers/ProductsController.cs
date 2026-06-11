/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : API Controller Sản phẩm Thời Trang & Nước Hoa (ProductsController)
              - GetAll()                : Lấy tất cả, hỗ trợ lọc ?gender & ?size
              - GetByCategoryProduct()  : Lọc theo danh mục, hỗ trợ thêm ?gender & ?size
              - GetByGender()           : Lấy sản phẩm theo giới tính
              - Search()                : Tìm kiếm theo tên + gender + size (Đã sửa lỗi logic chứa ký tự 'm' trong 'ml')
              - GetDetail()             : Chi tiết sản phẩm kèm danh mục và ảnh
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using System.Text.Json;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── API 1: Lấy tất cả sản phẩm, lọc theo gender & size (tuỳ chọn) ──
        // GET api/products?gender=nam&size=100ml
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? gender = null, [FromQuery] string? size = null)
        {
            var query = _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.CategoryProducts!)
                    .ThenInclude(cp => cp.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(gender))
                query = query.Where(p => p.Gender == gender);

            // Bổ sung lọc size cho toàn bộ sản phẩm
            if (!string.IsNullOrEmpty(size))
                query = query.Where(p => p.Sizes != null && p.Sizes.Contains(size));

            var products = await query
                .OrderByDescending(p => p.Id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.OriginalPrice,
                    p.ImageUrl,
                    p.Description,
                    p.Gender,
                    p.Sizes,
                    p.Colors,
                    p.Material,
                    p.Brand,
                    p.IsNew,
                    p.IsHot,
                    p.IsSale,
                    p.Stock,
                    CategoryName = p.CategoryProducts!
                        .Select(cp => cp.Category!.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }

        // ── API 2: Lọc sản phẩm theo danh mục và kích thước ──
        // GET api/products/categoryproduct/1032?gender=nam&size=10ml
        [HttpGet("categoryproduct/{categoryProductId}")]
        public async Task<IActionResult> GetByCategoryProduct(
            int categoryProductId,
            [FromQuery] string? gender = null,
            [FromQuery] string? size = null) // Đã thêm bộ lọc kích thước/dung tích ở đây
        {
            var query = _context.Products
                .Where(p => p.IsActive)
                .Where(p => p.CategoryProducts!.Any(cp => cp.CategoryId == categoryProductId))
                .AsQueryable();

            if (!string.IsNullOrEmpty(gender))
                query = query.Where(p => p.Gender == gender);

            // Thực hiện lọc dung tích/kích cỡ theo danh mục
            if (!string.IsNullOrEmpty(size))
                query = query.Where(p => p.Sizes != null && p.Sizes.Contains(size));

            var products = await query
                .OrderByDescending(p => p.Id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.OriginalPrice,
                    p.ImageUrl,
                    p.Description,
                    p.Gender,
                    p.Sizes,
                    p.Colors,
                    p.Material,
                    p.Brand,
                    p.IsNew,
                    p.IsHot,
                    p.IsSale,
                    p.Stock,
                    CategoryName = p.CategoryProducts!
                        .Select(cp => cp.Category!.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }

        // ── API 3: Lọc sản phẩm theo giới tính ──
        // GET api/products/gender/nam
        [HttpGet("gender/{gender}")]
        public async Task<IActionResult> GetByGender(string gender)
        {
            var products = await _context.Products
                .Where(p => p.IsActive && p.Gender == gender)
                .OrderByDescending(p => p.Id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.OriginalPrice,
                    p.ImageUrl,
                    p.Description,
                    p.Gender,
                    p.Sizes,
                    p.Colors,
                    p.Material,
                    p.Brand,
                    p.IsNew,
                    p.IsHot,
                    p.IsSale,
                    p.Stock,
                    CategoryName = p.CategoryProducts!
                        .Select(cp => cp.Category!.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }

        // ── API 4: Tìm kiếm sản phẩm nâng cao ──
        // GET api/products/search?keyword=Dior&size=10ml
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? keyword = null,
            [FromQuery] string? gender = null,
            [FromQuery] string? size = null)
        {
            var query = _context.Products
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(p => p.Name!.Contains(keyword) || p.Brand!.Contains(keyword));

            if (!string.IsNullOrEmpty(gender))
                query = query.Where(p => p.Gender == gender);

            // Xử lý bộ lọc thông minh: Nếu lọc size 'M', loại trừ trường hợp chứa 'ml' của nước hoa để tránh lẫn lộn
            if (!string.IsNullOrEmpty(size))
            {
                if (size.ToUpper() == "M")
                {
                    // Lấy sản phẩm chứa chữ M nhưng KHÔNG CHỨA "ml"
                    query = query.Where(p => p.Sizes != null && p.Sizes.Contains(size) && !p.Sizes.Contains("ml"));
                }
                else
                {
                    query = query.Where(p => p.Sizes != null && p.Sizes.Contains(size));
                }
            }

            var products = await query
                .OrderByDescending(p => p.Id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.OriginalPrice,
                    p.ImageUrl,
                    p.Gender,
                    p.Sizes,
                    p.IsNew,
                    p.IsHot,
                    p.IsSale,
                    CategoryName = p.CategoryProducts!
                        .Select(cp => cp.Category!.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }

        // ── API 5: Chi tiết sản phẩm ──
        // GET api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var product = await _context.Products
                .Include(p => p.CategoryProducts!)
                    .ThenInclude(cp => cp.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm này." });

            return Ok(new
            {
                product.Id,
                product.Name,
                product.Price,
                product.OriginalPrice,
                product.ImageUrl,
                product.Description,
                product.Gender,
                product.Sizes,
                product.Colors,
                product.Material,
                product.Brand,
                product.IsNew,
                product.IsHot,
                product.IsSale,
                product.Stock,
                Categories = product.CategoryProducts!
                    .Select(cp => new { cp.Category!.Id, cp.Category.Name })
                    .ToList(),
                Images = product.ProductImages?
                    .OrderBy(i => i.SortOrder)
                    .Select(i => i.ImageUrl)
                    .ToList()
            });
        }
    }
}