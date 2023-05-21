using Microsoft.EntityFrameworkCore;
using MoviesApi.Data.Models;
using MoviesApi.Data.Repositories.Interfaces;

namespace MoviesApi.Data.Repositories;

public class MoviesRepository : IMoviesRepository
{
    private readonly MoviesContext _context;

    public MoviesRepository(MoviesContext context)
    {
        _context = context;
    }

    public async Task<ICollection<FavoriteMovie>> GetFavoritesMovies(string userId)
    {
        return await _context.Favorites
            .Where(x => x.UserId == userId)
            .Select(x => x.FavoriteMovie)
            .ToListAsync();
    }

    public async Task<ICollection<FavoriteMovie>> GetTopFavoritesMovies()
    {
        var result = await _context.Favorites
            .GroupBy(favorites => favorites.FavoriteMovieId)
            .Select(grouping => new { Movie = grouping.First().FavoriteMovie, Count = grouping.Count() })
            .OrderByDescending(movieCount => movieCount.Count)
            .Take(10)
            .ToListAsync();

        return result.Select(x => x.Movie).ToList();
    }

    public async Task AddFavoriteMovie(string userId, FavoriteMovie favoriteMovie)
    {
        var existingMovie = await _context.FavoriteMovies
            .FirstOrDefaultAsync(fm => fm.FavoriteMovieId == favoriteMovie.FavoriteMovieId);
        
        await _context.Favorites.AddAsync(new Favorites
        {
            UserId = userId,
            FavoriteMovie = existingMovie ?? favoriteMovie
        });
        
        await _context.SaveChangesAsync();
    }

    public async Task<Rating> GetMovieRatingAsync(int movieId)
    {
        Rating rating = new Rating
        {
            MovieId = 0,
            RatingValue = 0,
            Votes = 0
        };
        
        try
        {
            rating = await _context.Ratings.FirstAsync(r => r.MovieId == movieId);
            return rating;
        }
        catch (InvalidOperationException)
        {
            return rating;
        }
        
    }

    public async Task AddRatedMovieAsync(RatedMovie ratedMovie)
    {
        await _context.RatedMovies.AddAsync(ratedMovie);
        await _context.SaveChangesAsync();
    }

    public async Task<RatedMovie> GetMovieRatingByUserId(string userId, int movieId)
    {
        RatedMovie ratedMovie = new RatedMovie()
        {
            RatedMovieId = 0,
            Rating = 0,
            UserId = null
        };
        
        try
        {
            ratedMovie = await _context.RatedMovies.FirstAsync(r => r.RatedMovieId == movieId && r.UserId == userId);
            return ratedMovie;
        }
        catch (InvalidOperationException)
        {
            return ratedMovie;
        }
    }

    public async Task<Rating> AddRatingAsync(Rating rating)
    {
        await _context.Ratings.AddAsync(rating);
        await _context.SaveChangesAsync();
        return rating;
    }

    public async Task UpdateRatingAsync(Rating rating, RatedMovie ratedMovie)
    {
        Rating ratingToUpdate = await _context.Ratings.FirstAsync(r => r.MovieId == rating.MovieId);
        ratingToUpdate.Votes = rating.Votes;
        ratingToUpdate.RatingValue = rating.RatingValue;
        _context.Update(ratingToUpdate);
        await _context.SaveChangesAsync();
        
        RatedMovie movieToUpdate = await _context.RatedMovies.FirstAsync(m => m.RatedMovieId == ratedMovie.RatedMovieId && m.UserId == ratedMovie.UserId);
        movieToUpdate.Rating = ratedMovie.Rating;
        _context.Update(movieToUpdate);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOnlyRatingAsync(Rating rating)
    {
        Rating ratingToUpdate = await _context.Ratings.FirstAsync(r => r.MovieId == rating.MovieId);
        ratingToUpdate.Votes = rating.Votes;
        ratingToUpdate.RatingValue = rating.RatingValue;
        _context.Update(ratingToUpdate);
        await _context.SaveChangesAsync();
    }
}