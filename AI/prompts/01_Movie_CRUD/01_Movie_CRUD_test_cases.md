# 01_Movie_CRUD Test Cases

## Unit Test Cases

| Test Name | Level | Behavior | Entry Point | Critical Standards |
|---|---|---|---|---|
| AddMovieValidator_EmptyTitle_ReturnsValidationError | Unit | Rejects empty or whitespace title before orchestration | AddMovieValidator | Given/When/Then, validation contract, explicit error assertion |
| AddMovieValidator_InvalidGenre_ReturnsValidationError | Unit | Rejects unsupported genre values | AddMovieValidator | Given/When/Then, typed validation result |
| AddMovieOrchestrator_ValidCommand_PersistsMovieAndReturnsResult | Unit | Creates a movie and returns a successful result | AddMovieOrchestrator | Given/When/Then, result contract, persistence side effect |
| AddMovieOrchestrator_DuplicateTitle_ReturnsMovieAlreadyExistsError | Unit | Stops duplicate title before persistence | AddMovieOrchestrator | Given/When/Then, typed error handling |
| UpdateMovieOrchestrator_ExistingMovieAndUniqueTitle_UpdatesMovieAndReturnsResult | Unit | Updates existing movie with valid new title/genre | UpdateMovieOrchestrator | Given/When/Then, domain update behavior |
| UpdateMovieOrchestrator_MissingMovie_ReturnsMovieNotFoundError | Unit | Returns not found when movie does not exist | UpdateMovieOrchestrator | Given/When/Then, typed not-found handling |
| UpdateMovieOrchestrator_TitleConflictWithAnotherMovie_ReturnsMovieAlreadyExistsError | Unit | Rejects title conflict during update | UpdateMovieOrchestrator | Given/When/Then, conflict behavior |
| DeleteMovieOrchestrator_ExistingMovie_RemovesMovieAndReturnsSuccess | Unit | Deletes an existing movie and returns success | DeleteMovieOrchestrator | Given/When/Then, successful delete behavior |
| DeleteMovieOrchestrator_MissingMovie_ReturnsMovieNotFoundError | Unit | Returns not found for missing movie | DeleteMovieOrchestrator | Given/When/Then, typed failure |
| GetMoviesOrchestrator_ExistingMovies_ReturnsPagedResult | Unit | Returns ordered paged results with total count | GetMoviesOrchestrator | Given/When/Then, pagination contract |
| Movie_Create_ValidData_SetsImmutableFields | Unit | Creates a movie with trimmed title and generated metadata | Movie | Given/When/Then, domain invariant |
| Movie_ApplyChanges_ValidChanges_UpdatesTitleAndGenre | Unit | Applies title and genre updates | Movie | Given/When/Then, domain mutation |

## Integration Test Cases

| Test Name | Level | Behavior | Entry Point | Critical Standards |
|---|---|---|---|---|
| AddMovieEndpoint_ValidRequest_Returns201CreatedAndPersistsMovie | Integration | Full endpoint flow creates a movie and persists it | POST /movies | Real endpoint + real persistence boundary, response contract, deterministic test data |
| AddMovieEndpoint_DuplicateTitle_Returns409Conflict | Integration | Duplicate title is rejected through endpoint pipeline | POST /movies | Real validation and persistence interaction |
| UpdateMovieEndpoint_ExistingMovie_Returns200OkAndUpdatedMovie | Integration | Updating existing movie returns updated payload | PUT /movies/{movieId} | Real orchestration + persistence interaction |
| DeleteMovieEndpoint_ExistingMovie_Returns204NoContent | Integration | Deleting existing movie removes it | DELETE /movies/{movieId} | End-to-end delete behavior with persistence |
| GetMoviesEndpoint_ValidRequest_Returns200AndPagedPayload | Integration | List endpoint returns paged content from persistence | GET /movies | Response contract and pagination |

## E2E / Scenario Test Cases

| Test Name | Level | Behavior | Entry Point | Critical Standards |
|---|---|---|---|---|
| CreateAndListMovies_ValidFlow_ReturnsCreatedMovieAndVisibleCatalogEntry | E2E | User can create a movie and later see it in the list | POST /movies + GET /movies | High-value flow, visible API outcome, no internal assertions |
