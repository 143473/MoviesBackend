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

    public override async Task<ActionResult<ICollection<MovieDto>>> GetFavoriteMovies(int userId)
    {
        return new ActionResult<ICollection<MovieDto>>(await _movieService.GetFavoriteMovies(userId));
    }

    public override async Task<ActionResult<ICollection<MovieDto>>> GetTopFavoriteMovies()
    {
        return new ActionResult<ICollection<MovieDto>>(await _movieService.GetTopFavoriteMovies());
    }
}