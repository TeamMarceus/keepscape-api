using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class AddBalanceIdInBalanceWithdrawal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceWithdrawals_SellerProfiles_SellerProfileId",
                table: "BalanceWithdrawals");

            migrationBuilder.RenameColumn(
                name: "SellerProfileId",
                table: "BalanceWithdrawals",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BalanceWithdrawals_SellerProfileId",
                table: "BalanceWithdrawals",
                newName: "IX_BalanceWithdrawals_UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "BalanceId",
                table: "BalanceWithdrawals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BalanceWithdrawals_BalanceId",
                table: "BalanceWithdrawals",
                column: "BalanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceWithdrawals_Balances_BalanceId",
                table: "BalanceWithdrawals",
                column: "BalanceId",
                principalTable: "Balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceWithdrawals_Users_UserId",
                table: "BalanceWithdrawals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceWithdrawals_Balances_BalanceId",
                table: "BalanceWithdrawals");

            migrationBuilder.DropForeignKey(
                name: "FK_BalanceWithdrawals_Users_UserId",
                table: "BalanceWithdrawals");

            migrationBuilder.DropIndex(
                name: "IX_BalanceWithdrawals_BalanceId",
                table: "BalanceWithdrawals");

            migrationBuilder.DropColumn(
                name: "BalanceId",
                table: "BalanceWithdrawals");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BalanceWithdrawals",
                newName: "SellerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_BalanceWithdrawals_UserId",
                table: "BalanceWithdrawals",
                newName: "IX_BalanceWithdrawals_SellerProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceWithdrawals_SellerProfiles_SellerProfileId",
                table: "BalanceWithdrawals",
                column: "SellerProfileId",
                principalTable: "SellerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
