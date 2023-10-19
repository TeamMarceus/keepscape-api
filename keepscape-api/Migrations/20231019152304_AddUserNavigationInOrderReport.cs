using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNavigationInOrderReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReports_Products_ProductGuid",
                table: "ProductReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReports_Users_UserGuid",
                table: "ProductReports");

            migrationBuilder.DropIndex(
                name: "IX_OrderReports_OrderId",
                table: "OrderReports");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "UserGuid",
                table: "ProductReports",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ProductGuid",
                table: "ProductReports",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReports_UserGuid",
                table: "ProductReports",
                newName: "IX_ProductReports_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReports_ProductGuid",
                table: "ProductReports",
                newName: "IX_ProductReports_ProductId");

            migrationBuilder.RenameColumn(
                name: "UserGuid",
                table: "OrderReports",
                newName: "UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                table: "ProductReports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReports_ProductId1",
                table: "ProductReports",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderReports_OrderId",
                table: "OrderReports",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderReports_UserId",
                table: "OrderReports",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderReports_Users_UserId",
                table: "OrderReports",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReports_Products_ProductId",
                table: "ProductReports",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReports_Products_ProductId1",
                table: "ProductReports",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReports_Users_UserId",
                table: "ProductReports",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderReports_Users_UserId",
                table: "OrderReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReports_Products_ProductId",
                table: "ProductReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReports_Products_ProductId1",
                table: "ProductReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReports_Users_UserId",
                table: "ProductReports");

            migrationBuilder.DropIndex(
                name: "IX_ProductReports_ProductId1",
                table: "ProductReports");

            migrationBuilder.DropIndex(
                name: "IX_OrderReports_OrderId",
                table: "OrderReports");

            migrationBuilder.DropIndex(
                name: "IX_OrderReports_UserId",
                table: "OrderReports");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductReports");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ProductReports",
                newName: "UserGuid");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductReports",
                newName: "ProductGuid");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReports_UserId",
                table: "ProductReports",
                newName: "IX_ProductReports_UserGuid");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReports_ProductId",
                table: "ProductReports",
                newName: "IX_ProductReports_ProductGuid");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "OrderReports",
                newName: "UserGuid");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OrderReports_OrderId",
                table: "OrderReports",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReports_Products_ProductGuid",
                table: "ProductReports",
                column: "ProductGuid",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReports_Users_UserGuid",
                table: "ProductReports",
                column: "UserGuid",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
