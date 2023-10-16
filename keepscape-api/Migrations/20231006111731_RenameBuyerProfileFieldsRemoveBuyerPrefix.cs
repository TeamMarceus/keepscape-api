using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class RenameBuyerProfileFieldsRemoveBuyerPrefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BuyerPreferences",
                table: "BuyerProfiles",
                newName: "Preferences");

            migrationBuilder.RenameColumn(
                name: "BuyerInterests",
                table: "BuyerProfiles",
                newName: "Interests");

            migrationBuilder.RenameColumn(
                name: "BuyerDescription",
                table: "BuyerProfiles",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "BuyerDeliveryFullName",
                table: "BuyerProfiles",
                newName: "DeliveryFullName");

            migrationBuilder.RenameColumn(
                name: "BuyerDeliveryAddress",
                table: "BuyerProfiles",
                newName: "DeliveryAddress");

            migrationBuilder.RenameColumn(
                name: "BuyerAltMobileNumber",
                table: "BuyerProfiles",
                newName: "AltMobileNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Preferences",
                table: "BuyerProfiles",
                newName: "BuyerPreferences");

            migrationBuilder.RenameColumn(
                name: "Interests",
                table: "BuyerProfiles",
                newName: "BuyerInterests");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "BuyerProfiles",
                newName: "BuyerDescription");

            migrationBuilder.RenameColumn(
                name: "DeliveryFullName",
                table: "BuyerProfiles",
                newName: "BuyerDeliveryFullName");

            migrationBuilder.RenameColumn(
                name: "DeliveryAddress",
                table: "BuyerProfiles",
                newName: "BuyerDeliveryAddress");

            migrationBuilder.RenameColumn(
                name: "AltMobileNumber",
                table: "BuyerProfiles",
                newName: "BuyerAltMobileNumber");
        }
    }
}
