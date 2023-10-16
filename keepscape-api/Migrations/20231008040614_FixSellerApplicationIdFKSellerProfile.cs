using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class FixSellerApplicationIdFKSellerProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerProfiles_SellerApplications_SellerApplicationId",
                table: "SellerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_SellerProfiles_SellerApplicationId",
                table: "SellerProfiles");

            migrationBuilder.CreateIndex(
                name: "IX_SellerApplications_SellerProfileId",
                table: "SellerApplications",
                column: "SellerProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SellerApplications_SellerProfiles_SellerProfileId",
                table: "SellerApplications",
                column: "SellerProfileId",
                principalTable: "SellerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerApplications_SellerProfiles_SellerProfileId",
                table: "SellerApplications");

            migrationBuilder.DropIndex(
                name: "IX_SellerApplications_SellerProfileId",
                table: "SellerApplications");

            migrationBuilder.CreateIndex(
                name: "IX_SellerProfiles_SellerApplicationId",
                table: "SellerProfiles",
                column: "SellerApplicationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SellerProfiles_SellerApplications_SellerApplicationId",
                table: "SellerProfiles",
                column: "SellerApplicationId",
                principalTable: "SellerApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
