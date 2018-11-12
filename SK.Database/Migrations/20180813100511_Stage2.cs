using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Stage2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Events",
                newName: "LogoImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "AboutVacancyHtml",
                table: "Vacancies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboutEventHtml",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WithAccomodation",
                table: "Events",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WithtDelivery",
                table: "Events",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "VacancyDocument",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VacancyId = table.Column<long>(nullable: false),
                    ExpertDocumentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancyDocument_ExpertDocuments_ExpertDocumentId",
                        column: x => x.ExpertDocumentId,
                        principalTable: "ExpertDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacancyDocument_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VacancyLanguage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VacancyId = table.Column<long>(nullable: false),
                    LanguageId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancyLanguage_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacancyLanguage_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VacancyDocument_ExpertDocumentId",
                table: "VacancyDocument",
                column: "ExpertDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyDocument_VacancyId",
                table: "VacancyDocument",
                column: "VacancyId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyLanguage_LanguageId",
                table: "VacancyLanguage",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyLanguage_VacancyId",
                table: "VacancyLanguage",
                column: "VacancyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacancyDocument");

            migrationBuilder.DropTable(
                name: "VacancyLanguage");

            migrationBuilder.DropColumn(
                name: "AboutVacancyHtml",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "AboutEventHtml",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "WithAccomodation",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "WithtDelivery",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "LogoImageUrl",
                table: "Events",
                newName: "Description");
        }
    }
}
