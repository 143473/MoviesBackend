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
    
    public override Task<ActionResult<MoviesListResponse>> GetMoviesByTitle([BindRequired] string title)
    {
        return Task.Run<ActionResult<MoviesListResponse>>(async () =>
        {
            var result = await _movieService.GetMoviesByTitleAsync(title);
            return Ok(result);
        });
    }
}