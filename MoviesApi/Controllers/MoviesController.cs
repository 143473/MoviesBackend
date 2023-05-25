using Microsoft.AspNetCore.Mvc;
using MoviesDB.API.Swagger.Controllers.Generated;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MoviesApi.Services.Interfaces;

namespace MoviesApi.Controllers;

public class MoviesController : MoviesControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }
    public override Task<ActionResult<MovieResponseDto>> GetMovie(string userId, int movie_id)
    {
        return Task.Run<ActionResult<MovieResponseDto>>(async () =>
        {
            var result = await _movieService.GetMovieAsync(userId, movie_id);
            return Ok(result);
        });
    }

    public override async Task<ActionResult<MovieListDto>> GetMoviesByTitle(string title, string userId)
    {
        return await _movieService.GetMoviesByTitleAsync(userId, title);
    }
    
    public override async Task<IActionResult> AddFavoriteMovie(FavoritesDto favoritesDto)
    {
        await _movieService.AddFavorite(favoritesDto);
        return Ok();
    }
    
    public override async Task<ActionResult<MovieListDto>> GetFavoriteMovies(string userId)
    {
        return await _movieService.GetFavoriteMovies(userId);
    }

    public override async Task<IActionResult> DeleteFavoriteMovie(FavoritesDto favoritesDto)
    {
        await _movieService.RemoveFavorite(favoritesDto);
        return Ok();
    }
    
    public override async Task<ActionResult<MovieListDto>> GetTopFavoriteMovies(string userId)
    {
        return await _movieService.GetTopFavorites(userId);    
    }

    public override Task<ActionResult<RatingDto>> GetMovieRating(int movieId)
    {
        return Task.Run<ActionResult<RatingDto>>(async () =>
        {
            var result = await _movieService.GetMovieRatingAsync(movieId);
            return Ok(result);
        });
    }

    public override Task<IActionResult> AddRatedMovie(RatedMovieDto ratedMovie)
    {
        return Task.Run<IActionResult>(async () =>
        {
            var result = await _movieService.AddRatedMovieAsync(ratedMovie);
            return Ok(result);
        });
    }

    public override Task<IActionResult> AddRating(RatedMovieDto ratedMovie)
    {
        return Task.Run<IActionResult>(async () =>
        {
            var result = await _movieService.AddRatingAsync(ratedMovie);
            return Created($"ratedMovie/{ratedMovie.RatedMovieId}", result);
        });
    }
}