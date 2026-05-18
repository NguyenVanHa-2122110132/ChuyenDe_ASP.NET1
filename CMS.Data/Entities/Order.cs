/*
Họ Và Tên: Nguyễn Văn Hà
MSSV: 2122110132
Lớp: CCQ2211D
Ngày Tạo:16/05/2026
Mô Tả: Thực Thể Đơn Hàng*/
using System;
using System.Collections.Generic;
using System.Text;


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CMS.Data.Entities
{
    public class Order
    {
        [Key]// khoá chính
        public int Id { get; set; }// mã duy nhất của mỗi dơn hàng

        public DateTime OrderDate { get; set; } = DateTime.Now;// NGÀY ĐẶT HÀNG (Mặc định là thời điểm hiện tại khi tạo đơn)

        public int CustomerId { get; set; }//KHÓA NGOẠI (FOREIGN KEY) tới Khách hàng (Customer)

        public int Status { get; set; } // 0: Chờ duyệt, 1: Đang giao, 2: Đã xong

        public string? Notes { get; set; }//ghi chú đơn hàng

        [ForeignKey("CustomerId")]//CHỈ ĐỊNH KHÓA NGOẠI CHO NAVIGATION PROPERTY Customer
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
