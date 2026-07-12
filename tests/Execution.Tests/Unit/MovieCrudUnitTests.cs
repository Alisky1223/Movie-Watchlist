using Execution.AddMovie.BusinessActions;
using Execution.AddMovie.Delivery;
using Execution.AddMovie.Domain.ValueObjects;
using Execution.AddMovie.Workflow;
using Execution.DeleteMovie.BusinessActions;
using Execution.DeleteMovie.Workflow;
using Execution.Domain.Entities;
using Execution.Domain.Repositories;
using Execution.Domain.ValueObjects;
using Execution.GetMovies.Workflow;
using Execution.UpdateMovie.BusinessActions;
using Execution.UpdateMovie.Delivery;
using Execution.UpdateMovie.Domain.ValueObjects;
using Execution.UpdateMovie.Workflow;
using SharedKernel.Domain.Services;
using Xunit;

namespace Execution.Tests.Unit
{
    public sealed class MovieCrudUnitTests
    {
        [Fact]
        public void AddMovieValidator_EmptyTitle_ReturnsValidationError()
        {
            // Given
            var request = new AddMovieRequest("   ", "SciFi");

            // When
            var result = AddMovieValidator.Validate(request);

            // Then
            Assert.False(result.IsSuccess);
            Assert.Equal("MovieValidation", result.Error.Code);
            Assert.Contains("Title", result.Error.Message);
        }

        [Fact]
        public void AddMovieValidator_InvalidGenre_ReturnsValidationError()
        {
            // Given
            var request = new AddMovieRequest("Inception", "NotAGenre");

            // When
            var result = AddMovieValidator.Validate(request);

            // Then
            Assert.False(result.IsSuccess);
            Assert.Equal("MovieValidation", result.Error.Code);
        }

        [Fact]
        public async Task AddMovieOrchestrator_ValidCommand_PersistsMovieAndReturnsResult()
        {
            // Given
            var repository = new FakeMovieRepository();
            var action = new EnsureTitleUniqueAction(repository);
            var persistAction = new PersistNewMovieAction(repository);
            var orchestrator = new AddMovieOrchestrator(action, persistAction, new FakeClock());
            var command = new AddMovieCommand("Inception", MovieGenre.SciFi);

            // When
            var result = await orchestrator.Execute(command, CancellationToken.None);

            // Then
            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value!.Id);
            Assert.Equal("Inception", result.Value.Title);
            Assert.Single(repository.AddedMovies);
        }

        [Fact]
        public async Task AddMovieOrchestrator_DuplicateTitle_ReturnsMovieAlreadyExistsError()
        {
            // Given
            var existingMovie = Movie.Create("Inception", MovieGenre.SciFi, new FakeClock());
            var repository = new FakeMovieRepository(existingMovie);
            var action = new EnsureTitleUniqueAction(repository);
            var persistAction = new PersistNewMovieAction(repository);
            var orchestrator = new AddMovieOrchestrator(action, persistAction, new FakeClock());
            var command = new AddMovieCommand("Inception", MovieGenre.SciFi);

            // When
            var result = await orchestrator.Execute(command, CancellationToken.None);

            // Then
            Assert.False(result.IsSuccess);
            Assert.Equal("MovieAlreadyExists", result.Error.Code);
        }

        [Fact]
        public void UpdateMovieValidator_ValidRequest_ReturnsCommand()
        {
            // Given
            var request = new UpdateMovieRequest("Inception 2", "Drama");

            // When
            var result = UpdateMovieValidator.Validate(request);

            // Then
            Assert.True(result.IsSuccess);
            Assert.Equal("Inception 2", result.Value!.Title);
        }

        [Fact]
        public async Task UpdateMovieOrchestrator_ExistingMovieAndUniqueTitle_UpdatesMovieAndReturnsResult()
        {
            // Given
            var repository = new FakeMovieRepository();
            var movie = Movie.Create("Original", MovieGenre.Action, new FakeClock());
            repository.Add(movie);
            var loadAction = new LoadMovieAction(repository);
            var persistAction = new PersistMovieAction(repository);
            var orchestrator = new UpdateMovieOrchestrator(loadAction, persistAction, repository);
            var command = new UpdateMovieCommand(movie.Id, "Updated", "Drama");

            // When
            var result = await orchestrator.Execute(command, CancellationToken.None);

            // Then
            Assert.True(result.IsSuccess);
            Assert.Equal("Updated", result.Value!.Title);
            Assert.Equal(MovieGenre.Drama, result.Value.Genre);
        }

