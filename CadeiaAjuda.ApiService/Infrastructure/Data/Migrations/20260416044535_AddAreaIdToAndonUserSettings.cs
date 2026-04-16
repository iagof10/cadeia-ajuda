using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaIdToAndonUserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AreaId",
                table: "AndonUserSettings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AndonUserSettings_AreaId",
                table: "AndonUserSettings",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_AndonUserSettings_Areas_AreaId",
                table: "AndonUserSettings",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AndonUserSettings_Areas_AreaId",
                table: "AndonUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_AndonUserSettings_AreaId",
                table: "AndonUserSettings");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "AndonUserSettings");
        }
    }
}
