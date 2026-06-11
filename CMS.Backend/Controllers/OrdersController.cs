/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 09/06/2026
    Mô tả    : API Controller Đơn hàng (OrdersController) - Phiên bản hoàn chỉnh
              - CreateOrder() : Tiếp nhận đơn hàng từ Checkout.jsx
                                Lưu Order + OrderDetails
                                Trừ tồn kho (Stock) từng sản phẩm
                                Gửi email xác nhận cho khách hàng
                                Trả về 201 Created kèm orderId
*/
using CMS.Data;
using CMS.Data.Entities;
using CMS.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // POST api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderInputDTO input)
        {
            if (input == null || input.Items == null || input.Items.Count == 0)
                return BadRequest(new { message = "Dữ liệu đơn hàng không hợp lệ." });

            // ── Bước 1: Tìm hoặc tạo Customer ──
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == input.Email);

            if (customer == null)
            {
                // Khách chưa có tài khoản → tạo bản ghi tạm
                customer = new Customer
                {
                    FullName = input.FullName,
                    Email = input.Email ?? $"guest_{DateTime.Now.Ticks}@guest.com",
                    Phone = input.Phone,
                    Address = input.Address,
                    Password = "guest", // Không dùng để đăng nhập
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            // ── Bước 2: Kiểm tra tồn kho trước khi tạo đơn ──
            foreach (var item in input.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest(new { message = $"Sản phẩm ID {item.ProductId} không tồn tại." });

                if (product.Stock < item.Quantity)
                    return BadRequest(new
                    {
                        message = $"Sản phẩm '{product.Name}' chỉ còn {product.Stock} trong kho!"
                    });
            }

            try
            {
                // ── Bước 3: Tạo đơn hàng ──
                var newOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerId = customer.Id,
                    Status = 0, // 0 = Chờ xử lý
                    Notes = $"Giao đến: {input.Address} | SĐT: {input.Phone}" +
                            (string.IsNullOrEmpty(input.Notes) ? "" : $" | {input.Notes}"),
                };
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync(); // Sinh OrderId

                // ── Bước 4: Lưu OrderDetails + trừ tồn kho ──
                foreach (var item in input.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);

                    // Lưu chi tiết đơn hàng
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = newOrder.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                    });

                    // Trừ tồn kho
                    product!.Stock -= item.Quantity;
                }

                await _context.SaveChangesAsync();

                // ── Bước 5: Gửi email xác nhận ──
                if (!string.IsNullOrEmpty(customer.Email) &&
                    !customer.Email.Contains("@guest.com"))
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
                        // Không để lỗi email hủy đơn hàng
                    }
                }

                return StatusCode(201, new
                {
                    message = "Đặt hàng thành công!",
                    orderId = newOrder.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Lỗi xử lý đơn hàng.",
                    detail = ex.Message
                });
            }
        }
    }

    // ── DTO ──
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
        public List<OrderItemDTO> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}