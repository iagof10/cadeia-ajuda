using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadeiaAjuda.ApiService.Infrastructure.Data.Migrations
{
    public partial class AddTenantUserTypeLimits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdministratorUserLimit",
                table: "Tenants",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AndonUserLimit",
                table: "Tenants",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerUserLimit",
                table: "Tenants",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandardUserLimit",
                table: "Tenants",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministratorUserLimit",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "AndonUserLimit",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ManagerUserLimit",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "StandardUserLimit",
                table: "Tenants");
        }
    }
}
