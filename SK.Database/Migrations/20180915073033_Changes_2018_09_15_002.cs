using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Changes_2018_09_15_002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Segments",
                keyColumn: "Id",
                keyValue: "Middle",
                column: "Rank",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Segments",
                keyColumn: "Id",
                keyValue: "Premium",
                column: "Rank",
                value: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Segments",
                keyColumn: "Id",
                keyValue: "Middle",
                column: "Rank",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Segments",
                keyColumn: "Id",
                keyValue: "Premium",
                column: "Rank",
                value: 0);
        }
    }
}
