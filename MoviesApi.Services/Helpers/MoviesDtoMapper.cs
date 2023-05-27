using MoviesApi.Data.Models;
using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;

namespace MoviesApi.Services.Helpers;

public static class MoviesDtoMapper
{
    private const string ImageUrl = "https://image.tmdb.org/t/p/w500";
    
    public static MovieListDto ToMovieDto(this IEnumerable<FavoriteMovie> favoriteMovies, IReadOnlySet<int> favoriteIds)
    {
        return new MovieListDto
        {
            Results = favoriteMovies.Select(fm => new MovieDto
            {
                MovieId = fm.FavoriteMovieId,
                Title = fm.Title,
                Description = fm.Overview,
                ReleaseDate = fm.ReleaseDate,
                PosterPath = fm.ImageUrl,
                BackdropPath = fm.BackdropPath,
                IsFavorite = favoriteIds.Contains(fm.FavoriteMovieId)
            }).ToList()
        };
    }
    
    public static MovieListDto ToMovieDto(this MoviesResponseTmdb movies, IReadOnlySet<int> movieIds)
    {
        return new MovieListDto
        {
            Results = movies.Results.Select(r => new MovieDto
            {
                MovieId = r.Id,
                Title = r.Title,
                Description = r.Overview,
                ReleaseDate = r.Release_date,
                PosterPath = !string.IsNullOrEmpty(r.Poster_path)
                    ? $"{ImageUrl}{r.Poster_path}"
                    : null,
                IsFavorite = movieIds.Contains(r.Id),
                BackdropPath = !string.IsNullOrEmpty(r.Backdrop_path)
                    ? $"{ImageUrl}{r.Backdrop_path}"
                    : null,
            }).ToList()
        };
    }

    public static MovieResponseDto ToMovieResponseDto(this MovieResponse movie, IReadOnlySet<int> favoriteIds)
    {
        return new MovieResponseDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Overview = movie.Overview,
            Popularity = movie.Popularity,
            Poster_path = !string.IsNullOrEmpty(movie.Poster_path)
                ? $"{ImageUrl}{movie.Poster_path}"
                : null,
            Release_date = movie.Release_date,
            Vote_average = movie.Vote_average,
            Vote_count = movie.Vote_count,
            IsFavorite = favoriteIds.Contains(movie.Id),
            Genres = movie.Genres.Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name
            }).ToList(),
            Budget = movie.Budget,
            Revenue = movie.Revenue,
            Original_language = movie.Original_language,
            Runtime = movie.Runtime,
            Tagline = movie.Tagline,
            Status = movie.Status,
            Backdrop_path = !string.IsNullOrEmpty(movie.Backdrop_path)
                ? $"{ImageUrl}{movie.Backdrop_path}"
                : null,
        };
    }

    public static MovieCreditsResponseDto ToMovieCreditsDto(this MovieCreditsResponseTmdb credits)
    {
        return new MovieCreditsResponseDto
        {
            Id = credits.Id,
            Cast = credits.Cast.Select(c => new MovieCastDto()
            {
                Adult = c.Adult,
                Cast_id = c.Cast_id,
                Character = c.Character,
                Credit_id = c.Credit_id,
                Gender = c.Gender,
                Id = c.Id,
                Known_for_department = c.Known_for_department,
                Name = c.Name,
                Order = c.Order,
                Original_name = c.Original_name,
                Popularity = c.Popularity,
                Profile_path = !string.IsNullOrEmpty(c.Profile_path)
                    ? $"{ImageUrl}{c.Profile_path}"
                    : null
            }).ToList(),
            Crew = credits.Crew.Select(c => new MovieCrewDto
            {
                Adult = c.Adult,
                Credit_id = c.Credit_id,
                Gender = c.Gender,
                Id = c.Id,
                Job = c.Job,
                Known_for_department = c.Known_for_department,
                Name = c.Name,
                Original_name = c.Original_name,
                Popularity = c.Popularity,
                Profile_path = !string.IsNullOrEmpty(c.Profile_path)
                    ? $"{ImageUrl}{c.Profile_path}"
                    : null
            }).ToList()
        };
    }
}