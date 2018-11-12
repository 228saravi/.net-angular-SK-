using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class SkillsForWaiters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "GroupName", "Name", "Rank", "SpecialityId" },
                values: new object[,]
                {
                    { "Waiter_RussianCuisine", "Кухня", "Русская", 2000, "Waiter" },
                    { "Waiter_Aperitifs", "Напитки", "Апперитивы", 2000, "Waiter" },
                    { "Waiter_Tinctures", "Напитки", "Настойки", 2000, "Waiter" },
                    { "Waiter_StrongAlcohol", "Напитки", "Крепкий алкоголь", 2000, "Waiter" },
                    { "Waiter_Wine", "Напитки", "Вино", 2000, "Waiter" },
                    { "Waiter_Bear", "Напитки", "Пиво", 2000, "Waiter" },
                    { "Waiter_SoftCocktails", "Напитки", "Безалкогольные коктейли/смузи", 2000, "Waiter" },
                    { "Waiter_SoftDrinks", "Напитки", "Безалкогольные напитки", 2000, "Waiter" },
                    { "Waiter_Coffee", "Напитки", "Кофе", 2000, "Waiter" },
                    { "Waiter_Tea", "Напитки", "Чай", 2000, "Waiter" },
                    { "Waiter_Сocktail", "Напитки", "Миксология/коктейли", 2000, "Waiter" },
                    { "Waiter_NewWorldCuisine", "Кухня", "Нового света", 2000, "Waiter" },
                    { "Waiter_UsaCuisine", "Кухня", "Американская", 2000, "Waiter" },
                    { "Waiter_ChinaCuisine", "Кухня", "Китайская", 2000, "Waiter" },
                    { "Waiter_GermanCuisine", "Кухня", "Немецкая", 2000, "Waiter" },
                    { "Waiter_BelgiumCuisine", "Кухня", "Бельгийская", 2000, "Waiter" },
                    { "Waiter_SpainCuisine", "Кухня", "Испанская", 2000, "Waiter" },
                    { "Waiter_EuropeCuisine", "Кухня", "Европейская", 2000, "Waiter" },
                    { "Waiter_FranceCuisine", "Кухня", "Французская", 2000, "Waiter" },
                    { "Waiter_UsbekCuisine", "Кухня", "Узбекская", 2000, "Waiter" },
                    { "Waiter_Digestives", "Напитки", "Дижестивы", 2000, "Waiter" },
                    { "Waiter_Sommelier", "Напитки", "Навыки сомелье", 2000, "Waiter" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_Aperitifs");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_Bear");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_BelgiumCuisine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_ChinaCuisine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_Coffee");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_Digestives");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_EuropeCuisine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_FranceCuisine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_GermanCuisine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_NewWorldCuisine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_RussianCuisine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_SoftCocktails");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_SoftDrinks");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_Sommelier");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_SpainCuisine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_StrongAlcohol");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_Tea");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_Tinctures");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_UsaCuisine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_UsbekCuisine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_Wine");

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: "Waiter_Сocktail");
        }
    }
}
