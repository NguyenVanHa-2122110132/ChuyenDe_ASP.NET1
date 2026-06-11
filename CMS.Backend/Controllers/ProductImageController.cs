/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Controller quản lý Album ảnh sản phẩm (ProductImage)
              - Index        : Hiển thị toàn bộ danh sách hình ảnh theo từng sản phẩm (Admin quản lý)
              - Create       : Thêm mới hình ảnh phụ (Lưu URL ảnh hoặc xử lý upload) vào database
              - SetThumbnail : Thiết lập một hình ảnh làm ảnh đại diện chính của sản phẩm (tự động hạ cấp ảnh cũ)
              - Delete       : Xóa bỏ hình ảnh khỏi hệ thống database
              - Phân quyền   : Chỉ Administrator và Admin mới được thay đổi album ảnh sản phẩm
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Thêm namespace để dùng [Authorize]
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được vào ban quản lý kho ảnh
    public class ProductImageController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public ProductImageController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX (Quản lý ảnh theo sản phẩm) ==========
        public IActionResult Index(int? productId)
        {
            // Lấy danh sách ảnh, nếu chọn một sản phẩm cụ thể thì lọc riêng, không thì lấy toàn bộ ảnh hệ thống
            var query = _context.ProductImages.Include(pi => pi.Product).AsQueryable();

            if (productId.HasValue)
            {
                query = query.Where(pi => pi.ProductId == productId.Value);
                ViewBag.ProductId = productId.Value; // Giữ lại ID sản phẩm ra View để phục vụ chức năng thêm ảnh nhanh
            }

            var data = query.OrderBy(pi => pi.SortOrder).ToList(); // Sắp xếp hình ảnh hiển thị theo thứ tự ưu tiên số nhỏ trước
            return View(data); // Truyền danh sách bộ sưu tập ảnh ra View
        }

        // ========== CREATE POST (Thêm ảnh vào album) ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductImage model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu nhập hợp lệ
            {
                // Nếu ảnh mới thêm được tick chọn làm ảnh đại diện chính (IsThumbnail = true)
                if (model.IsThumbnail)
                {
                    // Tìm và tắt trạng thái ảnh đại diện cũ của sản phẩm này (đưa IsThumbnail về false)
                    var oldThumbnail = _context.ProductImages
                        .FirstOrDefault(pi => pi.ProductId == model.ProductId && pi.IsThumbnail);

                    if (oldThumbnail != null)
                    {
                        oldThumbnail.IsThumbnail = false;
                        _context.ProductImages.Update(oldThumbnail);
                    }
                }

                model.CreatedAt = DateTime.UtcNow; // Thiết lập mốc thời gian tạo ảnh hiện tại
                _context.ProductImages.Add(model);  // Thêm thực thể ảnh mới vào database
                _context.SaveChanges();            // Lưu thay đổi

                TempData["SuccessMessage"] = "Thêm hình ảnh vào album sản phẩm thành công!";
                return RedirectToAction("Index", new { productId = model.ProductId }); // Quay về album của sản phẩm đó
            }
            return RedirectToAction("Index", new { productId = model.ProductId }); // Nếu lỗi cấu trúc điều hướng quay về trang cũ
        }

        // ========== SET THUMBNAIL (Đặt làm ảnh đại diện chính) ==========
        [HttpPost]
        public IActionResult SetThumbnail(int id)
        {
            var targetImage = _context.ProductImages.Find(id); // Tìm hình ảnh được chọn theo ID
            if (targetImage == null) return NotFound();        // Không tồn tại trả về lỗi 404

            // Bước 1: Tìm và gỡ bỏ ảnh đại diện chính hiện tại của sản phẩm này ra
            var currentThumbnail = _context.ProductImages
                .FirstOrDefault(pi => pi.ProductId == targetImage.ProductId && pi.IsThumbnail);

            if (currentThumbnail != null)
            {
                currentThumbnail.IsThumbnail = false; // Hạ cấp ảnh cũ xuống thành ảnh phụ
                _context.ProductImages.Update(currentThumbnail);
            }

            // Bước 2: Thiết lập hình ảnh được chọn này làm ảnh đại diện chính mới
            targetImage.IsThumbnail = true;
            _context.ProductImages.Update(targetImage);

            _context.SaveChanges(); // Áp dụng đồng thời cả 2 thay đổi vào database

            TempData["SuccessMessage"] = "Thay đổi hình ảnh đại diện chính thành công!";
            return RedirectToAction("Index", new { productId = targetImage.ProductId }); // Tải lại trang album của sản phẩm
        }

        // ========== DELETE ==========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var productImage = _context.ProductImages.FirstOrDefault(pi => pi.Id == id); // Tìm hình ảnh theo ID
            if (productImage == null) return NotFound(); // Nếu không thấy trả về trang lỗi 404

            int currentProductId = productImage.ProductId; // Giữ lại ID sản phẩm để điều hướng quay về sau khi xóa

            // Lưu ý logic dự án: Nếu xóa trúng ảnh đang làm Thumbnail, nên cảnh báo Admin đặt ảnh khác làm Thumbnail trước khi xóa
            if (productImage.IsThumbnail)
            {
                TempData["ErrorMessage"] = "Không thể xóa hình ảnh này! Đây đang là ảnh đại diện chính của sản phẩm. Vui lòng thiết lập ảnh khác làm đại diện trước.";
                return RedirectToAction("Index", new { productId = currentProductId });
            }

            _context.ProductImages.Remove(productImage); // Tiến hành xóa bản ghi hình ảnh khỏi database
            _context.SaveChanges();                     // Lưu thay đổi vào hệ thống database

            TempData["SuccessMessage"] = "Đã xóa hình ảnh ra khỏi bộ sưu tập của sản phẩm!";
            return RedirectToAction("Index", new { productId = currentProductId }); // Quay lại trang album ảnh
        }
    }
}