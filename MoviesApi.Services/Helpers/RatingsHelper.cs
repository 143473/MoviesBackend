using MoviesApi.Data.Models;
using tmdb_api;

namespace MoviesApi.Services.Helpers;

public static class RatingsHelper
{
    public static Rating CalculateExistingMovieRating(this Rating rating, RatedMovie ratedMovie)
    {
        var newVotes = (int) (rating.Votes + 1);
        var newRatingValue = ((rating.RatingValue * rating.Votes) + ratedMovie.Rating) / newVotes;
        var newRating = new Rating
        {
            MovieId = ratedMovie.RatedMovieId,
            RatingValue = newRatingValue,
            Votes = newVotes
        };
        return newRating;
    }

    public static Rating CalculateNewMovieRating(this MovieResponse newMovie, RatedMovie ratedMovie)
    {
        var newVotes = newMovie.Vote_count!.Value + 1;

        var newRatingValue = (newMovie.Vote_average!.Value * newMovie.Vote_count.Value +
                              ratedMovie.Rating) / newVotes;
        var rating = new Rating
        {
            MovieId = ratedMovie.RatedMovieId,
            RatingValue = newRatingValue,
            Votes = newVotes
        };

        return rating;
    }
    

    public static Rating CalculateUpdatedMovieRating(this RatedMovie oldRating, RatedMovie newRating, Rating? rating)
    {
        var oldVotes = (int) (rating!.Votes - 1);
        var oldRatingValue = ((rating.RatingValue * rating.Votes) - oldRating.Rating) / oldVotes;
        var newVotes = (int) (rating.Votes + 1);
        var newRatingValue = ((oldRatingValue * rating.Votes) + newRating.Rating) / newVotes;
        rating = new Rating
        {
            MovieId = oldRating.RatedMovieId,
            RatingValue = newRatingValue,
            Votes = rating.Votes
        };
        return rating;
    }
}