/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 09/06/2026
    Mô tả    : API Controller Đơn hàng (OrdersController) - Phiên bản hoàn chỉnh tích hợp Client
              - CreateOrder() : Tiếp nhận đơn hàng từ Checkout.jsx (POST)
              - GetOrders()   : Lấy danh sách đơn hàng thực tế cho OrdersPage.jsx (GET)
              - CancelOrder() : Xử lý hủy đơn hàng trực tiếp từ Client (PUT)
*/
using CMS.Backend.Services;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace CMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public OrdersController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ── 1. HTTP GET: api/orders ──
        // Lấy danh sách đơn hàng thực tế truyền về cho file OrdersPage.jsx của React
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                // Lấy toàn bộ danh sách đơn hàng, xếp đơn mới đặt lên trên cùng
                var orders = await _context.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                var result = new List<object>();

                foreach (var order in orders)
                {
                    // Chuyển đổi mã trạng thái số nguyên sang chuỗi chữ Tiếng Việt đồng bộ giao diện
                    string trangThaiText = "Chờ xử lý";
                    if (order.Status == 1) trangThaiText = "Đã xác nhận";
                    else if (order.Status == 2) trangThaiText = "Đang giao hàng";
                    else if (order.Status == 3) trangThaiText = "Đã hủy";

                    // Tính toán tổng tiền hóa đơn thực tế dựa trên bảng OrderDetails
                    var details = await _context.OrderDetails
                        .Where(d => d.OrderId == order.Id)
                        .ToListAsync();

                    decimal total = details.Sum(d => d.Quantity * d.UnitPrice);

                    result.Add(new
                    {
                        id = order.Id,
                        ngayDat = order.OrderDate.ToString("dd/MM/yyyy"),
                        trangThai = trangThaiText,
                        tongTien = total.ToString("N0") + "đ", // Định dạng tiền tệ có dấu chấm phân cách (Ví dụ: 1.250.000đ)
                        ghiChu = order.Notes
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi lấy danh sách đơn hàng.", detail = ex.Message });
            }
        }

        // ── 2. HTTP POST: api/orders ──
        // Tiếp nhận dữ liệu đặt hàng gửi từ form Checkout.jsx
         [Authorize(Roles = "Customer")] // ➕ Bắt buộc đăng nhập với JWT token hợp lệ
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderInputDTO input)
        {
            if (input == null || input.Items == null || input.Items.Count == 0)
                return BadRequest(new { message = "Dữ liệu đơn hàng không hợp lệ." });

            // ➕ Lấy CustomerId trực tiếp từ JWT token (không tin email do client gửi lên)
            var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                return Unauthorized(new { message = "Token không hợp lệ. Vui lòng đăng nhập lại." });

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return Unauthorized(new { message = "Không tìm thấy tài khoản. Vui lòng đăng nhập lại." });

            // Cập nhật lại thông tin liên hệ mới nhất (nếu khách đổi SĐT/địa chỉ lúc checkout)
            customer.FullName = input.FullName;
            customer.Phone = input.Phone;
            customer.Address = input.Address;
            await _context.SaveChangesAsync();

            // Kiểm tra lượng tồn kho của từng sản phẩm trước khi tạo hóa đơn
            foreach (var item in input.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest(new { message = $"Sản phẩm ID {item.ProductId} không tồn tại." });

                if (product.Stock < item.Quantity)
                    return BadRequest(new { message = $"Sản phẩm '{product.Name}' chỉ còn {product.Stock} trong kho!" });
            }

            try
            {
                // Tạo đối tượng đơn hàng mới vào DB
                var newOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerId = customer.Id,
                    Status = 0, // 0 = Chờ xử lý mặc định
                    Email = customer.Email ?? string.Empty, // ➕ Dùng email thật của customer, không tin client
                    PaymentMethod = input.PaymentMethod ?? "cod",
                    Notes = $"Giao đến: {input.Address} | SĐT: {input.Phone}" +
                            (string.IsNullOrEmpty(input.Notes) ? "" : $" | {input.Notes}"),
                };

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync(); // Lưu để sinh OrderId tự động tăng

                // Lưu dữ liệu vào bảng chi tiết hóa đơn & khấu trừ trực tiếp số lượng kho hàng
                foreach (var item in input.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);

                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = newOrder.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                    });

                    product!.Stock -= item.Quantity; // Trừ kho của sản phẩm
                }

                await _context.SaveChangesAsync();

                // Gửi email thông báo tự động xác nhận đặt hàng thành công
                if (!string.IsNullOrEmpty(customer.Email)) // ➕ Sửa điều kiện bỏ check @guest.com
                {
                    try
                    {
                        await _emailService.SendOrderConfirmEmailAsync(
                            customer.Email,
                            customer.FullName ?? input.FullName,
                            newOrder.Id.ToString(),
                            input.TotalAmount
                        );
                    }
                    catch
                    {
                        // Bỏ qua lỗi gửi email để không làm gián đoạn tiến trình tạo đơn
                    }
                }

                return StatusCode(201, new
                {
                    message = "Đặt hàng thành công!",
                    orderId = newOrder.Id,
                    paymentUrl = (string?)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi xử lý lưu đơn hàng.", detail = ex.Message });
            }
        }

        // 3. HTTP PUT: api/orders/confirm/{id}
        // Tiếp nhận yêu cầu XÁC NHẬN đơn hàng từ trang quản lý Admin
        [HttpPut("Confirm/{id}")]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound(new { message = $"Không tìm thấy mã đơn hàng #{id}" });
                }

                // Điều kiện bắt buộc: Chỉ cho phép xác nhận khi đơn hàng đang ở trạng thái Chờ xử lý (Status = 0)
                if (order.Status != 0)
                {
                    return BadRequest(new { message = "Đơn hàng này đã được xử lý hoặc đã hủy, không thể xác nhận lại." });
                }

                // Cập nhật mã trạng thái thành Đã xác nhận (Quy định hệ thống là số 1)
                order.Status = 1;

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Đã xác nhận thành công đơn hàng #{id}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi xác nhận đơn hàng.", detail = ex.Message });
            }
        }

        // ── 4. HTTP PUT: api/orders/cancel/{id} ──
        // Tiếp nhận yêu cầu hủy đơn từ nút bấm hủy hàng của trang OrdersPage.jsx
        [HttpPut("Cancel/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                    return NotFound(new { message = $"Không tìm thấy mã đơn hàng #{id}" });

                // Điều kiện bắt buộc: Chỉ được phép hủy khi trạng thái đơn hàng là Chờ xử lý (Status = 0)
                if (order.Status != 0)
                {
                    return BadRequest(new { message = "Đơn hàng này đã được xác nhận hoặc xử lý, không thể hủy." });
                }

                // Cập nhật mã trạng thái thành Đã hủy (Quy định hệ thống là số 3)
                order.Status = 3;

                // ── HOÀN LẠI TỒN KHO (STOCK) CHO SẢN PHẨM ──
                var details = await _context.OrderDetails.Where(d => d.OrderId == id).ToListAsync();
                foreach (var item in details)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity; // Hoàn lại đúng số lượng mặt hàng vào hệ thống kho
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Đã thực hiện hủy thành công đơn hàng #{id}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ khi thực hiện tác vụ hủy đơn.", detail = ex.Message });
            }
        }
    }

    // ── DATA TRANSFER OBJECTS (DTOs) ──
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class OrderInputDTO
    {
        public string FullName { get; set; } = "";
        public string? Email { get; set; }
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string? Notes { get; set; }
        public string? PaymentMethod { get; set; } = "cod";
        public List<OrderItemDTO> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}