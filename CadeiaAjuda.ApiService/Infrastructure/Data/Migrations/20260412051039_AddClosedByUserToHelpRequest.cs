using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClosedByUserToHelpRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClosedByUserId",
                table: "HelpRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HelpRequests_ClosedByUserId",
                table: "HelpRequests",
                column: "ClosedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpRequests_Users_ClosedByUserId",
                table: "HelpRequests",
                column: "ClosedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpRequests_Users_ClosedByUserId",
                table: "HelpRequests");

            migrationBuilder.DropIndex(
                name: "IX_HelpRequests_ClosedByUserId",
                table: "HelpRequests");

            migrationBuilder.DropColumn(
                name: "ClosedByUserId",
                table: "HelpRequests");
        }
    }
}
