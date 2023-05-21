﻿using Microsoft.EntityFrameworkCore;
using MoviesApi.Data.Models;
using MoviesApi.Data.Repositories.Interfaces;

namespace MoviesApi.Data.Repositories;

public class MoviesRepository : IMoviesRepository
{
    private readonly IMoviesContext _context;

    public MoviesRepository(IMoviesContext context)
    {
        _context = context;
    }

    public async Task<ICollection<FavoriteMovie>> GetFavoritesMovies(int userId)
    {
        return await _context.Favorites
            .Where(x => x.UserId == userId)
            .Select(x => x.FavoriteMovie)
            .ToListAsync();;
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
}