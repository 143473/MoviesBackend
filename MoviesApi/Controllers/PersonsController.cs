using Microsoft.AspNetCore.Mvc;
using MoviesApi.Services.Interfaces;
using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;

namespace MoviesApi.Controllers;

public class PersonsController : PersonsControllerBase
{
    private readonly IPersonService _personService;

    public PersonsController(IPersonService personService)
    {
        _personService = personService;
    }

    public override Task<ActionResult<PersonDetailsDTO>> GetPerson(int personId)
    {
        return Task.Run<ActionResult<PersonDetailsDTO>>(async () =>
        {
            var result = await _personService.GetPersonDetails(personId);
            return Ok(result);
        });
    }

    public override Task<ActionResult<PersonMovieCreditsDTO>> GetPersonMovies(int personId)
    {
        return Task.Run<ActionResult<PersonMovieCreditsDTO>>(async () =>
        {
            var result = await _personService.GetPersonMovies(personId);
            return Ok(result);
        });
    }

    public override Task<ActionResult<PersonsResponseDTO>> GetPersonsByName(string query)
    {
        return Task.Run<ActionResult<PersonsResponseDTO>>(async () =>
        {
            var result = await _personService.GetPersonsByName(query);
            return Ok(result);
        });
    }
}