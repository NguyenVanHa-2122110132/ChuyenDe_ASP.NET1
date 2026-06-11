/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Web API quản lý Kho hàng (Inventory) và Biến động kho bãi (InventoryTransaction)
              - GetStock          : Lấy thông tin tồn kho của sản phẩm theo ProductId
              - CreateTransaction : Xử lý tạo giao dịch kho (Nhập/Xuất/Điều chỉnh), tự động tính toán dữ liệu QuantityInStock, QuantityBefore, QuantityAfter và lưu lịch sử
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")] // Định nghĩa cấu hình đường dẫn URL định tuyến cho API trên Swagger (api/Inventories)
    [ApiController]             // Đánh dấu đây là một Web API Controller trả về dữ liệu dạng chuỗi JSON
    public class InventoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // Khai báo biến cơ sở dữ liệu hệ thống

        // Hàm khởi tạo nhận DbContext từ cơ chế Dependency Injection của dự án
        public InventoriesController(ApplicationDbContext context)
        {
            _context = context; // Gán dữ liệu kết nối vào biến cục bộ để sử dụng
        }

        // ========== API LẤY THÔNG TIN KHO THEO PRODUCT ID ==========
        [HttpGet("product/{productId}")] // Cấu hình phương thức GET: api/Inventories/product/{productId}
        public IActionResult GetStock(int productId)
        {
            // Tiến hành tìm bản ghi kho hàng đầu tiên trùng khớp với mã ID sản phẩm được truyền lên
            var inv = _context.Inventories.FirstOrDefault(i => i.ProductId == productId);

            // Nếu không tồn tại bản ghi kho cho sản phẩm này, trả về mã lỗi 404 kèm thông báo cụ thể
            if (inv == null) return NotFound("Sản phẩm này chưa được khởi tạo thông tin trong bảng kho hàng.");

            // Nếu tìm thấy dữ liệu hợp lệ, trả về mã thành công 200 kèm theo toàn bộ thực thể kho hàng
            return Ok(inv);
        }

        // ========== API TẠO GIAO DỊCH KHO VÀ CẬP NHẬT SỐ LƯỢNG TỒN KHO ==========
        [HttpPost("transaction")] // Cấu hình phương thức POST: api/Inventories/transaction
        public IActionResult CreateTransaction(int productId, int quantity, InventoryTransactionType type, string? reason, int? referenceId, int userId)
        {
            // 1. Tìm bản ghi kho của sản phẩm trong database dựa theo ProductId
            var stock = _context.Inventories.FirstOrDefault(i => i.ProductId == productId);
            if (stock == null) return NotFound("Sản phẩm này chưa được khởi tạo kho bãi.");

            // Ép buộc số lượng truyền vào phải là số dương lớn hơn 0
            if (quantity <= 0) return BadRequest("Số lượng sản phẩm thay đổi kho bãi phải lớn hơn 0.");

            // 2. Lưu lại trạng thái số lượng tồn kho trước khi thực hiện biến động (QuantityBefore)
            int beforeQuantity = stock.QuantityInStock;
            int afterQuantity = beforeQuantity;

            // 3. Phân tách logic xử lý số lượng dựa trên kiểu enum InventoryTransactionType bạn cung cấp
            if (type == InventoryTransactionType.Import || type == InventoryTransactionType.Return)
            {
                // Trường hợp Nhập kho hoặc Nhập hàng hoàn trả -> Tăng số lượng tồn kho lên
                stock.QuantityInStock += quantity;
                afterQuantity = stock.QuantityInStock;
            }
            else if (type == InventoryTransactionType.Export || type == InventoryTransactionType.Transfer)
            {
                // Trường hợp Xuất kho bán hàng hoặc Chuyển kho đi -> Kiểm tra xem kho còn đủ hàng không
                if (stock.QuantityInStock < quantity)
                {
                    return BadRequest("Số lượng hàng tồn kho hiện tại không đủ để thực hiện giao dịch xuất kho này.");
                }
                stock.QuantityInStock -= quantity;
                afterQuantity = stock.QuantityInStock;
            }
            else if (type == InventoryTransactionType.Adjustment)
            {
                // Trường hợp Điều chỉnh tồn kho -> Admin có thể truyền số lượng âm hoặc dương (Hàm này mặc định xử lý cộng thêm, nếu trừ đi thì dùng lý thuyết trên)
                stock.QuantityInStock += quantity; // Điều chỉnh tăng (Nếu muốn giảm, Admin truyền giá trị âm thông qua API mở rộng hoặc xử lý riêng)
                afterQuantity = stock.QuantityInStock;
            }

            // Cập nhật lại mốc thời gian sửa đổi của kho hàng
            stock.UpdatedAt = DateTime.UtcNow;

            // 4. Tạo bản ghi lịch sử biến động kho (InventoryTransaction) trùng khớp 100% thuộc tính file bạn gửi
            var transaction = new InventoryTransaction
            {
                InventoryId = stock.Id,            // Khóa ngoại liên kết tới bảng Inventory vừa tìm được
                Type = type,                       // Loại giao dịch (Import, Export, Adjustment...)
                Quantity = quantity,               // Số lượng biến động mặt hàng
                QuantityBefore = beforeQuantity,   // Số lượng tồn kho cũ trước khi thay đổi
                QuantityAfter = afterQuantity,     // Số lượng tồn kho mới sau khi thay đổi
                Reason = reason,                   // Lý do nhập xuất kho
                ReferenceId = referenceId,         // Mã hóa đơn hoặc mã tham chiếu liên quan
                CreatedByUserId = userId,          // ID người dùng/nhân viên thực hiện lệnh kho
                CreatedAt = DateTime.UtcNow        // Thời điểm tạo lịch sử giao dịch
            };

            // 5. Lưu đồng thời cả thay đổi số lượng kho và lịch sử giao dịch vào database
            _context.Inventories.Update(stock);
            _context.InventoryTransactions.Add(transaction);
            _context.SaveChanges(); // Hoàn tất thực thi lệnh SQL vào cơ sở dữ liệu

            // Trả về kết quả thành công 200 kèm dữ liệu mới sau khi xử lý xong
            return Ok(new
            {
                Message = "Tạo giao dịch kho bãi và cập nhật số lượng tồn kho thành công!",
                ProductId = productId,
                OldStock = beforeQuantity,
                NewStock = afterQuantity
            });
        }
    }
}