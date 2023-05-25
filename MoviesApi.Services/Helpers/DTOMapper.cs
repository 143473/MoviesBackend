using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;
using MovieResponse = tmdb_api.MovieResponse;

namespace MoviesApi.Services.Helpers;

public static class DTOMapper
{
    public static PersonsResponseDTO GePersonsResponseDto(PersonsResponseTmdb personsResponseTmdb)
    {
        PersonsResponseDTO personsResponseDto = new PersonsResponseDTO();
        personsResponseDto.Persons = new List<PersonDTO>();

        foreach (var result in personsResponseTmdb.Results)
        {
            personsResponseDto.Persons.Add(new PersonDTO
            {
                Adult = result.Adult,
                Gender = result.Gender == 2 ? "male" : "female",
                Id = result.Id,
                Known_for_department = result.Known_for_department,
                Name = result.Name,
                Original_name = result.Original_name,
                Popularity = result.Popularity,
                Profile_path = !string.IsNullOrEmpty(result.Profile_path)
                    ? $"https://image.tmdb.org/t/p/w500{result.Profile_path}"
                    : null,
                Known_for = GetMovieDTOList(result.Known_for)
            });
        }

        return personsResponseDto;
    }

    public static List<MovieDto> GetMovieDTOList(ICollection<MovieResponse?> movieResponses)
    {
        List<MovieDto> movieDTOList = new List<MovieDto>();
        foreach (var movie in movieResponses)
        {
            movieDTOList.Add(GetMovieDTO(movie));
        }

        return movieDTOList;
    }

    public static MoviesExtendedResponseDto GetExtendedMoviesResponseDTO(MoviesResponseTmdb moviesResponseTmdb)
    {
        return new MoviesExtendedResponseDto()
        {
            Results = moviesResponseTmdb.Results
                .Select(movie => GetMovieExtendedDTO(movie))
                .ToList()
        };
    }

    public static MovieExtendedDTO GetMovieExtendedDTO(MovieByTitleTmdb movieTmdb)
    {
        return new MovieExtendedDTO
        {
            Adult = movieTmdb.Adult,
            Backdrop_path = !string.IsNullOrEmpty(movieTmdb.Backdrop_path)
                ? $"https://image.tmdb.org/t/p/w500{movieTmdb.Backdrop_path}"
                : null,
            Id = movieTmdb.Id,
            Original_language = movieTmdb.Original_language,
            Original_title = movieTmdb.Original_title,
            Overview = movieTmdb.Overview,
            Popularity = movieTmdb.Popularity,
            Poster_path = !string.IsNullOrEmpty(movieTmdb.Poster_path)
                ? $"https://image.tmdb.org/t/p/w500{movieTmdb.Poster_path}"
                : null,
            Release_date = movieTmdb.Release_date,
            Title = movieTmdb.Title,
            Video = movieTmdb.Video,
            Vote_average = movieTmdb.Vote_average,
            Vote_count = movieTmdb.Vote_count
        };
        }

    public static MovieDto GetMovieDTO(MovieResponse? movieResponse = null, MovieByTitleTmdb? movieByTitleTmdb = null)
    {
        if (movieResponse != null)
        {
            return new MovieDto
            {
                MovieId = movieResponse.Id,
                Title = movieResponse.Title,
                Description = movieResponse.Overview,
                ReleaseDate = movieResponse.Release_date,
                PosterPath = !string.IsNullOrEmpty(movieResponse.Poster_path)
                    ? $"https://image.tmdb.org/t/p/w500{movieResponse.Poster_path}"
                    : null
            };
        }
        return new MovieDto
        {
            MovieId = movieByTitleTmdb.Id,
            Title = movieByTitleTmdb.Title,
            Description = movieByTitleTmdb.Overview,
            ReleaseDate = movieByTitleTmdb.Release_date,
            PosterPath = !string.IsNullOrEmpty(movieByTitleTmdb.Poster_path)
                ? $"https://image.tmdb.org/t/p/w500{movieByTitleTmdb.Poster_path}"
                : null
        };

    }

    public static PersonDetailsDTO GetPersonDetailsDto(PersonDetailsTmdb personDetailsTmdb)
    {
        return new PersonDetailsDTO
        {
            Biography = personDetailsTmdb.Biography,
            Birthday = personDetailsTmdb.Birthday,
            Deathday = personDetailsTmdb.Deathday,
            Gender = personDetailsTmdb.Gender == 2 ? "male" : "female",
            Homepage = personDetailsTmdb.Homepage,
            Id = personDetailsTmdb.Id,
            Name = personDetailsTmdb.Name,
            Place_of_birth = personDetailsTmdb.Place_of_birth,
            Popularity = personDetailsTmdb.Popularity,
            Profile_path = !string.IsNullOrEmpty(personDetailsTmdb.Profile_path)
                ? $"https://image.tmdb.org/t/p/w500{personDetailsTmdb.Profile_path}"
                : null
        };
    }

    public static PersonMovieCreditsDTO GetPersonMovieCreditsDTO(PersonMoviesResponseTmdb personMoviesResponseTmdb)
    {
        return new PersonMovieCreditsDTO()
        {
            Id = personMoviesResponseTmdb.Id,
            Crew = GetPersonMovieListDTO(personMoviesResponseTmdb.Crew),
            Cast = GetPersonMovieListDTO(personMoviesResponseTmdb.Cast)
        };
    }

    public static List<PersonMovieDTO> GetPersonMovieListDTO(ICollection<PersonMovie>? crew = null,
        ICollection<PersonMovie>? cast = null)
    {
        List<PersonMovieDTO> list = new List<PersonMovieDTO>();
        if (crew == null)
        {
            foreach (var personMovie in cast)
            {
                list.Add(GetPersonMovieDTO(personMovie));
            }
        }
        else
        {
            foreach (var personMovie in crew)
            {
                list.Add(GetPersonMovieDTO(personMovie));
            }
        }

        return list;
    }

    public static PersonMovieDTO GetPersonMovieDTO(PersonMovie personMovie)
    {
        return new PersonMovieDTO
        {
            Id = personMovie.Id,
            Original_language = personMovie.Original_language,
            Original_title = personMovie.Original_title,
            Overview = personMovie.Overview,
            Popularity = personMovie.Popularity,
            Poster_path = !string.IsNullOrEmpty(personMovie.Poster_path)
                ? $"https://image.tmdb.org/t/p/w500{personMovie.Poster_path}"
                : null,
            Release_date = personMovie.Release_date,
            Title = personMovie.Title,
            Vote_average = personMovie.Vote_average,
            Vote_count = personMovie.Vote_count,
            Character = personMovie.Character,
            Credit_id = personMovie.Credit_id,
            Order = personMovie.Order,
            Department = personMovie.Department,
            Job = personMovie.Job
        };
    }
}