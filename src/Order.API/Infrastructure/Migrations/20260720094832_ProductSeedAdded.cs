using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Order.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductSeedAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Orders_OrderModelId",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem");

            migrationBuilder.RenameTable(
                name: "OrderItem",
                newName: "OrderItems");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_OrderModelId",
                table: "OrderItems",
                newName: "IX_OrderItems_OrderModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("018f4e2a-7b91-7c3d-8a2f-000000000001"), "Laptop for work, gaming, etc.", "Laptop Asus TUF Gaming A15", 44299.00m },
                    { new Guid("018f4e2a-7b91-7c3d-8a2f-000000000002"), "", "Apple Iphone 17 Pro", 39999.00m }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderModelId",
                table: "OrderItems",
                column: "OrderModelId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderModelId",
                table: "OrderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("018f4e2a-7b91-7c3d-8a2f-000000000001"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("018f4e2a-7b91-7c3d-8a2f-000000000002"));

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "OrderItem");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_OrderModelId",
                table: "OrderItem",
                newName: "IX_OrderItem_OrderModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Orders_OrderModelId",
                table: "OrderItem",
                column: "OrderModelId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
