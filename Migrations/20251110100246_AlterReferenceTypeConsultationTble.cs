using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class AlterReferenceTypeConsultationTble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "Consultations",
                type: "nvarchar(8)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ConsultationName",
                table: "Consultations",
                type: "nvarchar(99)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Reference",
                table: "Consultations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)");

            migrationBuilder.AlterColumn<string>(
                name: "ConsultationName",
                table: "Consultations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(99)");
        }
    }
}
