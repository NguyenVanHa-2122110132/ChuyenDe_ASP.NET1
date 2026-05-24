/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 18/05/2026
    Mô tả    : DbContext cấu hình kết nối database và mối quan hệ giữa các bảng
              - Categories      : Bảng danh mục bài viết
              - Posts           : Bảng bài viết
              - Users           : Bảng người dùng/thành viên
              - Products        : Bảng sản phẩm điện thoại
              - Customers       : Bảng khách hàng
              - Orders          : Bảng đơn hàng
              - OrderDetails    : Bảng chi tiết đơn hàng
              - CategoriesProducts : Bảng trung gian liên kết Danh mục và Sản phẩm (Nhiều-Nhiều)
*/
using Microsoft.EntityFrameworkCore; // Thư viện Entity Framework Core
using CMS.Data.Entities;             // Các entity/thực thể của hệ thống

namespace CMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }             // Bảng danh mục
        public DbSet<Post> Posts { get; set; }                      // Bảng bài viết
        public DbSet<User> Users { get; set; }                      // Bảng người dùng
        public DbSet<Product> Products { get; set; }                // Bảng sản phẩm
        public DbSet<Customer> Customers { get; set; }              // Bảng khách hàng
        public DbSet<Order> Orders { get; set; }                    // Bảng đơn hàng
        public DbSet<OrderDetail> OrderDetails { get; set; }        // Bảng chi tiết đơn hàng
        public DbSet<CategoryProduct> CategoriesProducts { get; set; } // Bảng trung gian nhiều-nhiều

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình khóa chính kép cho bảng trung gian CategoryProduct
            modelBuilder.Entity<CategoryProduct>()
                .HasKey(cp => new { cp.CategoryId, cp.ProductId });

            // Cấu hình quan hệ 1-Nhiều từ Category -> CategoryProduct
            modelBuilder.Entity<CategoryProduct>()
                .HasOne(cp => cp.Category)
                .WithMany(c => c.CategoryProducts)
                .HasForeignKey(cp => cp.CategoryId);

            // Cấu hình quan hệ 1-Nhiều từ Product -> CategoryProduct
            modelBuilder.Entity<CategoryProduct>()
                .HasOne(cp => cp.Product)
                .WithMany(p => p.CategoryProducts)
                .HasForeignKey(cp => cp.ProductId);
        }
    }
}