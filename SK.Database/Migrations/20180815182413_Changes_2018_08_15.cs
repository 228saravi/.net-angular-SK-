using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Changes_2018_08_15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "ClothingSizes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ClothingSizes",
                keyColumn: "Id",
                keyValue: "L",
                column: "Rank",
                value: 4);

            migrationBuilder.UpdateData(
                table: "ClothingSizes",
                keyColumn: "Id",
                keyValue: "M",
                column: "Rank",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ClothingSizes",
                keyColumn: "Id",
                keyValue: "S",
                column: "Rank",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ClothingSizes",
                keyColumn: "Id",
                keyValue: "XL",
                column: "Rank",
                value: 5);

            migrationBuilder.UpdateData(
                table: "ClothingSizes",
                keyColumn: "Id",
                keyValue: "XS",
                column: "Rank",
                value: 1);

            migrationBuilder.UpdateData(
                table: "ClothingSizes",
                keyColumn: "Id",
                keyValue: "XXL",
                column: "Rank",
                value: 6);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "ClothingSizes");
        }
    }
}
