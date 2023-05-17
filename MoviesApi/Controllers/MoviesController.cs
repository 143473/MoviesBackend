using Microsoft.AspNetCore.Mvc;
using MoviesDB.API.Swagger.Controllers.Generated;
using MoviesDb.Services.Interfaces;

namespace MoviesApi.Controllers;

public class MoviesController : MoviesControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    public override Task<ActionResult<MoviesListResponse>> GetListOfMovies(int movie_id, string language, int? page)
    {
        return Task.Run<ActionResult<MoviesListResponse>>(async () =>
        {
            // use model state to check for bad request ?
            
            var result = await _movieService.GetMovies(movie_id, language, page);
            return Ok(result);
        });
    }
}