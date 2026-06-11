/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 01/06/2026 (Cập nhật sửa lỗi cấu hình khóa chính và bảng trung gian: 02/06/2026)
    Mô tả    : DbContext cấu hình kết nối database và mối quan hệ giữa các bảng
              - Categories          : Bảng danh mục bài viết
              - Posts               : Bảng bài viết
              - Users               : Bảng người dùng/thành viên
              - Products            : Bảng sản phẩm điện thoại
              - Customers           : Bảng khách hàng
              - Orders              : Bảng đơn hàng
              - OrderDetails        : Bảng chi tiết đơn hàng
              - CategoriesProducts  : Bảng trung gian liên kết Danh mục và Sản phẩm (Nhiều-Nhiều)
              - Comments            : Bảng bình luận của bài viết
              - Reviews             : Bảng đánh giá sản phẩm của người dùng
              - ReviewImages        : Bảng lưu trữ hình ảnh của đánh giá
              - Inventories         : Bảng quản lý số lượng tồn kho của sản phẩm
              - InventoryTransactions: Bảng lưu lịch sử biến động kho bãi
              - Coupons             : Bảng quản lý mã giảm giá khuyến mãi
              - CouponUsages        : Bảng lưu lịch sử áp dụng mã giảm giá của khách hàng
              - Carts               : Bảng quản lý giỏ hàng tổng của khách hàng
              - CartItems           : Bảng quản lý chi tiết các món hàng trong giỏ của khách
              - Wishlists           : Bảng quản lý danh sách sản phẩm yêu thích của người dùng
              - WishlistItems       : Bảng quản lý chi tiết các mặt hàng trong danh sách yêu thích
              - Shippings           : Bảng quản lý thông tin giao hàng và trạng thái vận chuyển của đơn hàng
              - Payments            : Bảng quản lý thông tin và lịch sử trạng thái thanh toán đơn hàng
              - ProductImages       : Bảng quản lý album hình ảnh phụ và ảnh đại diện của sản phẩm
              - PostCategories      : Bảng danh mục bài viết blog
              - PostPostCategories  : Bảng trung gian liên kết Bài viết và Danh mục bài viết (Nhiều-Nhiều)
              - Tags                : Bảng quản lý các thẻ/từ khóa của bài viết blog
              - PostTags            : Bảng trung gian liên kết Bài viết và Thẻ (Nhiều-Nhiều)
*/
using CMS.Data.Entities;             // Các entity/thực thể của hệ thống
using Microsoft.EntityFrameworkCore; // Thư viện Entity Framework Core

namespace CMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ========================================================
        public DbSet<Category> Categories { get; set; }               // Bảng danh mục
        public DbSet<Post> Posts { get; set; }                        // Bảng bài viết
        public DbSet<User> Users { get; set; }                        // Bảng người dùng
        public DbSet<Product> Products { get; set; }                  // Bảng sản phẩm
        public DbSet<Customer> Customers { get; set; }                // Bảng khách hàng
        public DbSet<Order> Orders { get; set; }                      // Bảng đơn hàng
        public DbSet<OrderDetail> OrderDetails { get; set; }          // Bảng chi tiết đơn hàng
        public DbSet<CategoryProduct> CategoriesProducts { get; set; } // Bảng trung gian nhiều-nhiều
        public DbSet<Comment> Comments { get; set; }                  // Bảng bình luận bài viết
        public DbSet<Review> Reviews { get; set; }                    // Bảng đánh giá sản phẩm
        public DbSet<ReviewImage> ReviewImages { get; set; }          // Bảng hình ảnh kèm theo đánh giá
        public DbSet<Inventory> Inventories { get; set; }             // Bảng quản lý số lượng tồn kho của sản phẩm
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; } // Bảng lưu lịch sử biến động kho bãi
        public DbSet<Coupon> Coupons { get; set; }                    // Bảng quản lý mã giảm giá khuyến mãi
        public DbSet<CouponUsage> CouponUsages { get; set; }          // Bảng lưu lịch sử áp dụng mã giảm giá của khách hàng
        public DbSet<Cart> Carts { get; set; }                        // Bảng quản lý giỏ hàng tổng của khách hàng
        public DbSet<CartItem> CartItems { get; set; }                // Bảng quản lý chi tiết các món hàng trong giỏ của khách
        public DbSet<Wishlist> Wishlists { get; set; }                // Bảng quản lý danh sách sản phẩm yêu thích của người dùng
        public DbSet<WishlistItem> WishlistItems { get; set; }        // Bảng quản lý chi tiết các mặt hàng trong danh sách yêu thích
        public DbSet<Shipping> Shippings { get; set; }                // Bảng quản lý thông tin giao hàng và trạng thái vận chuyển của đơn hàng
        public DbSet<Payment> Payments { get; set; }                  // Bảng quản lý thông tin và lịch sử trạng thái thanh toán đơn hàng
        public DbSet<ProductImage> ProductImages { get; set; }        // Bảng quản lý album hình ảnh phụ và ảnh đại diện của sản phẩm
        public DbSet<PostCategory> PostCategories { get; set; }        // Bảng danh mục bài viết blog
        public DbSet<PostPostCategory> PostPostCategories { get; set; } // Bảng trung gian liên kết Bài viết và Danh mục bài viết (Nhiều-Nhiều)
        public DbSet<Tag> Tags { get; set; }                          // Bảng quản lý các thẻ/từ khóa của bài viết blog
        public DbSet<PostTag> PostTags { get; set; }                  // Bảng trung gian liên kết giữa Bài viết và Thẻ tag (Nhiều-Nhiều) - ĐÃ GOM GỌN DÒNG TRÙNG LẶP

        // ========================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình khóa chính kép cho bảng trung gian CategoryProduct
            modelBuilder.Entity<CategoryProduct>()
                .HasKey(cp => new { cp.CategoryId, cp.ProductId });

            modelBuilder.Entity<CategoryProduct>()
                .HasOne(cp => cp.Category)
                .WithMany(c => c.CategoryProducts)
                .HasForeignKey(cp => cp.CategoryId);

            modelBuilder.Entity<CategoryProduct>()
                .HasOne(cp => cp.Product)
                .WithMany(p => p.CategoryProducts)
                .HasForeignKey(cp => cp.ProductId);

            // 2. Cấu hình quan hệ tự tham chiếu (Self-referencing) cho Comment (Bình luận gốc và Phản hồi con)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Cấu hình quan hệ 1-Nhiều từ Review -> ReviewImage (Một đánh giá có nhiều ảnh)
            modelBuilder.Entity<ReviewImage>()
                .HasOne(ri => ri.Review)
                .WithMany(r => r.ReviewImages)
                .HasForeignKey(ri => ri.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4.Cấu hình khóa chính kép cho bảng trung gian PostPostCategory (Sửa lỗi sập màn hình)
            modelBuilder.Entity<PostPostCategory>()
                .HasKey(ppc => new { ppc.PostId, ppc.PostCategoryId });

            modelBuilder.Entity<PostPostCategory>()
                .HasOne(ppc => ppc.PostCategory)
                .WithMany(pc => pc.PostPostCategories)
                .HasForeignKey(ppc => ppc.PostCategoryId);

            // 5.  Cấu hình khóa chính kép cho bảng trung gian PostTag
            modelBuilder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany()
                .HasForeignKey(pt => pt.PostId);

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId);
        }
    }
}