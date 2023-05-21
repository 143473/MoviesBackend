
namespace MoviesApi.Data.Models;

public class RatedMovie
{
    public long RatedMovieId { get; set; }
    public long UserId { get; set; }
    public double Rating { get; set; }
}