using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;
using MoviesApi.Data.Models;
using MoviesApi.Data.Repositories.Interfaces;
using MoviesApi.Services.Helpers;
using MoviesApi.Services.Interfaces;
using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;

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

    public async Task<MovieResponseDto> GetMovieAsync(string? userId, int movie_id, string language = "en_US")
    {
        var movieResponse = await _moviesClient.GetMovieAsync(movie_id, api_key, language);
        var favorites = await GetFavoriteIds(userId, new[] {movie_id});
        return movieResponse.ToMovieResponseDto(favorites);
    }

    public async Task<MovieListDto> GetMoviesByTitleAsync(string? userId, string movieName,
        string language = "en_US")
    {
        var moviesResponse = await _moviesClient.GetMoviesByTitleAsync(api_key, movieName, null, language);
        var movieIds = moviesResponse.Results.Select(r => r.Id);
        var favoriteMovieIds = await GetFavoriteIds(userId, movieIds);
        return moviesResponse.ToMovieDto(favoriteMovieIds);
    }

    public async Task<MovieListDto> GetFavoriteMovies(string userId)
    {
        var favorites = await _repository.GetFavorites(userId);
        var favoriteIds = favorites.Select(f => f.FavoriteMovieId).ToHashSet();
        return favorites.ToMovieDto(favoriteIds);
    }

    public async Task<MovieListDto> GetTopFavorites(string? userId)
    {
        var topFavorites = await _repository.GetTopFavorites();

        var movieIds = topFavorites.Select(fm => fm.FavoriteMovieId);
        var favoritesIds = await GetFavoriteIds(userId, movieIds);

        return topFavorites.ToMovieDto(favoritesIds);
    }

    private async Task<IReadOnlySet<int>> GetFavoriteIds(string? userId, IEnumerable<int> movieIds)
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
            var movie = await GetMovieAsync(favoritesDto.UserId, favoritesDto.MovieId);
            var favoriteMovie = new FavoriteMovie
            {
                FavoriteMovieId = movie.Id,
                Title = movie.Title,
                Overview = movie.Overview,
                ReleaseDate = movie.Release_date,
                ImageUrl = movie.Poster_path ?? string.Empty,
                BackdropPath = movie.Backdrop_path ?? string.Empty
            };

            await _repository.AddFavorite(favoritesDto.UserId, favoriteMovie);
        }
    }

    public async Task RemoveFavorite(FavoritesDto favoritesDto)
    {
        await _repository.RemoveFavorite(favoritesDto.UserId, favoritesDto.MovieId);
    }

    public async Task<MoviesExtendedResponseDto> GetFilteredMovies(DateTimeOffset? fromDate, DateTimeOffset? toDate,
        SortBy? sortBy, string language = "en-US", int page = 1, bool adult = false, float voteCountGte = 10f, float voteAverageLte = 10f)
    {
        var enumType = typeof(SortBy);
        var name = Enum.GetName(typeof(SortBy), sortBy);
        var enumMemberAttribute =
            ((EnumMemberAttribute[]) enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true))
            .Single();
        var sortByTmdbValue = enumMemberAttribute.Value;

        var response =
            await _moviesClient
            .GetFilteredMoviesAsync(
                api_key, adult, voteCountGte, voteAverageLte, language, page, fromDate, toDate, sortByTmdbValue);
        return DTOMapper.GetExtendedMoviesResponseDTO(response);
    }

    public async Task<MovieCreditsResponseDto> GetMovieCreditsAsync(int movieId)
    {
        var movieCreditsResponse = await _moviesClient.GetMovieCreditsAsync(api_key, movieId, "en-US");
        return movieCreditsResponse.ToMovieCreditsDto();
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
        RatedMovie newRatedMovie = new RatedMovie
        {
            RatedMovieId = ratedMovie.RatedMovieId!.Value,
            UserId = ratedMovie.UserId,
            Rating = ratedMovie.Rating!.Value
        };

        RatedMovie? oldRatedMovie = await _repository.GetMovieRatingByUserId(newRatedMovie.UserId, newRatedMovie.RatedMovieId);
        Rating ratingInDb = await GetMovieRatingAsync(newRatedMovie.RatedMovieId);
        Rating newRating;
        
        if (oldRatedMovie == null)
        {
            newRating = await FirstRatingForUser(newRatedMovie, ratingInDb);
        }
        else
        {
            newRating = await UpdateRating(oldRatedMovie, newRatedMovie, ratingInDb);
        }

        RatingDto newRatingDto = new RatingDto
        {
            MovieId = newRating.MovieId,
            RatingValue = newRating.RatingValue,
            Votes = (int?) newRating.Votes
        };
        return newRatingDto;
    }

    private async Task<Rating> FirstRatingForUser(RatedMovie ratedMovie, Rating? rating)
    {
        Rating newRating;
        await _repository.AddRatedMovieAsync(ratedMovie);
        if (rating == null)
        {
            var movieResponse = await _moviesClient.GetMovieAsync(ratedMovie.RatedMovieId, api_key);
            newRating = movieResponse.CalculateNewMovieRating(ratedMovie);
            await _repository.AddRatingAsync(newRating);
        }
        else
        {
            newRating = rating.CalculateExistingMovieRating(ratedMovie);
            await _repository.UpdateOnlyRatingAsync(newRating);
        }

        return newRating;
    }

    private async Task<Rating> UpdateRating(RatedMovie oldRatedMovie, RatedMovie newRatedMovie, Rating rating)
    {
        Rating newRating = oldRatedMovie.CalculateUpdatedMovieRating(newRatedMovie, rating);

        await _repository.UpdateRatingAsync(newRating, newRatedMovie);

        return newRating;
    }


    public async Task<CommentsDto> GetCommentsAsync(int movieId)
    {
        ICollection<Comment> comments = await _repository.GetCommentsAsync(movieId);
        CommentsDto returnedComments = new CommentsDto()
        {
            Comments = comments.Select(c => new CommentDto()
            {
                CommentId = c.CommentId,
                MovieId = c.MovieId,
                UserId = c.UserId,
                Text = c.Text,
                Username = c.Username
            }).ToList()
        };
        
        return returnedComments;
    }

    public async Task<CommentDto> AddCommentAsync(CommentDto comment)
    {
        Comment newComment = new Comment()
        {
            MovieId = comment.MovieId.Value,
            UserId = comment.UserId,
            Text = comment.Text,
            Username = comment.Username
        };

        Comment createdComment = await _repository.AddCommentAsync(newComment);

        comment = new CommentDto()
        {
            CommentId = createdComment.CommentId,
            MovieId = createdComment.MovieId,
            UserId = createdComment.UserId,
            Text = createdComment.Text,
            Username = createdComment.Username
        };
        
        return comment;
    }
}