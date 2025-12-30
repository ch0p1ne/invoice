using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class updateTpsAndCssDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TPS",
                table: "Factures",
                type: "decimal(4,3)",
                nullable: false,
                defaultValue: 0.095m,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Css",
                table: "Factures",
                type: "decimal(4,3)",
                nullable: false,
                defaultValue: 0.01m,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldDefaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TPS",
                table: "Factures",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,3)",
                oldDefaultValue: 0.095m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Css",
                table: "Factures",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,3)",
                oldDefaultValue: 0.01m);
        }
    }
}
