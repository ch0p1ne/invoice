using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class AddRolePermissionJoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RolesPermissions",
                table: "RolesPermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolesPermissions_RoleId",
                table: "RolesPermissions");

            migrationBuilder.AddColumn<DateTime>(
                name: "GrantedAt",
                table: "RolesPermissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolesPermissions",
                table: "RolesPermissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolesPermissions_PermissionId",
                table: "RolesPermissions",
                column: "PermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RolesPermissions",
                table: "RolesPermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolesPermissions_PermissionId",
                table: "RolesPermissions");

            migrationBuilder.DropColumn(
                name: "GrantedAt",
                table: "RolesPermissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolesPermissions",
                table: "RolesPermissions",
                columns: new[] { "PermissionId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolesPermissions_RoleId",
                table: "RolesPermissions",
                column: "RoleId");
        }
    }
}
