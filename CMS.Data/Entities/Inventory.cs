using CMS.Data.Entities;
using System;

namespace CMS.Data.Entities
{
    public enum InventoryTransactionType
    {
        Import = 0,          // Nhập kho
        Export = 1,          // Xuất kho (bán hàng)
        Adjustment = 2,      // Điều chỉnh tồn kho
        Return = 3,          // Nhập hàng hoàn trả
        Transfer = 4         // Chuyển kho
    }

    public class Inventory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? WarehouseLocation { get; set; }   // Vị trí kho
        public int QuantityInStock { get; set; } = 0;
        public int QuantityReserved { get; set; } = 0;   // Đã đặt nhưng chưa xuất
        public int LowStockThreshold { get; set; } = 5;  // Ngưỡng cảnh báo sắp hết
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Computed: số lượng thực tế có thể bán
        public int AvailableQuantity => QuantityInStock - QuantityReserved;

        // Navigation properties
        public Product Product { get; set; }
    }

    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public InventoryTransactionType Type { get; set; }
        public int Quantity { get; set; }                // Dương: nhập, Âm: xuất
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
        public string? Reason { get; set; }
        public int? ReferenceId { get; set; }            // OrderId hoặc PurchaseOrderId
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Inventory Inventory { get; set; }
        public User CreatedByUser { get; set; }
    }
}