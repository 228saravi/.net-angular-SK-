using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Changes_2018_09_11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EventFormats",
                columns: new[] { "Id", "Name", "Rank" },
                values: new object[] { "MainCourses", "Основные курсы", 0 });

            migrationBuilder.InsertData(
                table: "EventFormats",
                columns: new[] { "Id", "Name", "Rank" },
                values: new object[] { "Replacement", "Подмена в заведение", 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventFormats",
                keyColumn: "Id",
                keyValue: "MainCourses");

            migrationBuilder.DeleteData(
                table: "EventFormats",
                keyColumn: "Id",
                keyValue: "Replacement");
        }
    }
}
