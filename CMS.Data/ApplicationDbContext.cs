/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 17/05/2026
    Mô tả    : DbContext trung tâm - Kết nối Entity Framework Core với SQL Server
              Quản lý các bảng: Categories, Posts, Users
*/

using Microsoft.EntityFrameworkCore;
using CMS.Data.Entities;

namespace CMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 3 bảng cũ (đã có)
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        // 5 bảng MỚI CẦN THÊM VÀO ĐÂY
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<CategoryProduct> CategoriesProducts { get; set; }
    }
}