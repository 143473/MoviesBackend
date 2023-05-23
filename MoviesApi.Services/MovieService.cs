using Microsoft.Extensions.Configuration;
using MoviesApi.Data.Models;
using MoviesApi.Data.Repositories.Interfaces;
using MoviesApi.Services.Interfaces;
using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;
using MovieResponse = tmdb_api.MovieResponse;

namespace MoviesApi.Services;

public class MovieService : IMovieService
{
    private readonly IMoviesClient _moviesClient;
    private string api_key;
    private readonly IMoviesRepository _repository;

    public MovieService(IMoviesClient moviesClient, IConfiguration configuration, IMoviesRepository repository)
    {
        _moviesClient = moviesClient;
        _repository = repository;
        api_key = configuration.GetSection("TMDB")["APIKey"];
    }

    public async Task<MovieResponse> GetMovieAsync(int movie_id, string language = "en_US")
    {
        var movieResponse = await _moviesClient.GetMovieAsync(movie_id, api_key, language);
        movieResponse.Poster_path = !string.IsNullOrEmpty(movieResponse.Poster_path)
            ? $"https://image.tmdb.org/t/p/w500{movieResponse.Poster_path}"
            : null;
        return movieResponse;
    }

    public async Task<MoviesResponseDto> GetMoviesByTitleAsync(string? userId, string movieName,
        string language = "en_US")
    {
        var moviesResponse = await _moviesClient.GetMoviesByTitleAsync(api_key,movieName, null, language);
        var movieIds = moviesResponse.Results.Select(r => r.Id);
        var favoriteMovieIds = await GetFavorites(userId, movieIds);
        
        return new MoviesResponseDto
        {
            Results = moviesResponse.Results
                .Select(r => new MovieDto
                {
                    MovieId = r.Id,
                    Title = r.Title,
                    Description = r.Overview,
                    ReleaseDate = r.Release_date,
                    PosterPath = !string.IsNullOrEmpty(r.Poster_path)
                        ? $"https://image.tmdb.org/t/p/w500{r.Poster_path}"
                        : null,
                    IsFavorite = favoriteMovieIds.Contains(r.Id)
                })
                .ToList()
        };
    }
    
    public async Task<MoviesResponseDto> GetFavoriteMovies(string userId)
    {
        var favorites = await _repository.GetFavorites(userId);
        var favoriteIds = favorites.Select(f => f.FavoriteMovieId).ToHashSet();
        return ToMovieDto(favorites, favoriteIds);
    }
    
    public async Task<MoviesResponseDto> GetTopFavoriteMovies(string? userId)
    {
        var topFavorites = await _repository.GetTopFavorites();
        
        var movieIds = topFavorites.Select(fm => fm.FavoriteMovieId);
        var favoriteMovieIds = await GetFavorites(userId, movieIds);
        
        return ToMovieDto(topFavorites, favoriteMovieIds);
    }

    private static MoviesResponseDto ToMovieDto(IEnumerable<FavoriteMovie> favoriteMovies, IReadOnlySet<int> favoriteIds)
    {
        return new MoviesResponseDto
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

    private async Task<IReadOnlySet<int>> GetFavorites(string? userId, IEnumerable<int> movieIds)
    {
        return userId == null ? new HashSet<int>() : await _repository.GetFavorites(userId, movieIds);
    }
    
    public async Task AddFavorite(FavoritesDto favoritesDto)
    {   
        var existingFavorite = await _repository.GetFavorite(favoritesDto.MovieId);
        if (existingFavorite != null)
        {
            await _repository.AddFavorite(favoritesDto.UserId, existingFavorite);
        }
        else
        {
            var movie = await GetMovieAsync(favoritesDto.MovieId);
            var favoriteMovie = new FavoriteMovie
            {
                FavoriteMovieId = movie.Id,
                Title = movie.Title,
                Overview = movie.Overview,
                ReleaseDate = movie.Release_date,
                ImageUrl = movie.Poster_path
            };
        
            await _repository.AddFavorite(favoritesDto.UserId, favoriteMovie);
        }
    }
    public async Task RemoveFavorite(FavoritesDto favoritesDto)
    {
        await _repository.RemoveFavorite(favoritesDto.UserId, favoritesDto.MovieId);
    }

    public async Task<Rating> GetMovieRatingAsync(int movieId)
    {
        return await _repository.GetMovieRatingAsync(movieId);
    }

    public async Task<RatedMovieDto> AddRatedMovieAsync(RatedMovieDto ratedMovie)
    {
        RatedMovie movie = new RatedMovie
        {
            RatedMovieId = ratedMovie.RatedMovieId.Value,
            UserId = ratedMovie.UserId,
            Rating = ratedMovie.Rating.Value
        };

        await _repository.AddRatedMovieAsync(movie);

        return ratedMovie;
    }

    public async Task<RatingDto> AddRatingAsync(RatedMovieDto ratedMovie)
    {
        RatedMovie movie = new RatedMovie
        {
            RatedMovieId = ratedMovie.RatedMovieId.Value,
            UserId = ratedMovie.UserId,
            Rating = ratedMovie.Rating.Value
        };

       RatedMovie existingMovie = await _repository.GetMovieRatingByUserId(movie.UserId, movie.RatedMovieId);
       Rating rating = await GetMovieRatingAsync(movie.RatedMovieId);
       double newRatingValue;
       int newVotes;
       if (existingMovie.RatedMovieId == 0)
       {
           await _repository.AddRatedMovieAsync(movie);
           if (rating.MovieId == 0)
           {
               var movieResponse = await _moviesClient.GetMovieAsync(movie.RatedMovieId, api_key, "en_US");
               newVotes = movieResponse.Vote_count.Value + 1;
               newRatingValue = ((movieResponse.Vote_average.Value * movieResponse.Vote_count.Value) +
                            ratedMovie.Rating.Value) / newVotes;
               rating = new Rating
               {
                   MovieId = movie.RatedMovieId,
                   RatingValue = newRatingValue,
                   Votes = newVotes
               };
               await _repository.AddRatingAsync(rating);
           }
           else
           {
               newVotes = (int) (rating.Votes + 1);
               newRatingValue = ((rating.RatingValue * rating.Votes) + ratedMovie.Rating.Value)/ newVotes;
               rating = new Rating
               {
                   MovieId = movie.RatedMovieId,
                   RatingValue = newRatingValue,
                   Votes = newVotes
               };
               await _repository.UpdateOnlyRatingAsync(rating);
           }
       }
       else
       {
           int oldVotes = (int) (rating.Votes - 1);
           double oldRatingValue = ((rating.RatingValue * rating.Votes) - ratedMovie.Rating.Value)/ oldVotes;
           newVotes = (int) (rating.Votes + 1);
           newRatingValue = ((oldRatingValue * rating.Votes) + ratedMovie.Rating.Value)/ newVotes;
           rating = new Rating
           {
               MovieId = movie.RatedMovieId,
               RatingValue = newRatingValue,
               Votes = rating.Votes
           };

           await _repository.UpdateRatingAsync(rating, movie);
       }

       RatingDto newRating = new RatingDto
       {
           MovieId = rating.MovieId,
           RatingValue = rating.RatingValue,
           Votes = (int?) rating.Votes
       };
       return newRating;
    }
}