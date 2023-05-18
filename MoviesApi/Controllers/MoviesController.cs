using Microsoft.AspNetCore.Mvc;
using MoviesDB.API.Swagger.Controllers.Generated;
using MoviesDb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
}