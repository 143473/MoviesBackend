using System.Net;
using System.Net.Http.Json;
using Moq;
using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;

namespace MoviesApi.Tests.IntegrationTests;

[TestFixture]
public class IntegrationTest : TestFixture
{
    private readonly CustomWebApplicationFactory<Program> _factory = new();

    [Test]
    public async Task GetMovie()
    {
        var userId = A<string>();
        var movieResponse = A<MovieResponse>();
        var movieId = movieResponse.Id;

        _factory.MoviesRepositoryMock
            .Setup(repository => repository.GetFavorites(It.IsAny<string>(),It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new HashSet<int> {movieId});
        
        _factory.MoviesClientMock
            .Setup(client => client.GetMovieAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(movieResponse);
        
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/movie/{movieId}?userId={userId}");
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var movieResponseDto = await response.Content.ReadFromJsonAsync<MovieResponseDto>();
        Assert.That(movieResponseDto?.IsFavorite, Is.True);
    }
    
    [Test]
    public async Task AddMovieRating()
    {
        var ratedMovieDto = A<RatedMovieDto>();

        var client = _factory.CreateClient();
        
        _factory.MoviesClientMock
            .Setup(client => client.GetMovieAsync(ratedMovieDto.RatedMovieId.Value, It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(A<MovieResponse>());

        var response = await client.PostAsJsonAsync($"/movie/rating", ratedMovieDto);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }
}