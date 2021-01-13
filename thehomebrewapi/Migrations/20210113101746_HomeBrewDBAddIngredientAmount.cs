using Microsoft.EntityFrameworkCore.Migrations;

namespace thehomebrewapi.Migrations
{
    public partial class HomeBrewDBAddIngredientAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "Ingredients",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Ingredients");
        }
    }
}
