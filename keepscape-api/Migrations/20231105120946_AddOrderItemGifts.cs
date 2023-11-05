using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemGifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderItemGifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderItemId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateTimeCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemGifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemGifts_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemGifts_OrderItems_OrderItemId1",
                        column: x => x.OrderItemId1,
                        principalTable: "OrderItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItemGifts_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItemGifts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemGifts_OrderItemId",
                table: "OrderItemGifts",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemGifts_OrderItemId1",
                table: "OrderItemGifts",
                column: "OrderItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemGifts_PlaceId",
                table: "OrderItemGifts",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemGifts_UserId",
                table: "OrderItemGifts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemGifts");
        }
    }
}
