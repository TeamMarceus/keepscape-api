using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class FixDuplicateProductId1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReports_Products_ProductId1",
                table: "ProductReports");

            migrationBuilder.DropIndex(
                name: "IX_ProductReports_ProductId1",
                table: "ProductReports");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                table: "ProductReports",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReports_ProductId1",
                table: "ProductReports",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReports_Products_ProductId1",
                table: "ProductReports",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
