using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSectorToHelpRequestType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SectorId",
                table: "HelpRequestTypes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_HelpRequestTypes_SectorId",
                table: "HelpRequestTypes",
                column: "SectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpRequestTypes_Sectors_SectorId",
                table: "HelpRequestTypes",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpRequestTypes_Sectors_SectorId",
                table: "HelpRequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_HelpRequestTypes_SectorId",
                table: "HelpRequestTypes");

            migrationBuilder.DropColumn(
                name: "SectorId",
                table: "HelpRequestTypes");
        }
    }
}
