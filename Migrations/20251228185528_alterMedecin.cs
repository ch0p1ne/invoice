using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class alterMedecin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedecinName",
                table: "Medecins");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumberTwo",
                table: "Medecins",
                type: "nvarchar(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumberOne",
                table: "Medecins",
                type: "nvarchar(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Medecins",
                type: "nvarchar(75)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedecinFirstName",
                table: "Medecins",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedecinLastName",
                table: "Medecins",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Speciality",
                table: "Medecins",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Medecins");

            migrationBuilder.DropColumn(
                name: "MedecinFirstName",
                table: "Medecins");

            migrationBuilder.DropColumn(
                name: "MedecinLastName",
                table: "Medecins");

            migrationBuilder.DropColumn(
                name: "Speciality",
                table: "Medecins");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumberTwo",
                table: "Medecins",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumberOne",
                table: "Medecins",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedecinName",
                table: "Medecins",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
