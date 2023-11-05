using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraIdInGifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemGifts_OrderItems_OrderItemId1",
                table: "OrderItemGifts");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemGifts_OrderItemId1",
                table: "OrderItemGifts");

            migrationBuilder.DropColumn(
                name: "OrderItemId1",
                table: "OrderItemGifts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderItemId1",
                table: "OrderItemGifts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemGifts_OrderItemId1",
                table: "OrderItemGifts",
                column: "OrderItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemGifts_OrderItems_OrderItemId1",
                table: "OrderItemGifts",
                column: "OrderItemId1",
                principalTable: "OrderItems",
                principalColumn: "Id");
        }
    }
}
