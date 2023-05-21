
namespace MoviesApi.Data.Models;

public class RatedMovie
{
    public string RatedMovieId { get; set; } = null!;
    public int UserId { get; set; }
    public double Rating { get; set; }
}