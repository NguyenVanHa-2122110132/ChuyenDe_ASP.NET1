/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : API Controller quản lý Đơn hàng (OrdersController)
              - CreateOrder() : Tiếp nhận đơn đặt hàng từ FrontEnd gửi lên qua [HttpPost]
                                Tự động gán ngày đặt hàng và trạng thái mặc định là 0 (Chờ xử lý)
                                Trả về 201 Created kèm mã Id đơn hàng vừa tạo
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CMS.Backend.Controllers
{
    // Định nghĩa đường dẫn API: https://localhost:xxxx/api/Orders
    [Route("api/[controller]")]

    // Kích hoạt tính năng tự động kiểm tra dữ liệu đầu vào
    [ApiController]

    // Kế thừa ControllerBase để tối ưu bộ nhớ cho API thuần JSON
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Hàm khởi tạo: Tiêm ngữ cảnh dữ liệu SQL Server vào Controller thông qua DI
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // API: Tiếp nhận đơn đặt hàng từ giỏ hàng FrontEnd gửi lên
        // Đường dẫn: POST https://localhost:xxxx/api/Orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderInputDTO input)
        {
            // Kiểm tra dữ liệu truyền lên có hợp lệ không
            if (input == null)
            {
                return BadRequest(new { message = "Dữ liệu đơn hàng không hợp lệ" });
            }

            try
            {
                // Bước A: Khởi tạo đối tượng đơn hàng mới
                var newOrder = new Order
                {
                    OrderDate = DateTime.Now, // Tự động lấy ngày giờ thực tế lúc đặt hàng
                    CustomerId = input.CustomerId,
                    Status = 0,               // 0: Trạng thái mặc định "Chờ xử lý"
                    Notes = input.Notes
                };

                // Bước B: Thêm vào bảng tạm và lưu xuống SQL Server
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync(); // Sinh ra mã Id đơn hàng tự động tăng

                // Bước C: Trả về mã 201 Created kèm Id đơn hàng vừa tạo
                return StatusCode(201, new
                {
                    message = "Đặt hàng thành công!",
                    orderId = newOrder.Id
                });
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu có sự cố kết nối hoặc lỗi logic
                return StatusCode(500, new { message = "Lỗi xử lý tạo đơn hàng", detail = ex.Message });
            }
        }
    }

    // Lớp DTO trung gian để hứng dữ liệu từ FrontEnd truyền lên
    public class OrderInputDTO
    {
        public int CustomerId { get; set; }  // Mã khách hàng đặt hàng
        public string? Notes { get; set; }   // Ghi chú đơn hàng, cho phép null
    }
}