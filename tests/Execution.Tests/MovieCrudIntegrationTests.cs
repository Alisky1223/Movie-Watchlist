using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence;
using Xunit;

namespace Execution.Tests
{
    public sealed class MovieCrudIntegrationTests : IClassFixture<MovieCrudWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public MovieCrudIntegrationTests(MovieCrudWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AddMovie_ShouldReturn201AndPersistMovie()
        {
            var request = new { title = "Inception", genre = "SciFi" };

            var response = await _client.PostAsJsonAsync("/movies", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var payload = await response.Content.ReadFromJsonAsync<AddMovieResponse>();
            Assert.NotNull(payload);
            Assert.Equal("Inception", payload!.Title);
            Assert.Equal("SciFi", payload.Genre);
        }

        [Fact]
        public async Task GetMovies_ShouldReturnPagedList()
        {
            var addResponse = await _client.PostAsJsonAsync("/movies", new { title = "The Matrix", genre = "SciFi" });
            addResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync("/movies?page=1&pageSize=10");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var payload = await getResponse.Content.ReadFromJsonAsync<GetMoviesResponse>();
            Assert.NotNull(payload);
            Assert.NotEmpty(payload!.Items);
            Assert.Equal(1, payload.Page);
            Assert.Equal(10, payload.PageSize);
        }
    }

    public sealed class MovieCrudWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<MovieWatchlistDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<MovieWatchlistDbContext>>();
            

                services.AddDbContext<MovieWatchlistDbContext>(options =>
                    options.UseInMemoryDatabase("MovieCrudTests"));
            });
        }
    }

    public sealed record AddMovieResponse(Guid Id, string Title, string Genre, DateTimeOffset CreatedAt);
    public sealed record GetMoviesResponse(IReadOnlyCollection<GetMovieItemResponse> Items, int Page, int PageSize, int TotalCount);
    public sealed record GetMovieItemResponse(Guid Id, string Title, string Genre, DateTimeOffset CreatedAt);
}
