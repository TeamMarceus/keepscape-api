using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class MakeImagesUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_BaseImages_BaseImageId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_BaseImages_BaseImageId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Places_BaseImages_BaseImageId",
                table: "Places");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerApplications_BaseImages_BaseImageId",
                table: "SellerApplications");

            migrationBuilder.DropTable(
                name: "BaseImages");

            migrationBuilder.DropIndex(
                name: "IX_SellerApplications_BaseImageId",
                table: "SellerApplications");

            migrationBuilder.DropIndex(
                name: "IX_Places_BaseImageId",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BaseImageId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Categories_BaseImageId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "BaseImageId",
                table: "SellerApplications");

            migrationBuilder.DropColumn(
                name: "BaseImageId",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "BaseImageId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BaseImageId",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "StatusReason",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BusinessPermitUrl",
                table: "SellerApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdImageUrl",
                table: "SellerApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatusReason",
                table: "SellerApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Places",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryFeeProofImageUrl",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRImageUrl",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTimeCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropColumn(
                name: "StatusReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BusinessPermitUrl",
                table: "SellerApplications");

            migrationBuilder.DropColumn(
                name: "IdImageUrl",
                table: "SellerApplications");

            migrationBuilder.DropColumn(
                name: "StatusReason",
                table: "SellerApplications");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryFeeProofImageUrl",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "QRImageUrl",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Categories");

            migrationBuilder.AddColumn<Guid>(
                name: "BaseImageId",
                table: "SellerApplications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BaseImageId",
                table: "Places",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BaseImageId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BaseImageId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BaseImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Alt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTimeCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellerApplications_BaseImageId",
                table: "SellerApplications",
                column: "BaseImageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Places_BaseImageId",
                table: "Places",
                column: "BaseImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BaseImageId",
                table: "Orders",
                column: "BaseImageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_BaseImageId",
                table: "Categories",
                column: "BaseImageId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseImages_ProductId",
                table: "BaseImages",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_BaseImages_BaseImageId",
                table: "Categories",
                column: "BaseImageId",
                principalTable: "BaseImages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_BaseImages_BaseImageId",
                table: "Orders",
                column: "BaseImageId",
                principalTable: "BaseImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Places_BaseImages_BaseImageId",
                table: "Places",
                column: "BaseImageId",
                principalTable: "BaseImages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerApplications_BaseImages_BaseImageId",
                table: "SellerApplications",
                column: "BaseImageId",
                principalTable: "BaseImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
