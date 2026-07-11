using Execution.AddMovie.BusinessActions;
using Execution.AddMovie.Delivery;
using Execution.AddMovie.Workflow;
using Execution.DeleteMovie.BusinessActions;
using Execution.DeleteMovie.Delivery;
using Execution.DeleteMovie.Workflow;
using Execution.GetMovies.Delivery;
using Execution.GetMovies.Workflow;
using Execution.UpdateMovie.BusinessActions;
using Execution.UpdateMovie.Delivery;
using Execution.UpdateMovie.Workflow;
using Persistence;
using SharedKernel.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IClock, SystemClock>();

builder.Services.AddScoped<EnsureTitleUniqueAction>();
builder.Services.AddScoped<PersistNewMovieAction>();
builder.Services.AddScoped<AddMovieOrchestrator>();
builder.Services.AddScoped<LoadMovieAction>();
builder.Services.AddScoped<PersistMovieAction>();
builder.Services.AddScoped<UpdateMovieOrchestrator>();
builder.Services.AddScoped<DeleteMovieAction>();
builder.Services.AddScoped<DeleteMovieOrchestrator>();
builder.Services.AddScoped<GetMoviesOrchestrator>();

builder.Services.AddMovieWatchlistPersistence(
    builder.Configuration.GetConnectionString("MovieWatchlist")
    ?? "Server=(localdb)\\mssqllocaldb;Database=MovieWatchlist_Dev;Trusted_Connection=True;TrustServerCertificate=True;");

var app = builder.Build();

app.MapGet("/health/live", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/health/ready", () => Results.Ok(new { status = "Ready" }));

AddMovieEndpoint.Map(app);
GetMoviesEndpoint.Map(app);
UpdateMovieEndpoint.Map(app);
DeleteMovieEndpoint.Map(app);

app.Run();

public partial class Program { }