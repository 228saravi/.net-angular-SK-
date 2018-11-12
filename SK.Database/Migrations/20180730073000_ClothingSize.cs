using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class ClothingSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClothingSizes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClothingSizes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ClothingSizes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "XS", "XS" },
                    { "S", "S" },
                    { "M", "M" },
                    { "L", "L" },
                    { "XL", "XL" },
                    { "XXL", "XXL" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClothingSizes");
        }
    }
}
