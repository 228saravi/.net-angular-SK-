using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Changes_2018_09_15_001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Segments",
                keyColumn: "Id",
                keyValue: "Middle",
                column: "Name",
                value: "Стандарт");

            migrationBuilder.InsertData(
                table: "Segments",
                columns: new[] { "Id", "Name", "Rank" },
                values: new object[] { "Premium", "Премиум", 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Segments",
                keyColumn: "Id",
                keyValue: "Premium");

            migrationBuilder.UpdateData(
                table: "Segments",
                keyColumn: "Id",
                keyValue: "Middle",
                column: "Name",
                value: "Средний");
        }
    }
}
