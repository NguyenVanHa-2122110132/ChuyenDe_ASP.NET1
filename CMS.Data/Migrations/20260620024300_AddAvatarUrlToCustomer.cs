using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bỏ FK cũ trỏ tới bảng Users (CustomerId đã có sẵn, chỉ cần dọn UserId cũ)
            migrationBuilder.DropForeignKey(
                name: "FK_CouponUsages_Users_UserId",
                table: "CouponUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Users_UserId",
                table: "Wishlists");

            // Xóa index cũ gắn với UserId trước khi xóa cột
            migrationBuilder.DropIndex(
                name: "IX_CouponUsages_UserId",
                table: "CouponUsages");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists");

            // Xóa cột UserId cũ (đã có CustomerId thay thế)
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CouponUsages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Wishlists");

            // Thêm cột AvatarUrl cho Customer
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);



            migrationBuilder.CreateIndex(
                name: "IX_Comments_CustomerId",
                table: "Comments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsages_CustomerId",
                table: "CouponUsages",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_CustomerId",
                table: "Wishlists",
                column: "CustomerId");


            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

          


            migrationBuilder.DropIndex(
                name: "IX_Comments_CustomerId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_CouponUsages_CustomerId",
                table: "CouponUsages");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_CustomerId",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Wishlists",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "CouponUsages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsages_UserId",
                table: "CouponUsages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CouponUsages_Users_UserId",
                table: "CouponUsages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Users_UserId",
                table: "Wishlists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}