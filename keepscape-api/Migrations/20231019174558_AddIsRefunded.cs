using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRefunded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRefunded",
                table: "OrderReports",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRefunded",
                table: "OrderReports");
        }
    }
}
