using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DietDishes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DietDayId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<string>(type: "TEXT", nullable: true),
                    DishTime = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietDishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DietRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Kcal = table.Column<string>(type: "TEXT", nullable: true),
                    Proteins = table.Column<string>(type: "TEXT", nullable: true),
                    Carbohydrates = table.Column<string>(type: "TEXT", nullable: true),
                    Fat = table.Column<string>(type: "TEXT", nullable: true),
                    Fibers = table.Column<string>(type: "TEXT", nullable: true),
                    Cholesterol = table.Column<string>(type: "TEXT", nullable: true),
                    Potassium = table.Column<string>(type: "TEXT", nullable: true),
                    Sodium = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminA = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminC = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminB6 = table.Column<string>(type: "TEXT", nullable: true),
                    Magnesium = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminD = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminB12 = table.Column<string>(type: "TEXT", nullable: true),
                    Calcium = table.Column<string>(type: "TEXT", nullable: true),
                    Iron = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietRequirements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DietRequirementsId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Portions = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Recipe = table.Column<string>(type: "TEXT", nullable: true),
                    CookingTime = table.Column<string>(type: "TEXT", nullable: true),
                    Kcal = table.Column<string>(type: "TEXT", nullable: true),
                    Proteins = table.Column<string>(type: "TEXT", nullable: true),
                    Carbohydrates = table.Column<string>(type: "TEXT", nullable: true),
                    Fat = table.Column<string>(type: "TEXT", nullable: true),
                    Fibers = table.Column<string>(type: "TEXT", nullable: true),
                    Cholesterol = table.Column<string>(type: "TEXT", nullable: true),
                    Potassium = table.Column<string>(type: "TEXT", nullable: true),
                    Sodium = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminA = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminC = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminB6 = table.Column<string>(type: "TEXT", nullable: true),
                    Magnesium = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminD = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminB12 = table.Column<string>(type: "TEXT", nullable: true),
                    Calcium = table.Column<string>(type: "TEXT", nullable: true),
                    Iron = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IngredientId = table.Column<int>(type: "INTEGER", nullable: false),
                    DishId = table.Column<int>(type: "INTEGER", nullable: false),
                    PortionTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PortionQuantity = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishIngredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PortionTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Brand = table.Column<string>(type: "TEXT", nullable: true),
                    EAN = table.Column<string>(type: "TEXT", nullable: true),
                    PortionQuantity = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Kcal = table.Column<string>(type: "TEXT", nullable: true),
                    Proteins = table.Column<string>(type: "TEXT", nullable: true),
                    Carbohydrates = table.Column<string>(type: "TEXT", nullable: true),
                    Fat = table.Column<string>(type: "TEXT", nullable: true),
                    Fibers = table.Column<string>(type: "TEXT", nullable: true),
                    Cholesterol = table.Column<string>(type: "TEXT", nullable: true),
                    Potassium = table.Column<string>(type: "TEXT", nullable: true),
                    Sodium = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminA = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminC = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminB6 = table.Column<string>(type: "TEXT", nullable: true),
                    Magnesium = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminD = table.Column<string>(type: "TEXT", nullable: true),
                    VitaminB12 = table.Column<string>(type: "TEXT", nullable: true),
                    Calcium = table.Column<string>(type: "TEXT", nullable: true),
                    Iron = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProhibitedIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DietRequirementsId = table.Column<int>(type: "INTEGER", nullable: false),
                    IngredientId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProhibitedIngredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequiredIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DietRequirementsId = table.Column<int>(type: "INTEGER", nullable: false),
                    IngredientId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequiredIngredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TagNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TagTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    Surname = table.Column<string>(type: "TEXT", nullable: true),
                    Gender = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "BLOB", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameId = table.Column<int>(type: "INTEGER", nullable: false),
                    TableId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_DietRequirements_ItemId",
                        column: x => x.ItemId,
                        principalTable: "DietRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tags_Diets_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Diets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tags_Dishes_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tags_Ingredients_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tags_TagNames_NameId",
                        column: x => x.NameId,
                        principalTable: "TagNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tags_TagTables_TableId",
                        column: x => x.TableId,
                        principalTable: "TagTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ItemId",
                table: "Tags",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NameId",
                table: "Tags",
                column: "NameId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TableId",
                table: "Tags",
                column: "TableId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropTable(
                name: "DietDishes");

            migrationBuilder.DropTable(
                name: "DishIngredients");

            migrationBuilder.DropTable(
                name: "PortionTypes");

            migrationBuilder.DropTable(
                name: "ProhibitedIngredients");

            migrationBuilder.DropTable(
                name: "RequiredIngredients");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DietRequirements");

            migrationBuilder.DropTable(
                name: "Diets");

            migrationBuilder.DropTable(
                name: "Dishes");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "TagNames");

            migrationBuilder.DropTable(
                name: "TagTables");
        }
    }
}
