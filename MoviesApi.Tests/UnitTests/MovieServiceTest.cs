using Microsoft.Extensions.Configuration;
using Moq;
using MoviesApi.Data.Repositories.Interfaces;
using MoviesApi.Services;
using AutoFixture;
using MoviesApi.Data.Models;
using tmdb_api;

namespace MoviesApi.Tests.UnitTests;

[TestFixture]
public class MovieServiceTest : TestFixture
{
    private Mock<IMoviesClient> _clientMock;
    private Mock<IConfiguration> _configMock;
    private Mock<IMoviesRepository> _repositoryMock;
    private const string ApiKey = "123";
    private const string Language = "en_US";
    private MovieService Sut => Fixture.Create<MovieService>();

    [SetUp]
    public void Setup()
    {
        _clientMock = Mock<IMoviesClient>();
        _configMock = Mock<IConfiguration>();
        _configMock.Setup(c => c.GetSection("TMDB")["APIKey"]).Returns(ApiKey);
        _repositoryMock = Mock<IMoviesRepository>();
    }

    [Test]
    public async Task MovieIsNotFavoriteIfUserNotLoggedIn()
    {
        var response = A<MovieResponse>();
        var movieId = response.Id;
        _clientMock.Setup(c => c.GetMovieAsync(movieId, ApiKey, Language, default))
            .ReturnsAsync(response);
        
        var mrDto = await Sut.GetMovieAsync(null, movieId);

        Assert.That(mrDto.IsFavorite, Is.False);
    }
    
    [Test]
    public async Task MovieIsFavoriteIfUserLoggedIn()
    {
        var userId = A<string>();
        var response = A<MovieResponse>();
        var movieId = response.Id;
        
        _clientMock.Setup(c => c.GetMovieAsync(movieId, ApiKey, Language, default))
            .ReturnsAsync(response);
        _repositoryMock.Setup(r => r.GetFavorites(userId, new[] {movieId}))
            .ReturnsAsync(new HashSet<int>{movieId});
        
        var mrDto = await Sut.GetMovieAsync(userId, movieId);

        Assert.That(mrDto.IsFavorite, Is.True);
    }
    
    [Test]
    public async Task GetMovieRatingOfTheRequiredMovie()
    {
     var movieId = A<int>();
     var rating = A<Rating>();
     rating.MovieId = movieId;
     _repositoryMock.Setup(r => r.GetMovieRatingAsync(movieId))
      .ReturnsAsync(rating);
     
     var response = await Sut.GetMovieRatingAsync(movieId);

     Assert.That(response.MovieId, Is.EqualTo(movieId));
    }


}