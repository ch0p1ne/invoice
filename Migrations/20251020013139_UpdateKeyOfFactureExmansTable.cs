using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKeyOfFactureExmansTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacturesExamens_Examens_ExamensExamenId",
                table: "FacturesExamens");

            migrationBuilder.DropForeignKey(
                name: "FK_FacturesExamens_Factures_FacturesFactureId",
                table: "FacturesExamens");

            migrationBuilder.RenameColumn(
                name: "FacturesFactureId",
                table: "FacturesExamens",
                newName: "FactureId");

            migrationBuilder.RenameColumn(
                name: "ExamensExamenId",
                table: "FacturesExamens",
                newName: "ExamenId");

            migrationBuilder.RenameIndex(
                name: "IX_FacturesExamens_FacturesFactureId",
                table: "FacturesExamens",
                newName: "IX_FacturesExamens_FactureId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacturesExamens_Examens_ExamenId",
                table: "FacturesExamens",
                column: "ExamenId",
                principalTable: "Examens",
                principalColumn: "ExamenId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FacturesExamens_Factures_FactureId",
                table: "FacturesExamens",
                column: "FactureId",
                principalTable: "Factures",
                principalColumn: "FactureId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacturesExamens_Examens_ExamenId",
                table: "FacturesExamens");

            migrationBuilder.DropForeignKey(
                name: "FK_FacturesExamens_Factures_FactureId",
                table: "FacturesExamens");

            migrationBuilder.RenameColumn(
                name: "FactureId",
                table: "FacturesExamens",
                newName: "FacturesFactureId");

            migrationBuilder.RenameColumn(
                name: "ExamenId",
                table: "FacturesExamens",
                newName: "ExamensExamenId");

            migrationBuilder.RenameIndex(
                name: "IX_FacturesExamens_FactureId",
                table: "FacturesExamens",
                newName: "IX_FacturesExamens_FacturesFactureId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacturesExamens_Examens_ExamensExamenId",
                table: "FacturesExamens",
                column: "ExamensExamenId",
                principalTable: "Examens",
                principalColumn: "ExamenId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FacturesExamens_Factures_FacturesFactureId",
                table: "FacturesExamens",
                column: "FacturesFactureId",
                principalTable: "Factures",
                principalColumn: "FactureId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
