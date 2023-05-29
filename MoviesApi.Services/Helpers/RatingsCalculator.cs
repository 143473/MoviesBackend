using MoviesApi.Data.Models;
using tmdb_api;

namespace MoviesApi.Services.Helpers;

public static class RatingsCalculator
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
        var newRatingValue = newRating.Rating;
        
        if (oldVotes != 0)
        {
            var oldRatingValue = ((rating.RatingValue * rating.Votes) - oldRating.Rating) / oldVotes;
            newRatingValue = ((oldRatingValue * oldVotes) + newRating.Rating) / rating.Votes;
        }

        rating = new Rating
        {
            MovieId = oldRating.RatedMovieId,
            RatingValue = newRatingValue,
            Votes = rating.Votes
        };
        return rating;
    }
}