using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace keepscape_api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdInBalanceWithdrawal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceWithdrawals_Users_UserId",
                table: "BalanceWithdrawals");

            migrationBuilder.DropIndex(
                name: "IX_BalanceWithdrawals_UserId",
                table: "BalanceWithdrawals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BalanceWithdrawals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "BalanceWithdrawals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BalanceWithdrawals_UserId",
                table: "BalanceWithdrawals",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceWithdrawals_Users_UserId",
                table: "BalanceWithdrawals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
