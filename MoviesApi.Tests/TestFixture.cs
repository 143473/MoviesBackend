using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;

namespace MoviesApi.Tests;

public abstract class TestFixture
{
    protected IFixture Fixture = null!;
    
    protected T A<T>() => Fixture.Create<T>();
    protected Mock<T> Mock<T>() where T : class => Fixture.Freeze<Mock<T>>();
    
    [SetUp]
    public void SetupFixture()
    {
        Fixture = new Fixture().Customize(new AutoMoqCustomization());
    }
}