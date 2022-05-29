using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonPocket.Migrations
{
    public partial class PokeBallsid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pokeballs",
                table: "Pokeballs");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Pokeballs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pokeballs",
                table: "Pokeballs",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pokeballs",
                table: "Pokeballs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Pokeballs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pokeballs",
                table: "Pokeballs",
                column: "Name");
        }
    }
}
