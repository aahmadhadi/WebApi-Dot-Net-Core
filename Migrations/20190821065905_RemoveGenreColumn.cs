using Microsoft.EntityFrameworkCore.Migrations;

namespace BasicWebApi.Migrations
{
    public partial class RemoveGenreColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Books");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Books",
                nullable: true);
        }
    }
}
