# 01_Movie_CRUD Test Execution Checklist

- [ ] Create unit tests for AddMovie and UpdateMovie validation and orchestrator behavior.
- [ ] Create unit tests for DeleteMovie and GetMovies orchestrators.
- [ ] Create unit tests for Movie domain entity invariants.
- [ ] Create integration tests for POST /movies, PUT /movies/{movieId}, DELETE /movies/{movieId}, and GET /movies.
- [ ] Create one E2E-style scenario covering create and list visibility.
- [ ] Use the exact test case names from the test cases file.
- [ ] Keep test names explicit and behavior-oriented.
- [ ] Use in-memory persistence for integration tests and avoid mocks for internal components.
- [ ] Run the test suite with dotnet test tests/Execution.Tests/Execution.Tests.csproj.
- [ ] Fix any failing test and update the result report.
- [ ] Ensure acceptance criteria for add/update/delete/get are covered.
