using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductCategoryFieldNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductCategoryName",
                table: "ProductCategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "RegionName",
                table: "PlaceCategories",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductCategories",
                newName: "ProductCategoryName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PlaceCategories",
                newName: "RegionName");
        }
    }
}
