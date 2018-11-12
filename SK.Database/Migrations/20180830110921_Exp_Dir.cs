using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Exp_Dir : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceInYears",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "ExperienceInYears",
                table: "ExpertProfiles");

            migrationBuilder.AddColumn<string>(
                name: "ExperienceOptionId",
                table: "Vacancies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExperienceOptionId",
                table: "ExpertProfiles",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExperienceOptions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceOptions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ExperienceOptions",
                columns: new[] { "Id", "Name", "Rank" },
                values: new object[] { "Month6", "до 6 мес.", 0 });

            migrationBuilder.InsertData(
                table: "ExperienceOptions",
                columns: new[] { "Id", "Name", "Rank" },
                values: new object[] { "Month6_Year2", "от 6 мес. до 2 лет", 0 });

            migrationBuilder.InsertData(
                table: "ExperienceOptions",
                columns: new[] { "Id", "Name", "Rank" },
                values: new object[] { "Year2", "от 2 лет", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_ExperienceOptionId",
                table: "Vacancies",
                column: "ExperienceOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfiles_ExperienceOptionId",
                table: "ExpertProfiles",
                column: "ExperienceOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpertProfiles_ExperienceOptions_ExperienceOptionId",
                table: "ExpertProfiles",
                column: "ExperienceOptionId",
                principalTable: "ExperienceOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_ExperienceOptions_ExperienceOptionId",
                table: "Vacancies",
                column: "ExperienceOptionId",
                principalTable: "ExperienceOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpertProfiles_ExperienceOptions_ExperienceOptionId",
                table: "ExpertProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_ExperienceOptions_ExperienceOptionId",
                table: "Vacancies");

            migrationBuilder.DropTable(
                name: "ExperienceOptions");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_ExperienceOptionId",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_ExpertProfiles_ExperienceOptionId",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "ExperienceOptionId",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "ExperienceOptionId",
                table: "ExpertProfiles");

            migrationBuilder.AddColumn<int>(
                name: "ExperienceInYears",
                table: "Vacancies",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExperienceInYears",
                table: "ExpertProfiles",
                nullable: true);
        }
    }
}
