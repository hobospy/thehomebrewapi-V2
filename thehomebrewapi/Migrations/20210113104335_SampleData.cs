using Microsoft.EntityFrameworkCore.Migrations;

namespace thehomebrewapi.Migrations
{
    public partial class SampleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Single malt and single hop recipe using Amarillo hops", "Amarillo SMaSH" });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, "Porter based recipe with raspberries added during the second half of fermentation", "Raspberry Porter" });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 3, "Basic Bock recipe for those cold nights", "Bock" });

            migrationBuilder.InsertData(
                table: "Ingredients",
                columns: new[] { "Id", "Amount", "Name", "RecipeId" },
                values: new object[,]
                {
                    { 1, 68.0, "Amarillo", 1 },
                    { 2, 5.5, "Pale malt", 1 },
                    { 3, 150.0, "Light crystal malt", 1 },
                    { 4, 300.0, "Dark roast malt", 2 },
                    { 5, 60.0, "Bittering", 2 },
                    { 6, 120.0, "Dark crystal", 3 },
                    { 7, 300.0, "Rice husks", 3 },
                    { 8, 0.5, "Whirlfloc", 3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
