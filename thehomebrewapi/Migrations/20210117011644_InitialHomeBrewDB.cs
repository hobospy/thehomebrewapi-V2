using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace thehomebrewapi.Migrations
{
    public partial class InitialHomeBrewDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaterProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Type = table.Column<short>(nullable: false),
                    ExpectedABV = table.Column<double>(nullable: false),
                    Favourite = table.Column<bool>(nullable: false),
                    WaterProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_WaterProfiles_WaterProfileId",
                        column: x => x.WaterProfileId,
                        principalTable: "WaterProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaterProfileAdditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    WaterProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterProfileAdditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterProfileAdditions_WaterProfiles_WaterProfileId",
                        column: x => x.WaterProfileId,
                        principalTable: "WaterProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Brews",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    BrewDate = table.Column<DateTime>(nullable: false),
                    BrewedState = table.Column<int>(nullable: false),
                    BrewingNotes = table.Column<string>(maxLength: 2000, nullable: true),
                    ABV = table.Column<double>(nullable: false),
                    Rating = table.Column<double>(nullable: false),
                    RecipeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brews", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Brews_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeSteps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 500, nullable: false),
                    RecipeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeSteps_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TastingNote",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(maxLength: 1000, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    BrewID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TastingNote", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TastingNote_Brews_BrewID",
                        column: x => x.BrewID,
                        principalTable: "Brews",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    RecipeStepId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredients_RecipeSteps_RecipeStepId",
                        column: x => x.RecipeStepId,
                        principalTable: "RecipeSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Timers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Duration = table.Column<long>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    RecipeStepId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Timers_RecipeSteps_RecipeStepId",
                        column: x => x.RecipeStepId,
                        principalTable: "RecipeSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "WaterProfiles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Light beer water profile", "Pilsner" });

            migrationBuilder.InsertData(
                table: "WaterProfiles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, "Dark beer water profile", "Stout" });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "Description", "ExpectedABV", "Favourite", "Name", "Type", "WaterProfileId" },
                values: new object[,]
                {
                    { 1, "Single malt and single hop recipe using Amarillo hops", 3.5, true, "Amarillo SMaSH", (short)6, 1 },
                    { 2, "Porter based recipe with raspberries added during the second half of fermentation", 5.2000000000000002, false, "Raspberry Porter", (short)9, 2 },
                    { 3, "Basic Bock recipe for those cold nights", 6.7999999999999998, false, "Bock", (short)15, 2 }
                });

            migrationBuilder.InsertData(
                table: "WaterProfileAdditions",
                columns: new[] { "Id", "Amount", "Name", "Unit", "WaterProfileId" },
                values: new object[] { 1, 300.0, "Light crystal malt", 1, 1 });

            migrationBuilder.InsertData(
                table: "Brews",
                columns: new[] { "ID", "ABV", "BrewDate", "BrewedState", "BrewingNotes", "Name", "Rating", "RecipeId" },
                values: new object[,]
                {
                    { 1, 4.2999999999999998, new DateTime(2020, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "The yeast in this one didn't settle", "First Brew", 3.0, 1 },
                    { 2, 4.0999999999999996, new DateTime(2021, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, "Gonna be better brew", 0.0, 1 }
                });

            migrationBuilder.InsertData(
                table: "RecipeSteps",
                columns: new[] { "Id", "Description", "RecipeId" },
                values: new object[,]
                {
                    { 1, "Get the water up to mash in temp", 1 },
                    { 2, "Mash in the grains", 1 },
                    { 3, "Mash out", 1 },
                    { 4, "Raise temp to a rolling boil", 1 },
                    { 5, "Add bittering hops", 1 }
                });

            migrationBuilder.InsertData(
                table: "Ingredients",
                columns: new[] { "Id", "Amount", "Name", "RecipeStepId" },
                values: new object[,]
                {
                    { 1, 68.0, "Amarillo", 2 },
                    { 2, 5.5, "Pale malt", 2 },
                    { 3, 150.0, "Light crystal malt", 2 }
                });

            migrationBuilder.InsertData(
                table: "Timers",
                columns: new[] { "Id", "Duration", "RecipeStepId", "Type" },
                values: new object[,]
                {
                    { 1, 3600L, 2, 1 },
                    { 2, 600L, 5, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brews_RecipeId",
                table: "Brews",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeStepId",
                table: "Ingredients",
                column: "RecipeStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_WaterProfileId",
                table: "Recipes",
                column: "WaterProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSteps_RecipeId",
                table: "RecipeSteps",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_TastingNote_BrewID",
                table: "TastingNote",
                column: "BrewID");

            migrationBuilder.CreateIndex(
                name: "IX_Timers_RecipeStepId",
                table: "Timers",
                column: "RecipeStepId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaterProfileAdditions_WaterProfileId",
                table: "WaterProfileAdditions",
                column: "WaterProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "TastingNote");

            migrationBuilder.DropTable(
                name: "Timers");

            migrationBuilder.DropTable(
                name: "WaterProfileAdditions");

            migrationBuilder.DropTable(
                name: "Brews");

            migrationBuilder.DropTable(
                name: "RecipeSteps");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "WaterProfiles");
        }
    }
}
