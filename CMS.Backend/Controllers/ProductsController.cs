/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : API Controller quản lý Sản phẩm (ProductsController)
              - GetAll()                  : Lấy toàn bộ danh sách sản phẩm, sắp xếp mới nhất lên đầu
              - GetByCategoryProduct()    : Lọc sản phẩm theo danh mục thông qua bảng trung gian CategoryProduct
              - GetDetail()               : Lấy chi tiết một sản phẩm theo Id, trả về 404 nếu không tìm thấy
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data;

namespace CMS.Backend.Controllers
{
    // Định nghĩa đường dẫn API: 
    [Route("api/[controller]")]

    // Kích hoạt tính năng tự động kiểm tra dữ liệu đầu vào (Validation)
    [ApiController]

    // Kế thừa ControllerBase 
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Hàm khởi tạo: Tiêm ngữ cảnh dữ liệu SQL Server vào Controller thông qua DI
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // API 1: Lấy toàn bộ sản phẩm 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Lấy toàn bộ sản phẩm từ bảng Products, sắp xếp theo Id giảm dần (mới nhất lên đầu)
            var products = await _context.Products
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            // Trả về danh sách kèm mã HTTP 200 OK
            return Ok(products);
        }

        // API 2: Lấy sản phẩm theo danh mục
        [HttpGet("categoryproduct/{categoryProductId}")]
        public async Task<IActionResult> GetByCategoryProduct(int categoryProductId)
        {
            // Lọc sản phẩm có liên kết với CategoryId trong bảng trung gian CategoryProduct
            var products = await _context.Products
                .Where(p => p.CategoryProducts!.Any(cp => cp.CategoryId == categoryProductId))
                .ToListAsync();

            // Trả về danh sách sản phẩm thuộc danh mục kèm mã HTTP 200 OK
            return Ok(products);
        }

        // API 3: Lấy chi tiết một sản phẩm 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            // Tìm sản phẩm đầu tiên có Id khớp với tham số truyền vào
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            // Nếu không tìm thấy: trả về mã lỗi 404 kèm thông báo JSON
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm này trong hệ thống" });
            }

            // Trả về đối tượng sản phẩm đầy đủ kèm mã HTTP 200 OK
            return Ok(product);
        }
    }
}