using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonPocket.Migrations
{
    public partial class PityID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pity",
                table: "Pity");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Pity",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pity",
                table: "Pity",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pity",
                table: "Pity");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Pity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pity",
                table: "Pity",
                column: "Value");
        }
    }
}
