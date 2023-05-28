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

    public async Task<ICollection<FavoriteMovie>> GetFavorites(string userId)
    {
        return await _context.Favorites
            .Where(x => x.UserId == userId)
            .Select(x => x.FavoriteMovie)
            .ToListAsync();
    }

    public async Task<IReadOnlySet<int>> GetFavorites(string userId, IEnumerable<int> movieIds)
    {
        var favorites = await _context.Favorites
            .Where(x => x.UserId == userId && movieIds.Contains(x.FavoriteMovieId))
            .Select(x => x.FavoriteMovieId)
            .ToListAsync();
        return favorites.ToHashSet();
    }

    public async Task<ICollection<FavoriteMovie>> GetTopFavorites()
    {
        var result = await _context.Favorites
            .GroupBy(favorites => favorites.FavoriteMovieId)
            .Select(grouping => new {Movie = grouping.First().FavoriteMovie, Count = grouping.Count()})
            .OrderByDescending(movieCount => movieCount.Count)
            .Take(10)
            .ToListAsync();

        return result.Select(x => x.Movie).ToList();
    }

    public async Task<FavoriteMovie?> GetFavorite(int movieId)
    {
        return await _context.FavoriteMovies
            .FirstOrDefaultAsync(fm => fm.FavoriteMovieId == movieId);
    }

    public async Task AddFavorite(string userId, FavoriteMovie favoriteMovie)
    {
        await _context.Favorites.AddAsync(new Favorites
        {
            UserId = userId,
            FavoriteMovie = favoriteMovie
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemoveFavorite(string userId, int movieId)
    {
        Favorites toRemove;
        try
        {
            toRemove = await _context.Favorites
                .FirstAsync(x => x.UserId == userId && x.FavoriteMovieId == movieId);
        }
        catch (InvalidOperationException)
        {
            return;
        }

        _context.Remove(toRemove);
        await _context.SaveChangesAsync();
    }

    public async Task<Rating?> GetMovieRatingAsync(int movieId)
    {
        Rating? rating = (await _context.Ratings.FirstOrDefaultAsync(r => r.MovieId == movieId))!;
        return rating;
    }

    public async Task AddRatedMovieAsync(RatedMovie ratedMovie)
    {
        await _context.RatedMovies.AddAsync(ratedMovie);
        await _context.SaveChangesAsync();
    }

    public async Task<RatedMovie?> GetMovieRatingByUserId(string userId, int movieId)
    {
        RatedMovie? ratedMovie = await _context.RatedMovies.FirstOrDefaultAsync(r => r.RatedMovieId == movieId && r.UserId == userId);
        return ratedMovie;
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

        RatedMovie movieToUpdate = await _context.RatedMovies.FirstAsync(m =>
            m.RatedMovieId == ratedMovie.RatedMovieId && m.UserId == ratedMovie.UserId);
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

    public async Task<ICollection<Comment>> GetCommentsAsync(int movieId)
    {
        ICollection<Comment> comments = new List<Comment>();

        try
        {
            comments = await _context.Comments
                .Where(c => c.MovieId == movieId).ToListAsync();
            return comments;
        }
        catch (InvalidOperationException)
        {
            return comments;
        }
    }

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return comment;
    }
}