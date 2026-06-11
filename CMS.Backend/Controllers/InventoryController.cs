/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 01/06/2026
    Mô tả    : Controller quản lý Tồn kho và Giao dịch kho (Inventory)
              - Index   : Hiển thị danh sách tồn kho của các sản phẩm và cảnh báo sắp hết hàng
              - Edit    : Thay đổi vị trí kho và ngưỡng cảnh báo hết hàng
              - UpdateStock: Xử lý Nhập/Xuất/Điều chỉnh kho và tự động ghi lịch sử (InventoryTransaction)
              - History : Xem lịch sử các lần biến động kho bãi
              - Phân quyền: Chỉ Administrator và Admin mới được phép thao tác kho bãi
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
    [Authorize(Roles = "Administrator,Admin")] // Chỉ Administrator và Admin mới được quyền quản lý kho
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context; // Biến kết nối database

        public InventoryController(ApplicationDbContext context)
        {
            _context = context; // Nhận database context qua Dependency Injection
        }

        // ========== INDEX (Danh sách tồn kho) ==========
        public IActionResult Index()
        {
            var data = _context.Inventories
                .Include(i => i.Product) // Lấy thông tin sản phẩm đi kèm để hiển thị tên/hình ảnh
                .ToList();

            return View(data); // Truyền danh sách tồn kho ra View quản trị
        }

        // ========== EDIT GET (Sửa thông tin cơ bản của kho) ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var inventory = _context.Inventories
                .Include(i => i.Product)
                .FirstOrDefault(i => i.Id == id); // Tìm kho theo ID

            if (inventory == null) return NotFound(); // Không tìm thấy trả về 404
            return View(inventory); // Hiển thị form thay đổi thông tin vị trí, ngưỡng cảnh báo
        }

        // ========== EDIT POST (Cập nhật thông tin cấu hình kho) ==========
        [HttpPost]
        public IActionResult Edit(Inventory model)
        {
            // Tìm đối tượng thực tế trong database để tránh mất các dữ liệu số lượng hiện tại
            var inventory = _context.Inventories.FirstOrDefault(i => i.Id == model.Id);
            if (inventory == null) return NotFound();

            if (ModelState.IsValid) // Kiểm tra dữ liệu hợp lệ
            {
                inventory.WarehouseLocation = model.WarehouseLocation; // Cập nhật vị trí kho mới
                inventory.LowStockThreshold = model.LowStockThreshold; // Cập nhật ngưỡng cảnh báo mới
                inventory.UpdatedAt = DateTime.UtcNow;                  // Cập nhật thời gian sửa đổi

                _context.Inventories.Update(inventory); // Cập nhật vào database
                _context.SaveChanges();                // Lưu thay đổi
                return RedirectToAction("Index");      // Quay về trang danh sách
            }
            return View(model); // Nếu lỗi hiển thị lại form
        }

        // ========== UPDATE STOCK (Thao tác Nhập / Điều chỉnh kho) ==========
        [HttpPost]
        public IActionResult UpdateStock(int inventoryId, int transactionType, int quantity, string? reason, int userId)
        {
            // Tìm bản ghi kho cần thao tác
            var inventory = _context.Inventories.FirstOrDefault(i => i.Id == inventoryId);
            if (inventory == null) return NotFound();

            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Số lượng thay đổi phải lớn hơn 0!";
                return RedirectToAction("Index");
            }

            // Lưu lại số lượng cũ trước khi biến động
            int luongCu = inventory.QuantityInStock;

            // Xử lý tính toán số lượng dựa theo loại giao dịch được chọn
            // (0: Import - Nhập, 2: Adjustment - Điều chỉnh, 3: Return - Hàng trả)
            if (transactionType == (int)InventoryTransactionType.Import || transactionType == (int)InventoryTransactionType.Return)
            {
                inventory.QuantityInStock += quantity; // Nhập kho hoặc hàng trả về thì cộng thêm vào tổng tồn
            }
            else if (transactionType == (int)InventoryTransactionType.Adjustment)
            {
                inventory.QuantityInStock = quantity; // Điều chỉnh kho thì gán trực tiếp số lượng tồn kho mới
            }
            else
            {
                TempData["ErrorMessage"] = "Loại giao dịch không hợp lệ cho chức năng này!";
                return RedirectToAction("Index");
            }

            inventory.UpdatedAt = DateTime.UtcNow; // Cập nhật mốc thời gian sửa đổi kho
            _context.Inventories.Update(inventory); // Cập nhật trạng thái kho

            // Tự động tạo và lưu lịch sử giao dịch (InventoryTransaction)
            var transaction = new InventoryTransaction
            {
                InventoryId = inventory.Id,
                Type = (InventoryTransactionType)transactionType,
                Quantity = (transactionType == (int)InventoryTransactionType.Adjustment) ? (inventory.QuantityInStock - luongCu) : quantity,
                QuantityBefore = luongCu,
                QuantityAfter = inventory.QuantityInStock,
                Reason = reason ?? "Admin thực hiện cập nhật thủ công tại trang quản trị",
                CreatedByUserId = userId, // ID của Admin thực hiện
                CreatedAt = DateTime.UtcNow
            };

            _context.InventoryTransactions.Add(transaction); // Thêm lịch sử giao dịch vào database
            _context.SaveChanges();                         // Lưu đồng thời cả thay đổi kho và lịch sử

            TempData["SuccessMessage"] = "Cập nhật số lượng kho bãi thành công!";
            return RedirectToAction("Index");
        }

        // ========== HISTORY (Xem lịch sử giao dịch biến động kho) ==========
        public IActionResult History(int? id)
        {
            // Lấy danh sách giao dịch, nếu truyền ID thì lọc riêng theo sản phẩm, không thì lấy toàn bộ
            var query = _context.InventoryTransactions
                .Include(t => t.Inventory).ThenInclude(i => i.Product)
                .Include(t => t.CreatedByUser) // Lấy thông tin Admin nào đã làm
                .AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(t => t.InventoryId == id.Value);
            }

            var data = query.OrderByDescending(t => t.CreatedAt).ToList(); // Sắp xếp giao dịch mới nhất lên đầu
            return View(data); // Truyền danh sách lịch sử ra View
        }
    }
}