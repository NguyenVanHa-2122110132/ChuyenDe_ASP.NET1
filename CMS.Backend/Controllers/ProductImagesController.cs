/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Web API quản lý hình ảnh sản phẩm (ProductImage)
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductImagesController(ApplicationDbContext context) => _context = context;

        [HttpGet("product/{productId}")]
        public IActionResult GetImagesByProduct(int productId)
            => Ok(_context.ProductImages.Where(pi => pi.ProductId == productId).OrderBy(pi => pi.SortOrder).ToList());

        [HttpPost]
        public IActionResult AddImage(ProductImage image)
        {
            _context.ProductImages.Add(image);
            _context.SaveChanges();
            return Ok(image);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteImage(int id)
        {
            var img = _context.ProductImages.Find(id);
            if (img == null) return NotFound();
            _context.ProductImages.Remove(img);
            _context.SaveChanges();
            return NoContent();
        }
    }
}