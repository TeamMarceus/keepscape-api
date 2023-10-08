using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class MakeImageInCategoryOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_BaseImages_BaseImageId",
                table: "Categories");

            migrationBuilder.AlterColumn<Guid>(
                name: "BaseImageId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_BaseImages_BaseImageId",
                table: "Categories",
                column: "BaseImageId",
                principalTable: "BaseImages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_BaseImages_BaseImageId",
                table: "Categories");

            migrationBuilder.AlterColumn<Guid>(
                name: "BaseImageId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_BaseImages_BaseImageId",
                table: "Categories",
                column: "BaseImageId",
                principalTable: "BaseImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
