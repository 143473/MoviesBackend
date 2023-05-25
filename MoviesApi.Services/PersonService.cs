using Microsoft.Extensions.Configuration;
using MoviesApi.Data.Repositories.Interfaces;
using MoviesApi.Services.Helpers;
using MoviesApi.Services.Interfaces;
using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;

namespace MoviesApi.Services;

public class PersonService : IPersonService
{
    private readonly IPersonsClient _personsClient;
    private string api_key;

    public PersonService(IPersonsClient personsClient, IConfiguration configuration)
    {
        _personsClient = personsClient;
        api_key = configuration.GetSection("TMDB")["APIKey"];
    }

    public async Task<PersonsResponseDTO> GetPersonsByName(string name, bool includeAdult = false, string language ="en-US", int page = 1)
    {
        var response = await _personsClient.GetPersonsByNameAsync(api_key,name, includeAdult, language, page);
        return DTOMapper.GePersonsResponseDto(response);
    }

    public async Task<PersonDetailsDTO> GetPersonDetails(int id, string language = "en-US")
    {
        var response = await _personsClient.GetPersonAsync(api_key, id, language);
        return DTOMapper.GetPersonDetailsDto(response);
    }

    public async Task<PersonMovieCreditsDTO> GetPersonMovies(int id, string language = "en-Us")
    {
        var response = await _personsClient.GetPersonMoviesAsync(api_key, id, language);
        return DTOMapper.GetPersonMovieCreditsDTO(response);
    }
}