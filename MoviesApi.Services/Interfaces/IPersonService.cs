using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;

namespace MoviesApi.Services.Interfaces;

public interface IPersonService
{
    Task<PersonsResponseDTO> GetPersonsByName(string name, bool includeAdult = false, string language = "en-US",
        int page = 1);

    Task<PersonDetailsDTO> GetPersonDetails(int id, string language ="en-US");
    Task<PersonMovieCreditsDTO> GetPersonMovies(int id, string language = "en-Us");
}