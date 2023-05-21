using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesApi.Data.Migrations
{
    public partial class AddedRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RatedMovie",
                table: "RatedMovie");

            migrationBuilder.RenameTable(
                name: "RatedMovie",
                newName: "RatedMovies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RatedMovies",
                table: "RatedMovies",
                columns: new[] { "RatedMovieId", "UserId" });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RatingValue = table.Column<double>(type: "float", nullable: false),
                    Votes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.MovieId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RatedMovies",
                table: "RatedMovies");

            migrationBuilder.RenameTable(
                name: "RatedMovies",
                newName: "RatedMovie");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RatedMovie",
                table: "RatedMovie",
                columns: new[] { "RatedMovieId", "UserId" });
        }
    }
}
