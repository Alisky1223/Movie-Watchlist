# 01_Movie_CRUD Test Scope

## Story Classification
- Story Type: Business Story
- Reason: این استوری روی منطق CRUD فیلم، اعتبارسنجی ورودی، وضعیت خطاهای دامنه‌ای و قرارداد API تمرکز دارد. احراز هویت و policy enforcement در Slice A deferred است و در این استوری فقط نقش‌های کنترل‌کننده در سطح API و workflow بررسی می‌شوند.

## Test Pyramid Decision
- Unit Test: 70%
- Integration Test: 20%
- E2E / Scenario Test: 10%
- Rationale: منطق بیزینسی و validation dominant است، اما همکاری واقعی endpoint + persistence نیز باید با یک لایه integration پوشش داده شود. تنها یک مسیر API مهم برای regression و smoke نیاز به E2E دارد.

## Requirement Traceability
| Requirement / Acceptance Criteria | Primary Level | Secondary Level |
|---|---|---|
| Add movie success with valid title and genre | Unit | Integration |
| Duplicate title returns 409 conflict | Unit | Integration |
| Validation errors return 400 bad request | Unit | Integration |
| Update movie success | Unit | Integration |
| Update movie not found returns 404 | Unit | Integration |
| Update title conflict returns 409 | Unit | Integration |
| Delete movie success | Unit | Integration |
| Delete movie not found returns 404 | Unit | Integration |
| Get movies list with pagination | Unit | Integration |
| Empty list returns empty array | Unit | Integration |
| CancellationToken propagates through workflow | Unit | Integration |

## Units to Test
- Movie entity: Create and ApplyChanges behavior
- AddMovieValidator
- UpdateMovieValidator
- EnsureTitleUniqueAction
- PersistNewMovieAction
- AddMovieOrchestrator
- UpdateMovieOrchestrator
- DeleteMovieOrchestrator
- GetMoviesOrchestrator
- Typed error mapping and result flow

## Bounded Contexts / Components for Integration Test
- Execution slice AddMovie
- Execution slice UpdateMovie
- Execution slice DeleteMovie
- Execution slice GetMovies
- Persistence + EF Core InMemory test host
- API endpoint layer for POST /movies, PUT /movies/{movieId}, DELETE /movies/{movieId}, GET /movies

## Entry Points for E2E / Scenario Test
- POST /movies
- GET /movies?page=1&pageSize=10

## Test Standards to Enforce
- Unit tests must follow Classicist style: small cohesive behavior, Given/When/Then, no over-mocking, no implementation-detail assertions.
- Integration tests must use real application components and a real in-memory persistence boundary; no mock-only assertions.
- E2E tests must cover only high-risk visible behavior and must not test internal implementation details.
- All tests must avoid production secrets, real user data, and non-deterministic timing.
- Test names must be explicit and behavior-oriented.
