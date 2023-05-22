﻿using MoviesApi.Data.Models;

namespace MoviesApi.Data.Repositories.Interfaces;

public interface IMoviesRepository
{
    Task<ICollection<FavoriteMovie>> GetFavorites(string userId);
    Task<IReadOnlySet<int>> GetFavorites(string userId, IEnumerable<int> movieIds);
    Task<ICollection<FavoriteMovie>> GetTopFavorites();
    Task<FavoriteMovie?> GetFavorite(int movieId);
    Task AddFavorite(string userId, FavoriteMovie favoriteMovie);
    Task<Rating> GetMovieRatingAsync(int movieId);
    Task AddRatedMovieAsync(RatedMovie movie);
    Task<RatedMovie> GetMovieRatingByUserId(string userId, int movieId);
    Task<Rating> AddRatingAsync(Rating rating);
    Task UpdateRatingAsync(Rating rating, RatedMovie ratedMovie);
    Task UpdateOnlyRatingAsync(Rating rating);
    
}