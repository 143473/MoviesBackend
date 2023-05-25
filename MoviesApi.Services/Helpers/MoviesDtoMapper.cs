using System.Net.Http.Headers;
using MoviesApi.Data.Models;
using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;

namespace MoviesApi.Services.Helpers;

public static class MoviesDtoMapper
{
    public static MovieListDto ToMovieDto(this IEnumerable<FavoriteMovie> favoriteMovies, IReadOnlySet<int> favoriteIds)
    {
        return new MovieListDto()
        {
            Results = favoriteMovies.Select(fm => new MovieDto
            {
                MovieId = fm.FavoriteMovieId,
                Title = fm.Title,
                Description = fm.Overview,
                ReleaseDate = fm.ReleaseDate,
                PosterPath = fm.ImageUrl,
                IsFavorite = favoriteIds.Contains(fm.FavoriteMovieId)
            }).ToList()
        };
    }

    public static MovieListDto ToMovieDto(this MoviesResponseTmdb movies, IReadOnlySet<int> movieIds)
    {
        return new MovieListDto()
        {
            Results = movies.Results.Select(r => new MovieDto
            {
                MovieId = r.Id,
                Title = r.Title,
                Description = r.Overview,
                ReleaseDate = r.Release_date,
                PosterPath = !string.IsNullOrEmpty(r.Poster_path)
                    ? $"https://image.tmdb.org/t/p/w500{r.Poster_path}"
                    : null,
                IsFavorite = movieIds.Contains(r.Id)
            }).ToList()
        };
    }

    public static MovieResponseDto ToMovieResponseDto(this MovieResponse movie, IReadOnlySet<int> favoriteIds)
    {
        return new MovieResponseDto()
        {
            Id = movie.Id,
            Title = movie.Title,
            Overview = movie.Overview,
            Popularity = movie.Popularity,
            Poster_path = !string.IsNullOrEmpty(movie.Poster_path)
                ? $"https://image.tmdb.org/t/p/w500{movie.Poster_path}"
                : null,
            Release_date = movie.Release_date,
            Vote_average = movie.Vote_average,
            Vote_count = movie.Vote_count,
            IsFavorite = favoriteIds.Contains(movie.Id),
            Genres = movie.Genres.Select(g => new GenreDto()
            {
                Id = g.Id,
                Name = g.Name
            }).ToList(),
            Budget = movie.Budget,
            Revenue = movie.Revenue,
            Original_language = movie.Original_language,
            Runtime = movie.Runtime,
            Tagline = movie.Tagline
        };
    }
}