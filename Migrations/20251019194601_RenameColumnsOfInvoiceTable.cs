using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnsOfInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount_flat", 
                table: "Factures");

            migrationBuilder.RenameColumn(
                name: "Payment_method",
                table: "Factures",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "Patient_responsibility",
                table: "Factures",
                newName: "PatientPercent");

            migrationBuilder.RenameColumn(
                name: "Discount_percent",
                table: "Factures",
                newName: "DiscountPercent");

            migrationBuilder.RenameColumn(
                name: "Amount_paid",
                table: "Factures",
                newName: "AmountPaid");

            migrationBuilder.RenameColumn(
                name: "Assurance_coverage_percent",
                table: "Factures",
                newName: "InsuranceCoveragePercent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "Factures",
                newName: "Payment_method");

            migrationBuilder.RenameColumn(
                name: "PatientPercent",
                table: "Factures",
                newName: "Patient_responsibility");

            migrationBuilder.RenameColumn(
                name: "InsuranceCoveragePercent",
                table: "Factures",
                newName: "Assurance_coverage_percent"); 

            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "Factures",
                newName: "Discount_percent");

            migrationBuilder.RenameColumn(
                name: "AmountPaid",
                table: "Factures",
                newName: "Amount_paid"); 

            migrationBuilder.AddColumn<decimal>(
                name: "Discount_flat",
                table: "Factures",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
