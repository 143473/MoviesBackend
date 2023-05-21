using Microsoft.AspNetCore.Mvc;
using MoviesDB.API.Swagger.Controllers.Generated;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MoviesApi.Services.Interfaces;
using RatedMovie = MoviesApi.Data.Models.RatedMovie;

namespace MoviesApi.Controllers;

public class MoviesController : MoviesControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    public override Task<ActionResult<MovieResponse>> GetMovie([BindRequired] int movie_id)
    {
        return Task.Run<ActionResult<MovieResponse>>(async () =>
        {
            var result = await _movieService.GetMovieAsync(movie_id);
            return Ok(result);
        });
    }
    
    public override async Task<ActionResult<MoviesResponseDto>> GetMoviesByTitle([BindRequired] string title)
    {
        return await _movieService.GetMoviesByTitleAsync(title);
    }

    public override async Task<ActionResult<ICollection<MovieDto>>> GetFavoriteMovies(string userId)
    {
        return new ActionResult<ICollection<MovieDto>>(await _movieService.GetFavoriteMovies(userId));
    }

    public override async Task<ActionResult<ICollection<MovieDto>>> GetTopFavoriteMovies()
    {
        return new ActionResult<ICollection<MovieDto>>(await _movieService.GetTopFavoriteMovies());
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
}