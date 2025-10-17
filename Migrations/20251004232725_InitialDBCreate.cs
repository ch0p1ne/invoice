using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class InitialDBCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assurances",
                columns: table => new
                {
                    AssuranceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Compagny = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CoveragePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assurances", x => x.AssuranceId);
                });

            migrationBuilder.CreateTable(
                name: "Consultations",
                columns: table => new
                {
                    ConsultationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<int>(type: "int", nullable: false),
                    ConsultationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultations", x => x.ConsultationId);
                });

            migrationBuilder.CreateTable(
                name: "Examens",
                columns: table => new
                {
                    ExamenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<int>(type: "int", nullable: false),
                    ExamenName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examens", x => x.ExamenId);
                });

            migrationBuilder.CreateTable(
                name: "GreetingMessages",
                columns: table => new
                {
                    GreetingMessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeginDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GreetingMessages", x => x.GreetingMessageId);
                });

            migrationBuilder.CreateTable(
                name: "Medecins",
                columns: table => new
                {
                    MedecinId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedecinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartWork = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndWork = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medecins", x => x.MedecinId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Create_patient = table.Column<int>(type: "int", nullable: false),
                    Create_fac = table.Column<int>(type: "int", nullable: false),
                    Manage_fac = table.Column<int>(type: "int", nullable: false),
                    Manage_user = table.Column<int>(type: "int", nullable: false),
                    Manage_med = table.Column<int>(type: "int", nullable: false),
                    Manage_exam = table.Column<int>(type: "int", nullable: false),
                    Manage_consul = table.Column<int>(type: "int", nullable: false),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_name = table.Column<string>(type: "nvarchar(75)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Phone_number_one = table.Column<string>(type: "nvarchar(15)", nullable: false),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssuranceNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssuranceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_Patients_Assurances_AssuranceId",
                        column: x => x.AssuranceId,
                        principalTable: "Assurances",
                        principalColumn: "AssuranceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRoles", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UsersRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Factures",
                columns: table => new
                {
                    FactureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Total_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Tva = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    Css = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Assurance_coverage_percent = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    Patient_responsibility = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    Amount_paid = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Discount_percent = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    Discount_flat = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Payment_method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factures", x => x.FactureId);
                    table.ForeignKey(
                        name: "FK_Factures_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Factures_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FactureAvoirs",
                columns: table => new
                {
                    FactureAvoirId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Raison = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reference = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Css = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssuranceCoveragePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PatientResponsibility = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount_percent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount_flat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FactureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactureAvoirs", x => x.FactureAvoirId);
                    table.ForeignKey(
                        name: "FK_FactureAvoirs_Factures_FactureId",
                        column: x => x.FactureId,
                        principalTable: "Factures",
                        principalColumn: "FactureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FactureConsultation",
                columns: table => new
                {
                    FactureId = table.Column<int>(type: "int", nullable: false),
                    ConsultationId = table.Column<int>(type: "int", nullable: false),
                    Qte = table.Column<int>(type: "int", nullable: false),
                    MedecinId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactureConsultation", x => new { x.ConsultationId, x.FactureId });
                    table.ForeignKey(
                        name: "FK_FactureConsultation_Consultations_ConsultationId",
                        column: x => x.ConsultationId,
                        principalTable: "Consultations",
                        principalColumn: "ConsultationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactureConsultation_Factures_FactureId",
                        column: x => x.FactureId,
                        principalTable: "Factures",
                        principalColumn: "FactureId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactureConsultation_Medecins_MedecinId",
                        column: x => x.MedecinId,
                        principalTable: "Medecins",
                        principalColumn: "MedecinId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacturesExamens",
                columns: table => new
                {
                    ExamensExamenId = table.Column<int>(type: "int", nullable: false),
                    FacturesFactureId = table.Column<int>(type: "int", nullable: false),
                    Qte = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturesExamens", x => new { x.ExamensExamenId, x.FacturesFactureId });
                    table.ForeignKey(
                        name: "FK_FacturesExamens_Examens_ExamensExamenId",
                        column: x => x.ExamensExamenId,
                        principalTable: "Examens",
                        principalColumn: "ExamenId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturesExamens_Factures_FacturesFactureId",
                        column: x => x.FacturesFactureId,
                        principalTable: "Factures",
                        principalColumn: "FactureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assurances_Compagny",
                table: "Assurances",
                column: "Compagny",
                unique: true,
                filter: "[Compagny] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirs_FactureId",
                table: "FactureAvoirs",
                column: "FactureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FactureAvoirs_Reference",
                table: "FactureAvoirs",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FactureConsultation_FactureId",
                table: "FactureConsultation",
                column: "FactureId");

            migrationBuilder.CreateIndex(
                name: "IX_FactureConsultation_MedecinId",
                table: "FactureConsultation",
                column: "MedecinId");

            migrationBuilder.CreateIndex(
                name: "IX_Factures_PatientId",
                table: "Factures",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Factures_Reference",
                table: "Factures",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Factures_UserId",
                table: "Factures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturesExamens_FacturesFactureId",
                table: "FacturesExamens",
                column: "FacturesFactureId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AssuranceId",
                table: "Patients",
                column: "AssuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AssuranceNumber",
                table: "Patients",
                column: "AssuranceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Role_name",
                table: "Roles",
                column: "Role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Account_name",
                table: "Users",
                column: "Account_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersRoles_UserId",
                table: "UsersRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FactureAvoirs");

            migrationBuilder.DropTable(
                name: "FactureConsultation");

            migrationBuilder.DropTable(
                name: "FacturesExamens");

            migrationBuilder.DropTable(
                name: "GreetingMessages");

            migrationBuilder.DropTable(
                name: "UsersRoles");

            migrationBuilder.DropTable(
                name: "Consultations");

            migrationBuilder.DropTable(
                name: "Medecins");

            migrationBuilder.DropTable(
                name: "Examens");

            migrationBuilder.DropTable(
                name: "Factures");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Assurances");
        }
    }
}
