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
                ImageUrl = movie.Poster_path
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
                newRatingValue = ((rating.RatingValue * rating.Votes) + ratedMovie.Rating.Value) / newVotes;
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
            double oldRatingValue = ((rating.RatingValue * rating.Votes) - ratedMovie.Rating.Value) / oldVotes;
            newVotes = (int) (rating.Votes + 1);
            newRatingValue = ((oldRatingValue * rating.Votes) + ratedMovie.Rating.Value) / newVotes;
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
        //TODO: Convert comment dto to comment
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