using MoviesApi.Data.Models;
using MoviesApi.Services.Helpers;
using tmdb_api;

namespace MoviesApi.Tests.UnitTests;

[TestFixture]
public class RatingCalculatorTest : TestFixture
{
    [TestCase(0, 0, 10, 10)]
    [TestCase(1, 5, 10, 7.5)]
    [TestCase(9, 7, 5, 6.8)]
    [TestCase(1, 10, 0, 5)]
    public void CalculateExistingMovieRating(int previousVotes, double previousRating, double userRating, double expectedRating)
    {
        var rating = A<Rating>();
        rating.Votes = previousVotes;
        rating.RatingValue = previousRating;
        
        var ratedMovie = A<RatedMovie>();
        ratedMovie.Rating = userRating;

        var newRating = rating.CalculateExistingMovieRating(ratedMovie);
        Assert.That(newRating.RatingValue, Is.EqualTo(expectedRating));
    }
    
    [TestCase(0, 0, 10, 10)]
    [TestCase(1, 5, 10, 7.5)]
    [TestCase(9, 7, 5, 6.8)]
    [TestCase(1, 10, 0, 5)]
    public void CalculateNewMovieRating(int previousVotes, double previousRating, double userRating, double expectedRating)
    {
       var movieResponse = A<MovieResponse>();
       movieResponse.Vote_count = previousVotes;
       movieResponse.Vote_average = previousRating;
       var ratedMovie = A<RatedMovie>();
       ratedMovie.Rating = userRating;
       
       var newRating = movieResponse.CalculateNewMovieRating(ratedMovie);
        
       Assert.That(newRating.RatingValue, Is.EqualTo(expectedRating));
    }
    
    [TestCase(1, 10, 10, 5, 5)]
    [TestCase(2, 10, 10, 5, 7.5)]
    [TestCase(5, 10, 10, 5, 9)]
    public void CalculateUpdatedMovieRating(int previousVotes, double previousRating, double oldUserRating,double userRating, double expectedRating)
    {
        var oldRating = A<RatedMovie>();
        oldRating.Rating = oldUserRating;
        var newRating = A<RatedMovie>();
        newRating.Rating = userRating;
        var rating = A<Rating>();
        rating.RatingValue = previousRating;
        rating.Votes = previousVotes;
        
        var result = oldRating.CalculateUpdatedMovieRating(newRating, rating);
        
        Assert.That(result.RatingValue, Is.EqualTo(expectedRating));
    }
}