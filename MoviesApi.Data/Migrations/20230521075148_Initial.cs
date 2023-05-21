using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesApi.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteMovies",
                columns: table => new
                {
                    FavoriteMovieId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Overview = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteMovies", x => x.FavoriteMovieId);
                });

            migrationBuilder.CreateTable(
                name: "RatedMovie",
                columns: table => new
                {
                    RatedMovieId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatedMovie", x => new { x.RatedMovieId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    FavoriteMovieId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => new { x.FavoriteMovieId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Favorites_FavoriteMovies_FavoriteMovieId",
                        column: x => x.FavoriteMovieId,
                        principalTable: "FavoriteMovies",
                        principalColumn: "FavoriteMovieId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "RatedMovie");

            migrationBuilder.DropTable(
                name: "FavoriteMovies");
        }
    }
}
