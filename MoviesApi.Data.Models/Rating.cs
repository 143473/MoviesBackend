using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Data.Models;
public class Rating
{
    [Key]
    public int MovieId { get; set; }
    public double RatingValue { get; set; }
    public long Votes { get; set; }
    
}