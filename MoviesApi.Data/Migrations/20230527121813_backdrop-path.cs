using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesApi.Data.Migrations
{
    public partial class backdroppath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackdropPath",
                table: "FavoriteMovies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackdropPath",
                table: "FavoriteMovies");
        }
    }
}
