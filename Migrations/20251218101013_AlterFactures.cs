using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class AlterFactures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Css",
                table: "Factures",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0.1m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "ESCOMPT",
                table: "Factures",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0.9m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ESCOMPT",
                table: "Factures");

            migrationBuilder.AlterColumn<decimal>(
                name: "Css",
                table: "Factures",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldDefaultValue: 0.1m);
        }
    }
}
