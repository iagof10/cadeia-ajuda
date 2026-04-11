using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEscalationLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EscalationLevels_Roles_ResponsibleRoleId",
                table: "EscalationLevels");

            migrationBuilder.DropIndex(
                name: "IX_EscalationLevels_TenantId_Order",
                table: "EscalationLevels");

            migrationBuilder.RenameColumn(
                name: "ResponsibleRoleId",
                table: "EscalationLevels",
                newName: "SectorId");

            migrationBuilder.RenameIndex(
                name: "IX_EscalationLevels_ResponsibleRoleId",
                table: "EscalationLevels",
                newName: "IX_EscalationLevels_SectorId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EscalationLevels",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EscalationLevelResponsibles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EscalationLevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscalationLevelResponsibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EscalationLevelResponsibles_EscalationLevels_EscalationLeve~",
                        column: x => x.EscalationLevelId,
                        principalTable: "EscalationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscalationLevelResponsibles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EscalationLevels_TenantId_SectorId_Order",
                table: "EscalationLevels",
                columns: new[] { "TenantId", "SectorId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EscalationLevelResponsibles_EscalationLevelId_UserId",
                table: "EscalationLevelResponsibles",
                columns: new[] { "EscalationLevelId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EscalationLevelResponsibles_UserId",
                table: "EscalationLevelResponsibles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EscalationLevels_Sectors_SectorId",
                table: "EscalationLevels",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EscalationLevels_Sectors_SectorId",
                table: "EscalationLevels");

            migrationBuilder.DropTable(
                name: "EscalationLevelResponsibles");

            migrationBuilder.DropIndex(
                name: "IX_EscalationLevels_TenantId_SectorId_Order",
                table: "EscalationLevels");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "EscalationLevels");

            migrationBuilder.RenameColumn(
                name: "SectorId",
                table: "EscalationLevels",
                newName: "ResponsibleRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_EscalationLevels_SectorId",
                table: "EscalationLevels",
                newName: "IX_EscalationLevels_ResponsibleRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationLevels_TenantId_Order",
                table: "EscalationLevels",
                columns: new[] { "TenantId", "Order" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EscalationLevels_Roles_ResponsibleRoleId",
                table: "EscalationLevels",
                column: "ResponsibleRoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
