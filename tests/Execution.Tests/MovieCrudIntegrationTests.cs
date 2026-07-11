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
    public sealed class MovieCrudIntegrationTests : IClassFixture<MovieCrudWebApplicationFactory>, IAsyncLifetime
    {

        private readonly HttpClient _client;
        private readonly MovieCrudWebApplicationFactory _factory;

        public MovieCrudIntegrationTests(MovieCrudWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            // Clean slate before every test
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MovieWatchlistDbContext>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

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

        [Fact]
        public async Task AddMovieEndpoint_ValidRequest_Returns201CreatedAndPersistsMovie()
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
        public async Task AddMovieEndpoint_DuplicateTitle_Returns409Conflict()
        {
            await _client.PostAsJsonAsync("/movies", new { title = "Duplicate", genre = "SciFi" });

            var response = await _client.PostAsJsonAsync("/movies", new { title = "Duplicate", genre = "SciFi" });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMovieEndpoint_ExistingMovie_Returns200OkAndUpdatedMovie()
        {
            var createdResponse = await _client.PostAsJsonAsync("/movies", new { title = "Original", genre = "Drama" });
            var createdBody = await createdResponse.Content.ReadFromJsonAsync<AddMovieResponse>();

            var response = await _client.PutAsJsonAsync($"/movies/{createdBody!.Id}", new { title = "Updated", genre = "Comedy" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteMovieEndpoint_ExistingMovie_Returns204NoContent()
        {
            var createdResponse = await _client.PostAsJsonAsync("/movies", new { title = "Delete Me", genre = "Action" });
            var createdBody = await createdResponse.Content.ReadFromJsonAsync<AddMovieResponse>();

            var response = await _client.DeleteAsync($"/movies/{createdBody!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetMoviesEndpoint_ValidRequest_Returns200AndPagedPayload()
        {
            await _client.PostAsJsonAsync("/movies", new { title = "Paged", genre = "SciFi" });

            var response = await _client.GetAsync("/movies?page=1&pageSize=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var payload = await response.Content.ReadFromJsonAsync<GetMoviesResponse>();
            Assert.NotNull(payload);
            Assert.NotEmpty(payload!.Items);
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
