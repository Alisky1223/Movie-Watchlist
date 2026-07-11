US-001: Add movie to shared watchlist

As a:        Authenticated user (role: user or admin)
I want:      Add a movie to the global/shared watchlist with title and genres and optionally my watched status and rating
So that:     The shared movie catalog grows and my personal watch metadata (watched/rating) is stored per-user

Preconditions:
- Caller is authenticated (X-User-Id header present) and has at least role `user` or `admin` (⚠ ASSUMPTION: temporary header-based auth until external IAM integration)
- Title must be unique in Movies (case-insensitive)

Trigger:
- HTTP POST /api/movies  with JSON payload

story> prd> implemantioan> reviwe


Happy Path:
1. User → API Endpoint : POST /api/movies {title, genres[], optional watched, optional rating} (uses BR-001, BR-002, BR-003, SVC-001)
2. API → Validate input (title non-empty, genres array allowed, rating integer if provided) (uses BR-002)
3. API → MoviesService.checkDuplicate(title) : rejects if duplicate (uses BR-001)
4. API → MoviesRepository.save(Movie) : create global Movie (SVC-001)
5. If request includes watched/rating → WatchlistService.saveEntry(userId, movieId, watched, rating) (SVC-002)
6. API → Respond 201 Created with body { movieId, location }

Alternative Paths:
- AP1: Duplicate title → 409 Conflict with error code MOVIE_DUPLICATE (BR-001)
- AP2: Invalid rating format (decimal) → 400 Bad Request with validation error (BR-002)
- AP3: Missing authentication header → 401 Unauthorized

Error & Edge Cases:
- E1: Concurrent insert with same title → ensure unique constraint at DB; return 409
- E2: Genres array empty or includes unknown genre → accept free-text genres (no fixed taxonomy) for v1
- E3: Provided rating but watched=false or no watched flag → reject (400) per BR-004

Postconditions:
- Movie row exists in Movies table (Id, Title, Genres[])
- If user supplied watched/rating → WatchlistEntry exists for that user only

State Transitions:
- Movie: New -> Persisted
- WatchlistEntry: None -> Watched/Unwatched (per-user)

Acceptance Criteria:
- [ ] Given a valid unique title and genres, when POST /api/movies is called, then respond 201 and movie is persisted
- [ ] Given duplicate title, when POST /api/movies is called, then respond 409 and movie is not created
- [ ] Given a rating provided as decimal, when POST is called, then respond 400
- [ ] Given rating provided but user has not set watched=true, when POST is called, then respond 400

Touches:
- BR-001, BR-002, BR-003, BR-004
- SVC-001 (MoviesService), SVC-002 (WatchlistService)
- DF-001 (MoviesAddFlow)

Technical & Data Notes:
- DB: SQL Server
- Tables:
  - Movies (Id uniqueidentifier PK, Title nvarchar(255) UNIQUE CI, ReleaseDate date nullable, Description nvarchar(max) nullable, CreatedAt datetimeoffset)
  - MovieGenres (MovieId FK, Genre nvarchar(100)) — simple denormalized per-v1
  - WatchlistEntries (Id PK, UserId nvarchar(128), MovieId FK, Watched bit, Rating int nullable, WatchedAt datetimeoffset nullable, CreatedAt datetimeoffset)
- Constraints:
  - Movies.Title unique (case-insensitive)
  - Rating stored as INT; decimals not allowed (BR-002)
  - Rating cannot be set unless WatchlistEntry.Watched = true (BR-004)
- Indexes: Movies.Title (unique), MovieGenres.MovieId, WatchlistEntries.UserId + MovieId (unique per-user per-movie)
- API shape (request):
  POST /api/movies
  {
	"title": "string",
	"genres": ["Drama","Sci-Fi"],
	"watched": true,           // optional
	"rating": 8                // optional, integer only
  }
- API shape (response 201): { "movieId": "guid", "location": "/api/movies/{id}" }

Mock Data Examples:
1) Add movie minimal
{
  "title": "Blade Runner",
  "genres": ["Sci-Fi","Thriller"]
}

2) Add movie with watched/rating (creates WatchlistEntry for caller):
{
  "title": "Inception",
  "genres": ["Sci-Fi"],
  "watched": true,
  "rating": 9
}

Edge Cases to test:
- Concurrent adds with same title -> expect one success and others 409
- Adding with decimal rating -> 400
- Adding rating when watched=false -> 400

Security & Permissions Notes:
- Requires authentication header X-User-Id and X-User-Role (user|admin|readonly) (⚠ ASSUMPTION)
- Role `readonly` cannot POST; `user` and `admin` can add
- Do not log any sensitive headers or PII

Notes & Assumptions:
- ⚠ ASSUMPTION-001: Using header-based mock auth for v1. Replace with external IAM integration later.
- ⚠ ASSUMPTION-002: Genres are free-text for v1; later migrate to taxonomy service.
