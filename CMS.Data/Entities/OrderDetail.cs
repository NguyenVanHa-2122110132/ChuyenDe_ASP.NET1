/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : Thực thể Chi tiết Đơn hàng (OrderDetail) - ánh xạ tới bảng OrderDetails trong database
              - Id        : Khóa chính, tự tăng
              - OrderId   : Khóa ngoại liên kết tới bảng Order
              - ProductId : Khóa ngoại liên kết tới bảng Product
              - Quantity  : Số lượng sản phẩm trong đơn hàng
              - UnitPrice : Đơn giá tại thời điểm đặt hàng
              - Order     : Navigation property để truy xuất thông tin đơn hàng
              - Product   : Navigation property để truy xuất thông tin sản phẩm
*/
namespace CMS.Data.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }                      // Khóa chính, tự tăng
        public int OrderId { get; set; }                 // Khóa ngoại liên kết tới bảng Order
        public int ProductId { get; set; }               // Khóa ngoại liên kết tới bảng Product
        public int Quantity { get; set; }                // Số lượng sản phẩm đặt mua
        public decimal UnitPrice { get; set; }           // Đơn giá tại thời điểm đặt hàng
        public virtual Order? Order { get; set; }        // Navigation property — truy xuất đơn hàng, cho phép null
        public virtual Product? Product { get; set; }   // Navigation property — truy xuất sản phẩm, cho phép null
    }
}