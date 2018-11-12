using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Changes_2018_10_12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "Id", "Name", "Rank", "SpecialityId" },
                values: new object[] { "Cook_SushiMaker", "Сушист", 1000, "Cook" });

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "Id", "Name", "Rank", "SpecialityId" },
                values: new object[] { "Cook_PizzaMaker", "Пиццмейкер", 1000, "Cook" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: "Cook_PizzaMaker");

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: "Cook_SushiMaker");
        }
    }
}