        [Fact]
        public async Task UpdateMovieOrchestrator_MissingMovie_ReturnsMovieNotFoundError()
        {
            // Given
            var repository = new FakeMovieRepository();
            var loadAction = new LoadMovieAction(repository);
            var persistAction = new PersistMovieAction(repository);
            var orchestrator = new UpdateMovieOrchestrator(loadAction, persistAction, repository);
            var command = new UpdateMovieCommand(Guid.NewGuid(), "Updated", "Drama");

            // When
            var result = await orchestrator.Execute(command, CancellationToken.None);

            // Then
            Assert.False(result.IsSuccess);
            Assert.Equal("MovieNotFound", result.Error.Code);
        }

        [Fact]
        public async Task DeleteMovieOrchestrator_ExistingMovie_RemovesMovieAndReturnsSuccess()
        {
            // Given
            var repository = new FakeMovieRepository();
            var movie = Movie.Create("Delete Me", MovieGenre.Action, new FakeClock());
            repository.Add(movie);
            var loadAction = new LoadMovieAction(repository);
            var deleteAction = new DeleteMovieAction(repository);
            var orchestrator = new DeleteMovieOrchestrator(loadAction, deleteAction);

            // When
            var result = await orchestrator.Execute(movie.Id, CancellationToken.None);

            // Then
            Assert.True(result.IsSuccess);
            Assert.Contains(movie, repository.RemovedMovies);
        }

        [Fact]
        public async Task DeleteMovieOrchestrator_MissingMovie_ReturnsMovieNotFoundError()
        {
            // Given
            var repository = new FakeMovieRepository();
            var loadAction = new LoadMovieAction(repository);
            var deleteAction = new DeleteMovieAction(repository);
            var orchestrator = new DeleteMovieOrchestrator(loadAction, deleteAction);

            // When
            var result = await orchestrator.Execute(Guid.NewGuid(), CancellationToken.None);

            // Then
            Assert.False(result.IsSuccess);
            Assert.Equal("MovieNotFound", result.Error.Code);
        }

        [Fact]
        public async Task GetMoviesOrchestrator_ExistingMovies_ReturnsPagedResult()
        {
            // Given
            var repository = new FakeMovieRepository();
            repository.Add(Movie.Create("First", MovieGenre.Action, new FakeClock()));
            repository.Add(Movie.Create("Second", MovieGenre.Comedy, new FakeClock()));
            var orchestrator = new GetMoviesOrchestrator(repository);

            // When
            var result = await orchestrator.Execute(1, 10, CancellationToken.None);

            // Then
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.TotalCount);
            Assert.Equal(2, result.Value.Items.Count);
        }

        [Fact]
        public void Movie_Create_ValidData_SetsImmutableFields()
        {
            // Given
            var clock = new FakeClock();

            // When
            var movie = Movie.Create("  Inception  ", MovieGenre.SciFi, clock);

            // Then
            Assert.Equal("Inception", movie.Title);
            Assert.Equal(MovieGenre.SciFi, movie.Genre);
            Assert.Equal(clock.UtcNow, movie.CreatedAt);
        }

        [Fact]
        public void Movie_ApplyChanges_ValidChanges_UpdatesTitleAndGenre()
        {
            // Given
            var movie = Movie.Create("Original", MovieGenre.Action, new FakeClock());

            // When
            movie.ApplyChanges("Updated", MovieGenre.Drama);

            // Then
            Assert.Equal("Updated", movie.Title);
            Assert.Equal(MovieGenre.Drama, movie.Genre);
        }

        private sealed class FakeMovieRepository : IMovieRepository
        {
            private readonly List<Movie> _movies = new();

            public FakeMovieRepository(params Movie[] movies)
            {
                _movies.AddRange(movies);
            }

            public List<Movie> AddedMovies { get; } = new();
            public List<Movie> RemovedMovies { get; } = new();

            public Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default)
                => Task.FromResult(_movies.FirstOrDefault(m => m.Id == id));

            public Task<Movie?> GetByTitleAsync(string title, CancellationToken ct = default)
                => Task.FromResult(_movies.FirstOrDefault(m => m.Title.Equals(title.Trim(), StringComparison.OrdinalIgnoreCase)));

            public Task<IReadOnlyCollection<Movie>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
            {
                var items = _movies.OrderByDescending(m => m.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Task.FromResult<IReadOnlyCollection<Movie>>(items);
            }

            public Task<int> GetTotalCountAsync(CancellationToken ct = default)
                => Task.FromResult(_movies.Count);

            public void Add(Movie movie) => _movies.Add(movie);

            public void Remove(Movie movie)
            {
                _movies.Remove(movie);
                RemovedMovies.Add(movie);
            }

            public Task SaveChangesAsync(CancellationToken ct = default)
            {
                AddedMovies.AddRange(_movies.Where(m => !AddedMovies.Contains(m)));
                return Task.CompletedTask;
            }
        }

        private sealed class FakeClock : IClock
        {
            public DateTimeOffset UtcNow { get; } = new(2026, 7, 11, 12, 0, 0, TimeSpan.Zero);
        }
    }
}
