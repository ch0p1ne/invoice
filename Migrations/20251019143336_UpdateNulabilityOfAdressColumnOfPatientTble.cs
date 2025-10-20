using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNulabilityOfAdressColumnOfPatientTble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Assurances_AssuranceId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_AssuranceNumber",
                table: "Patients");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patients",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "AssuranceNumber",
                table: "Patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AssuranceId",
                table: "Patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AssuranceNumber",
                table: "Patients",
                column: "AssuranceNumber",
                unique: true,
                filter: "[AssuranceNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Assurances_AssuranceId",
                table: "Patients",
                column: "AssuranceId",
                principalTable: "Assurances",
                principalColumn: "AssuranceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Assurances_AssuranceId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_AssuranceNumber",
                table: "Patients");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssuranceNumber",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssuranceId",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AssuranceNumber",
                table: "Patients",
                column: "AssuranceNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Assurances_AssuranceId",
                table: "Patients",
                column: "AssuranceId",
                principalTable: "Assurances",
                principalColumn: "AssuranceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
