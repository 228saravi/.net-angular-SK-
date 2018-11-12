using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Changes_2018_08_21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Companies_UserId",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "WorkingHours",
                table: "Vacancies",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Vacancies",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<int>(
                name: "RatePerHour",
                table: "Vacancies",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ExperienceInYears",
                table: "Vacancies",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Vacancies",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Vacancies",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Events",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<int>(
                name: "EstimatedGuestsCount",
                table: "Events",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "EstimatedAverageCheck",
                table: "Events",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Events",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Events",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "FeedbackForCompanyId",
                table: "Connections",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FeedbackForExpertId",
                table: "Connections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboutCompanyHtml",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Companies",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FeedbackForCompany",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Rating = table.Column<int>(nullable: false),
                    CommentHtml = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackForCompany", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackForExpert",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Rating = table.Column<int>(nullable: false),
                    CommentHtml = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackForExpert", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_FeedbackForCompanyId",
                table: "Connections",
                column: "FeedbackForCompanyId",
                unique: true,
                filter: "[FeedbackForCompanyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_FeedbackForExpertId",
                table: "Connections",
                column: "FeedbackForExpertId",
                unique: true,
                filter: "[FeedbackForExpertId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CityId",
                table: "Companies",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserId",
                table: "Companies",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Cities_CityId",
                table: "Companies",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_FeedbackForCompany_FeedbackForCompanyId",
                table: "Connections",
                column: "FeedbackForCompanyId",
                principalTable: "FeedbackForCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_FeedbackForExpert_FeedbackForExpertId",
                table: "Connections",
                column: "FeedbackForExpertId",
                principalTable: "FeedbackForExpert",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Cities_CityId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Connections_FeedbackForCompany_FeedbackForCompanyId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Connections_FeedbackForExpert_FeedbackForExpertId",
                table: "Connections");

            migrationBuilder.DropTable(
                name: "FeedbackForCompany");

            migrationBuilder.DropTable(
                name: "FeedbackForExpert");

            migrationBuilder.DropIndex(
                name: "IX_Connections_FeedbackForCompanyId",
                table: "Connections");

            migrationBuilder.DropIndex(
                name: "IX_Connections_FeedbackForExpertId",
                table: "Connections");

            migrationBuilder.DropIndex(
                name: "IX_Companies_CityId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_UserId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "FeedbackForCompanyId",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "FeedbackForExpertId",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "AboutCompanyHtml",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "WorkingHours",
                table: "Vacancies",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Vacancies",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RatePerHour",
                table: "Vacancies",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExperienceInYears",
                table: "Vacancies",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Vacancies",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Events",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EstimatedGuestsCount",
                table: "Events",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EstimatedAverageCheck",
                table: "Events",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Events",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserId",
                table: "Companies",
                column: "UserId");
        }
    }
}
