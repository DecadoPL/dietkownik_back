using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    public partial class ChangedTagEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_DietRequirements_ItemId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Diets_ItemId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Dishes_ItemId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Ingredients_ItemId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_TagNames_NameId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_TagTables_TableId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "TagTables");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ItemId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_NameId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TableId",
                table: "Tags");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_DietRequirements_ItemId",
                table: "Tags",
                column: "ItemId",
                principalTable: "DietRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Diets_ItemId",
                table: "Tags",
                column: "ItemId",
                principalTable: "Diets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Dishes_ItemId",
                table: "Tags",
                column: "ItemId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Ingredients_ItemId",
                table: "Tags",
                column: "ItemId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_TagNames_NameId",
                table: "Tags",
                column: "NameId",
                principalTable: "TagNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_TagTables_TableId",
                table: "Tags",
                column: "TableId",
                principalTable: "TagTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
