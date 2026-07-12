# Implementation Plan — Movie CRUD (Slice B)

- **Story ID:** `01`
- **Story Name:** `Movie_CRUD`
- **منشأ PRD:** `AI/prd/01_Movie_CRUD.md`
- **وضعیت:** طراحی کامل — آماده اجرا
- **تاریخ تولید:** 2026-07-11

---

## ۰ — یافته‌های حیاتی فاز Pre-flight

> `⚠ ASSUMPTION-000` — **Repository کاملاً خالی است.** بررسى نشان داد:
> - فایل `MovieWatchlist.slnx` فقط `<Solution />` خالى است (هیچ پروژه‌ای ثبت نشده).
> - هیچ پوشه `src/`، هیچ فایل `*.csproj`، هیچ `Directory.Packages.props`، هیچ `Directory.Build.props` و هیچ `.editorconfig` وجود ندارد.
> - با وجود اینکه در backlog نوشته شده EPIC-00 «Done» است، در واقعیت هیچ foundation کدی ساخته نشده.
>
> **نتیجه:** این استوری علاوه بر چهار use case فیلم، باید **کل foundation پروژه** را هم بسازد (solution، package management، پروژه‌های base، composition root، persistence shell، shared kernel). این کار به‌عنوان تسک‌های foundation جدا در چک‌لیست (T-001 تا T-009) گنجانده شده است.

> `⚠ ASSUMPTION-001` — **ساختار پروژه‌ها:** با توجه به اینکه Repository خالى است و معماری به bounded contextها به‌صورت پروژه‌هاى جدا اشاره می‌کند، ساختار زیر برای این MVP انتخاب شد (کوچک‌ترین مجموعه که معماری را نقض نکند):
>
> ```text
> MovieWatchlist.slnx
> Directory.Packages.props
> Directory.Build.props
> .editorconfig
> src/
> ├── Api/                  # ASP.NET Core host: Program.cs, composition root, dev profile
> ├── SharedKernel/        # Result, Error, IClock, CorrelationId
> ├── Persistence/         # MovieWatchlistDbContext shell + DI registration
> ├── Migrations/          # EF Core migrations project (MovieWatchlist.Migrations)
> └── Execution/           # Bounded context: Movie vertical slices
> tests/
> ├── Architecture.Tests/  # Architecture consistency tests
> └── Execution.Tests/     # slice tests (unit + integration)
> ```
>
> دلیل: naming rule می‌گوید از prefix `MovieWatchlist.` پرهیز شود؛ پس نام‌ها ساده هستند. `Api/` نقش entry point را دارد. `Migrations/` نام پروژه `MovieWatchlist.Migrations` را در داخل خود نگه می‌دارد (طبق معماری). سایر bounded contextها (Definitions, Callbacks و...) در این استوری ساخته نمی‌شوند (Out of Scope).

---

## ۲.۱ — Business Rules

| ID | عنوان | شرح | نوع | لایه اعمال | وابسته به | منشأ |
|---|---|---|---|---|---|---|
| BR-001 | یکتایی عنوان فیلم | دو فیلم نباید عنوان یکسان (case-insensitive) داشته باشند. در SQL Server با default collation مقایسه `==` روی string به‌صورت case-insensitive انجام می‌شود. | Validation | BusinessActions (یکتایی) + DB unique index | — | PRD §5, §9.5 |
| BR-002 | اعتبار ژانر | `genre` باید عضو enum معتبر `MovieGenre` باشد. | Validation | Delivery (Validator) | — | PRD §9.6 |
| BR-003 | قوانین عنوان | `title` غیرخالى، پس از trim طول ۱ تا ۲۰۰ کاراکتر. | Validation | Delivery (Validator) | — | PRD §9.6 |
| BR-004 | پارامترهای صفحه‌بندی | `page` ≥ ۱؛ `pageSize` در بازه `[1, maxPageSize]`؛ در صورت نبود، defaults از Config اعمال می‌شود؛ اگر بزرگ‌تر از سقف باشد، سقف اعمال می‌شود. | Computation | BusinessActions (ResolvePaging) | — | PRD §9.5, §9.6 |
| BR-005 | ترتیب لیست | لیست فیلم‌ها بر اساس `CreatedAt` نزولی (جدیدترین اول). | Computation | BusinessActions (Query) | — | PRD §9.5 |
| BR-006 | موجود بودن فیلم برای ویرایش/حذف | اگر `movieId` وجود نداشت، `MovieNotFoundError` برگردانده شود (404). | State Transition | BusinessActions (Load) | — | PRD §8.2, §8.3 |
| BR-007 | کنترل دسترسی نقش‌ها | write (Add/Update/Delete) فقط برای Contributor/Admin؛ read (Get) برای همه. در این slice، Policyها تعریف اما در محیط Development موقتاً غیرفعال. | Authorization | Delivery (endpoint) | — | PRD §9.1, §9.7 |
| BR-008 | تولید شناسه | `Movie.Id` با `Guid.NewGuid()` تولید می‌شود. | Computation | Domain (factory) | — | PRD §5 |
| BR-009 | زمان ایجاد | `Movie.CreatedAt` از abstraction `IClock` تولید می‌شود، نه `DateTime.UtcNow` مستقیم. | Computation | Domain (factory) | — | PRD §9.5, Architecture |
| BR-010 | خطای typed به‌جای exception | خطاهاى قابل انتظار دامنه‌ای (تکرار عنوان، فیلم ناموجود) باید typed و با `Result<T>` برگردانده شوند، نه با exception. | Error Modeling | BusinessActions / Orchestrator | — | PRD §9.5, dotnet-error-handling |
| BR-011 | cancellation propagation | تمام عملیات async باید `CancellationToken` دریافت و عبور دهند. | Cross-cutting | همه لایه‌ها | — | PRD §9.5, dotnet-async |
| BR-012 | ممنوعیت null و List<T> | به‌جای null از `Result<T>` و به‌جای `List<T>`/`T[]` در APIهای داخلی از `IReadOnlyCollection<T>` استفاده شود. | Cross-cutting | همه لایه‌ها | — | Architecture, dotnet-async note |

---

## ۲.۲ — User Stories & Flows

### US-001: افزودن فیلم (AddMovie)

```md
US-001: افزودن فیلم

As a:        Contributor (یا Admin)
I want:      فیلم جدید با title و genre اضافه کنم
So that:     فیلم در کاتالوگ سراسری موجود شود

Preconditions:
- API host در حال اجراست و DB migrat.e شده است
- (در Dev) Policyها غیرفعال‌اند؛ در Production به Slice A متصل می‌شوند

Trigger:
- درخواست POST به endpoint افزودن فیلم با بدنه { title, genre }

Happy Path:
1. Client → AddMovieEndpoint : دریافت request (uses BR-011)
2. AddMovieEndpoint → AddMovieValidator : اعتبارسنجی ورودی (applies BR-002, BR-003)
3. AddMovieEndpoint → AddMovieOrchestrator : فراخوانی با command و ct (uses BR-011)
4. AddMovieOrchestrator → EnsureTitleUniqueAction : بررسی یکتایی عنوان (applies BR-001, BR-010)
5. AddMovieOrchestrator → Movie (Domain factory) : ساخت موجودیت با Id و CreatedAt (applies BR-008, BR-009)
6. AddMovieOrchestrator → PersistNewMovieAction : ذخیره در repository (uses BR-011)
7. AddMovieEndpoint → Client : 201 Created با response DTO

Alternative Paths:
- AP1: عنوان تکراری → EnsureTitleUniqueAction خروجی MovieAlreadyExistsError → Orchestrator آن را منتشر می‌کند → Endpoint 409 Conflict برمی‌گرداند (applies BR-010)

Error & Edge Cases:
- E1: ولیدیشن ورودی شکست خورد → 400 Bad Request با جزئیات (applies BR-002, BR-003)
- E2: خطای زیرساخت غیرمنتظره (مثلاً DB down) → 500 با ProblemDetails عمومی، بدون stack trace

Postconditions:
- فیلم جدید در جدول Movie ثبت شده با Id یکتا
- response شامل id, title, genre, createdAt است

State Transitions:
- (هیچ) → Movie موجود در سیستم

Acceptance Criteria:
- [ ] افزودن موفق 201 با id/title/genre/createdAt برمی‌گرداند
- [ ] عنوان تکراری (case-insensitive) → 409
- [ ] ورودی نامعتبر → 400
- [ ] CancellationToken در کل زنجیره عبور داده می‌شود

Touches:
- BR-001, BR-002, BR-003, BR-008, BR-009, BR-010, BR-011, BR-012
- SVC-001, SVC-002, SVC-003, SVC-004
- DF-001
```

### US-002: ویرایش فیلم (UpdateMovie)

```md
US-002: ویرایش فیلم

As a:        Contributor (یا Admin)
I want:      title و/یا genre یک فیلم موجود را ویرایش کنم
So that:     اطلاعات کاتالوگ به‌روز بماند

Preconditions:
- فیلم با movieId از قبل وجود داشته باشد

Trigger:
- درخواست PUT به endpoint ویرایش فیلم با movieId و بدنه { title?, genre? }

Happy Path:
1. Client → UpdateMovieEndpoint : دریافت request (uses BR-011)
2. UpdateMovieEndpoint → UpdateMovieValidator : اعتبارسنجی (applies BR-002, BR-003)
3. UpdateMovieEndpoint → UpdateMovieOrchestrator : فراخوانی با command و ct
4. UpdateMovieOrchestrator → LoadMovieAction : بارگذاری فیلم با movieId (applies BR-006, BR-010)
5. UpdateMovieOrchestrator → EnsureTitleUniqueAction : اگر title تغییر کرده، یکتایی بررسی شود، فیلم فعلی مستثنی (applies BR-001)
6. UpdateMovieOrchestrator → Movie.ApplyChanges : اعمال تغییرات روی موجودیت (domain method)
7. UpdateMovieOrchestrator → PersistMovieAction : ذخیره تغییرات (uses BR-011)
8. UpdateMovieEndpoint → Client : 200 OK با response DTO

Alternative Paths:
- AP1: فیلم یافت نشد → LoadMovieAction → MovieNotFoundError → 404 (applies BR-006, BR-010)
- AP2: title جدید با فیلم دیگری تداخل دارد → 409 (applies BR-001)

Error & Edge Cases:
- E1: ولیدیشن شکست → 400
- E2: خطای زیرساخت غیرمنتظره → 500 عمومی

Postconditions:
- فیلم با فیلدهای جدید به‌روز شده؛ createdAt تغییر نمی‌کند

State Transitions:
- Movie(موجود) → Movie(موجود، فیلدها تغییر کرده)

Acceptance Criteria:
- [ ] ویرایش موفق 200 با داده جدید
- [ ] فیلم ناموجود → 404
- [ ] تداخل عنوان با فیلم دیگر → 409
- [ ] createdAt پس از ویرایش ثابت می‌ماند

Touches:
- BR-001, BR-002, BR-003, BR-006, BR-010, BR-011
- SVC-005, SVC-006, SVC-007, SVC-008
- DF-002
```

### US-003: حذف فیلم (DeleteMovie)

```md
US-003: حذف فیلم

As a:        Contributor (یا Admin)
I want:      فیلمی را از کاتالوگ حذف کنم
So that:     کاتالوگ تمیز بماند

Preconditions:
- فیلم با movieId وجود داشته باشد

Trigger:
- درخواست DELETE به endpoint حذف فیلم با movieId

Happy Path:
1. Client → DeleteMovieEndpoint : دریافت request (uses BR-011)
2. DeleteMovieEndpoint → DeleteMovieOrchestrator : فراخوانی با movieId و ct
3. DeleteMovieOrchestrator → LoadMovieAction : بارگذاری برای اطمینان از وجود (applies BR-006, BR-010)
4. DeleteMovieOrchestrator → DeleteMovieAction : حذف از repository (uses BR-011)
5. DeleteMovieEndpoint → Client : 204 No Content

Alternative Paths:
- AP1: فیلم یافت نشد → LoadMovieAction → MovieNotFoundError → 404 (applies BR-006, BR-010)

Error & Edge Cases:
- E1: خطای زیرساخت غیرمنتظره → 500 عمومی
- E2: cancellation حین حذف → بدون رکورد ناقص مدیریت شود

Postconditions:
- فیلم از جدول Movie حذف شده

State Transitions:
- Movie(موجود) → (حذف شده)

Acceptance Criteria:
- [ ] حذف موفق → 204
- [ ] فیلم ناموجود → 404
- [ ] cancellation به‌درستی propagate می‌شود

Touches:
- BR-006, BR-010, BR-011
- SVC-009, SVC-010
- DF-003
```

### US-004: مشاهده لیست فیلم‌ها (GetMovies)

```md
US-004: مشاهده لیست فیلم‌ها

As a:        Viewer (یا هر کاربر authenticated)
I want:      لیست فیلم‌ها را با صفحه‌بندی ببینم
So that:     از فیلم‌های موجود آگاه شوم

Preconditions:
- DB migrat.e شده باشد

Trigger:
- درخواست GET به endpoint لیست با query params { page?, pageSize? }

Happy Path:
1. Client → GetMoviesEndpoint : دریافت request (uses BR-011)
2. GetMoviesEndpoint → GetMoviesOrchestrator : فراخوانی با پارامترهای خام و ct
3. GetMoviesOrchestrator → ResolvePagingParametersAction : اعمال defaults و سقف (applies BR-004)
4. GetMoviesOrchestrator → QueryMoviesAction : query با AsNoTracking، مرتب CreatedAt نزولی، skip/take (applies BR-005, BR-012)
5. GetMoviesEndpoint → Client : 200 OK با { items, page, pageSize, totalCount }

Alternative Paths:
- AP1: page/pageSize نامعتبر یا غایب → ResolvePaging مقادیر Config را به‌عنوان Fallback اعمال می‌کند (applies BR-004)
- AP2: page بزرگ‌تر از تعداد صفحات موجود → items خالی، totalCount صحیح (نه خطا)

Error & Edge Cases:
- E1: خطای زیرساخت غیرمنتظره → 500 عمومی
- E2: لیست خالی → items به‌صورت آرایه خالی [] برگردد، نه null (applies BR-012)

Postconditions:
- هیچ تغییری در داده ایجاد نمی‌شود (read-only)

State Transitions:
- (بدون تغییر حالت)

Acceptance Criteria:
- [ ] دریافت لیست با pagination → 200 با items + page + pageSize + totalCount
- [ ] لیست خالی → 200 با items: []
- [ ] page/pageSize غایب → defaults از Config اعمال شود
- [ ] page بزرگ‌تر از موجود → items خالی، totalCount صحیح
- [ ] مرتب بر اساس جدیدترین CreatedAt

Touches:
- BR-004, BR-005, BR-010, BR-011, BR-012
- SVC-011, SVC-012
- DF-004
```

---

## ۲.۳ — Services & Interactions

| ID | نام | مسئولیت | لایه طبق معماری | وابستگی‌ها | الگوی ارتباط | Owner of State |
|---|---|---|---|---|---|---|
| SVC-001 | `AddMovieEndpoint` | دریافت HTTP، فراخوانی validator و orchestrator، نگاشت response | Delivery | AddMovieValidator, AddMovieOrchestrator | Sync | No |
| SVC-002 | `AddMovieValidator` | اعتبارسنجی request (title, genre) | Delivery | — | Sync | No |
| SVC-003 | `AddMovieOrchestrator` | هماهنگ‌سازی flow افزودن فیلم | Workflow | EnsureTitleUniqueAction, PersistNewMovieAction | Sync | No |
| SVC-004 | `EnsureTitleUniqueAction` | بررسی یکتایی عنوان فیلم | BusinessActions | IMovieRepository | Sync | No |
| SVC-K1 | `PersistNewMovieAction` | ذخیره فیلم جدید | BusinessActions | IMovieRepository | Sync | No |
| SVC-005 | `UpdateMovieEndpoint` | دریافت HTTP، فراخوانی validator و orchestrator | Delivery | UpdateMovieValidator, UpdateMovieOrchestrator | Sync | No |
| SVC-006 | `UpdateMovieValidator` | اعتبارسنجی request ویرایش | Delivery | — | Sync | No |
| SVC-007 | `UpdateMovieOrchestrator` | هماهنگ‌سازی flow ویرایش | Workflow | LoadMovieAction, EnsureTitleUniqueAction, PersistMovieAction | Sync | No |
| SVC-008 | `LoadMovieAction` | بارگذاری فیلم با id | BusinessActions | IMovieRepository | Sync | No |
| SVC-K2 | `PersistMovieAction` | ذخیره تغییرات فیلم | BusinessActions | IMovieRepository | Sync | No |
| SVC-009 | `DeleteMovieEndpoint` | دریافت HTTP، فراخوانی orchestrator | Delivery | DeleteMovieOrchestrator | Sync | No |
| SVC-010 | `DeleteMovieOrchestrator` | هماهنگ‌سازی flow حذف | Workflow | LoadMovieAction, DeleteMovieAction | Sync | No |
| SVC-K3 | `DeleteMovieAction` | حذف فیلم از repository | BusinessActions | IMovieRepository | Sync | No |
| SVC-011 | `GetMoviesEndpoint` | دریافت HTTP، فراخوانی orchestrator، نگاشت response | Delivery | GetMoviesOrchestrator | Sync | No |
| SVC-012 | `GetMoviesOrchestrator` | هماهنگ‌سازی flow دریافت لیست | Workflow | ResolvePagingParametersAction, QueryMoviesAction | Sync | No |
| SVC-K4 | `ResolvePagingParametersAction` | اعمال defaults و سقف page/pageSize | BusinessActions | IOptions (Config) | Sync | No |
| SVC-K5 | `QueryMoviesAction` | query فیلم‌ها با sort + pagination | BusinessActions | IMovieRepository | Sync | No |
| SVC-D1 | `Movie` (Aggregate/Entity) | موجودیت دامنه: Id, Title, Genre, CreatedAt + factory + ApplyChanges | Domain (Entities) | IClock | Sync | Yes (in-memory) |
| SVC-D2 | `IMovieRepository` | interface repository برای Movie (read/write) | Domain (Repositories) | — | Sync | No |
| SVC-D3 | `MovieRepository` | پیاده‌سازی EF Core repository | Infrastructure | MovieWatchlistDbContext | Sync | No |
| SVC-D4 | `Result<T>`, `Error` | انواع نتیجه و خطای typed | SharedKernel | — | Sync | No |
| SVC-D5 | `IClock` | abstraction زمان | SharedKernel | — | Sync | No |
| SVC-D6 | `MovieGenre` (enum) | ژانرهای فیلم | Domain (ValueObjects) | — | Sync | No |
| SVC-D7 | typed errors | `MovieAlreadyExistsError`, `MovieNotFoundError` | Domain (ValueObjects) | — | Sync | No |

### Contract نمونه (AddMovieOrchestrator)

```md
SVC-003 Contract (AddMovieOrchestrator):
  Inputs:
    - AddMovieCommand { Title: string, Genre: MovieGenre }
    - CancellationToken ct
  Outputs:
    - Result<AddMovieResult> که AddMovieResult { Id, Title, Genre, CreatedAt }
  Errors:
    - MovieAlreadyExistsError
  Side Effects:
    - درج رکورد در جدول Movie (از طریق repository)
  Invariants:
    - قبل از ذخیره، یکتایی عنوان بررسی می‌شود
    - Id و CreatedAt از domain factory تولید می‌شوند، نه از ورودی کلاینت
    - CancellationToken عبور داده می‌شود
```

### نمودار متنی تعامل (AddMovie)

```text
Interaction: AddMovie
  US-001 → AddMovieEndpoint
       → AddMovieValidator.Validate(request)
       → AddMovieOrchestrator.Execute(command, ct)
              → EnsureTitleUniqueAction.Execute(title, ct)   [SVC-004]
              → Movie.Create(title, genre, clock)            [SVC-D1]
              → PersistNewMovieAction.Execute(movie, ct)     [SVC-K1]
       ← Result<AddMovieResult> / MovieAlreadyExistsError
  Endpoint maps Result → 201 / 409 / 400
```

---

## ۲.۴ — Data Flow & Decision Points

### DF-001: AddMovie

```md
DF-001: AddMovie
  Source: HTTP Client (body { title, genre })
  Sink:   جدول Movie + HTTP 201 response
  Stages:
    Stage 1: { title, genre } → AddMovieValidator (applies BR-002, BR-003)
    Stage 2: valid request → نگاشت به AddMovieCommand
    Stage 3: command → AddMovieOrchestrator
    Stage 4: → EnsureTitleUniqueAction (applies BR-001 at DP-001)
    Stage 5: → Movie.Create (applies BR-008, BR-009)
    Stage 6: → PersistNewMovieAction → DB insert
    Stage 7: → response DTO → 201
  Decision Points:
    DP-001: title قبلاً وجود دارد؟ → بله: MovieAlreadyExistsError (409) / خیر: ادامه (References BR-001)
  State Ownership: DB (جدول Movie)
  Validation Points:
    Input Validation:  AddMovieValidator (BR-002, BR-003)
    Domain Validation: EnsureTitleUniqueAction (BR-001)
    Authorization Check: endpoint Policy (BR-007) — غیرفعال در Dev
  Sensitive Data: ندارد (title, genre غیرحساس)
  Masking: N/A
  Caching: N/A
  Errors: MovieAlreadyExistsError, ValidationError, InfrastructureError
```

### DF-002: UpdateMovie

```md
DF-002: UpdateMovie
  Source: HTTP Client (movieId in route, body { title?, genre? })
  Sink:   جدول Movie + HTTP 200/404/409
  Stages:
    Stage 1: { movieId, title?, genre? } → UpdateMovieValidator (applies BR-002, BR-003)
    Stage 2: → UpdateMovieOrchestrator
    Stage 3: → LoadMovieAction (applies BR-006 at DP-002)
    Stage 4: اگر title تغییر کرده → EnsureTitleUniqueAction با استثنای id فعلی (applies BR-001 at DP-003)
    Stage 5: → Movie.ApplyChanges (domain method)
    Stage 6: → PersistMovieAction → DB update
    Stage 7: → response DTO → 200
  Decision Points:
    DP-002: فیلم با movieId وجود دارد؟ → خیر: MovieNotFoundError (404) / بله: ادامه (References BR-006)
    DP-003: title جدید با فیلم دیگری تداخل دارد؟ → بله: MovieAlreadyExistsError (409) / خیر: ادامه (References BR-001)
  State Ownership: DB (جدول Movie)
  Validation Points:
    Input Validation:  UpdateMovieValidator
    Domain Validation: LoadMovieAction, EnsureTitleUniqueAction
    Authorization Check: endpoint Policy (BR-007) — غیرفعال در Dev
  Sensitive Data: ندارد
  Errors: MovieNotFoundError, MovieAlreadyExistsError, ValidationError
```

### DF-003: DeleteMovie

```md
DF-003: DeleteMovie
  Source: HTTP Client (movieId in route)
  Sink:   جدول Movie + HTTP 204/404
  Stages:
    Stage 1: { movieId } → DeleteMovieEndpoint
    Stage 2: → DeleteMovieOrchestrator
    Stage 3: → LoadMovieAction (applies BR-006 at DP-004)
    Stage 4: → DeleteMovieAction → DB delete
    Stage 5: → 204
  Decision Points:
    DP-004: فیلم وجود دارد؟ → خیر: MovieNotFoundError (404) / بله: ادامه (References BR-006)
  State Ownership: DB (جدول Movie)
  Validation Points:
    Input Validation: route movieId معتبر (Guid)
    Authorization Check: endpoint Policy (BR-007) — غیرفعال در Dev
  Sensitive Data: ندارد
  Errors: MovieNotFoundError, InfrastructureError
```

### DF-004: GetMovies

```md
DF-004: GetMovies
  Source: HTTP Client (query { page?, pageSize? })
  Sink:   HTTP 200 با { items, page, pageSize, totalCount }
  Stages:
    Stage 1: { page?, pageSize? } → GetMoviesEndpoint
    Stage 2: → GetMoviesOrchestrator
    Stage 3: → ResolvePagingParametersAction (applies BR-004 at DP-005)
    Stage 4: → QueryMoviesAction (applies BR-005, BR-012) با AsNoTracking
    Stage 5: → response DTO → 200
  Decision Points:
    DP-005: page/pageSize غایب یا خارج بازه؟ → اعمال defaults/سقف از Config (References BR-004)
  State Ownership: DB (read-only)
  Validation Points:
    Input Validation: page/pageSize عدد صحیح (model binding)
    Authorization Check: endpoint Policy (BR-007) — غیرفعال در Dev
  Sensitive Data: ندارد
  Errors: InfrastructureError
  Notes: items هرگز null نیست؛ آرایه خالی در صورت نبود داده (applies BR-012)
```

---

## ۲.۵ — File & Folder Plan

> قانون naming: namespace باید با مسیر پوشه هم‌راستا باشد و از prefix `MovieWatchlist.` پرهیز شود.

### Foundation

| File | Purpose | Owner BC | Owner Slice | Rules |
|---|---|---|---|---|
| `MovieWatchlist.slnx` | solution (به‌جای خالی، پروژه‌ها ثبت شوند) | — | — | dotnet-tooling |
| `Directory.Packages.props` | Central Package Management — نسخه‌ها | — | — | dotnet-tooling |
| `Directory.Build.props` | تنظیمات مشترک build (TargetFramework, Nullable, ImplicitUsings, LangVersion) | — | — | dotnet-tooling |
| `.editorconfig` | فرمت‌بندی یکنواخت | — | — | dotnet-tooling |
| `src/Api/Api.csproj` | Web host entry point | — | — | dotnet-structure |
| `src/Api/Program.cs` | composition root، DI registration، dev pipeline | — | — | dotnet-di, dotnet-structure |
| `src/Api/appsettings.json` | Config (defaultPageSize, maxPageSize) | — | — | dotnet-security |
| `src/Api/appsettings.Development.json` | dev profile overrides | — | — | dotnet-security |
| `src/SharedKernel/SharedKernel.csproj` | shared kernel | SharedKernel | — | dotnet-structure |
| `src/SharedKernel/Domain/ValueObjects/Result.cs` | `Result<T>`, `Result` | SharedKernel | — | dotnet-error-handling |
| `src/SharedKernel/Domain/ValueObjects/Error.cs` | نوع پایه `Error` | SharedKernel | — | dotnet-error-handling |
| `src/SharedKernel/Domain/Services/IClock.cs` | abstraction زمان | SharedKernel | — | dotnet-solid |
| `src/SharedKernel/Domain/Services/SystemClock.cs` | پیاده‌سازی IClock | SharedKernel | — | dotnet-solid |
| `src/Persistence/Persistence.csproj` | EF Core infrastructure | Persistence | — | dotnet-structure |
| `src/Persistence/MovieWatchlistDbContext.cs` | DbContext shell | Persistence | — | dotnet-performance |
| `src/Persistence/DependencyInjection.cs` | `AddMovieWatchlistPersistence` | Persistence | — | dotnet-di |
| `src/Persistence/Configurations/MovieConfiguration.cs` | EF mapping + unique index روی Title | Persistence | — | dotnet-performance |
| `src/Migrations/Migrations.csproj` | migrations project (نام: MovieWatchlist.Migrations) | Migrations | — | dotnet-tooling |
| `src/Execution/Execution.csproj` | bounded context فیلم | Execution | — | dotnet-structure |

### Vertical Slices (هر کدام end-to-end)

| File | Purpose | Owner BC | Owner Slice | Rules |
|---|---|---|---|---|
| `src/Execution/Domain/Entities/Movie.cs` | entity Movie + factory + ApplyChanges | Execution | (shared domain) | dotnet-solid, dotnet-clean-code |
| `src/Execution/Domain/ValueObjects/MovieGenre.cs` | enum ژانر | Execution | (shared domain) | dotnet-naming |
| `src/Execution/Domain/ValueObjects/MovieErrors.cs` | MovieAlreadyExistsError, MovieNotFoundError | Execution | (shared domain) | dotnet-error-handling |
| `src/Execution/Domain/Repositories/IMovieRepository.cs` | repository interface | Execution | (shared domain) | dotnet-solid |
| `src/Execution/Infrastructure/MovieRepository.cs` | EF Core implementation | Execution | (shared infra) | dotnet-performance |
| `src/Execution/AddMovie/Delivery/AddMovieEndpoint.cs` | endpoint | Execution | AddMovie | dotnet-api-documentation, dotnet-structure |
| `src/Execution/AddMovie/Delivery/AddMovieRequest.cs` | request DTO | Execution | AddMovie | dotnet-structure |
| `src/Execution/AddMovie/Delivery/AddMovieResponse.cs` | response DTO | Execution | AddMovie | dotnet-api-documentation |
| `src/Execution/AddMovie/Delivery/AddMovieValidator.cs` | validator | Execution | AddMovie | dotnet-error-handling |
| `src/Execution/AddMovie/Workflow/AddMovieOrchestrator.cs` | orchestrator | Execution | AddMovie | dotnet-solid, dotnet-async |
| `src/Execution/AddMovie/Domain/ValueObjects/AddMovieCommand.cs` | command | Execution | AddMovie | dotnet-structure |
| `src/Execution/AddMovie/Domain/ValueObjects/AddMovieResult.cs` | result دامنه‌ای | Execution | AddMovie | dotnet-structure |
| `src/Execution/AddMovie/BusinessActions/EnsureTitleUniqueAction.cs` | action یکتایی | Execution | AddMovie | dotnet-solid, dotnet-async |
| `src/Execution/AddMovie/BusinessActions/PersistNewMovieAction.cs` | action ذخیره | Execution | AddMovie | dotnet-solid, dotnet-async |
| `src/Execution/UpdateMovie/Delivery/UpdateMovieEndpoint.cs` | endpoint | Execution | UpdateMovie | dotnet-api-documentation |
| `src/Execution/UpdateMovie/Delivery/UpdateMovieRequest.cs` | request DTO | Execution | UpdateMovie | dotnet-structure |
| `src/Execution/UpdateMovie/Delivery/UpdateMovieResponse.cs` | response DTO | Execution | UpdateMovie | dotnet-api-documentation |
| `src/Execution/UpdateMovie/Delivery/UpdateMovieValidator.cs` | validator | Execution | UpdateMovie | dotnet-error-handling |
| `src/Execution/UpdateMovie/Workflow/UpdateMovieOrchestrator.cs` | orchestrator | Execution | UpdateMovie | dotnet-solid, dotnet-async |
| `src/Execution/UpdateMovie/Domain/ValueObjects/UpdateMovieCommand.cs` | command | Execution | UpdateMovie | dotnet-structure |
| `src/Execution/UpdateMovie/BusinessActions/LoadMovieAction.cs` | action بارگذاری | Execution | UpdateMovie | dotnet-solid, dotnet-async |
| `src/Execution/UpdateMovie/BusinessActions/PersistMovieAction.cs` | action ذخیره تغییرات | Execution | UpdateMovie | dotnet-solid, dotnet-async |
| `src/Execution/DeleteMovie/Delivery/DeleteMovieEndpoint.cs` | endpoint | Execution | DeleteMovie | dotnet-api-documentation |
| `src/Execution/DeleteMovie/Workflow/DeleteMovieOrchestrator.cs` | orchestrator | Execution | DeleteMovie | dotnet-solid, dotnet-async |
| `src/Execution/DeleteMovie/BusinessActions/DeleteMovieAction.cs` | action حذف | Execution | DeleteMovie | dotnet-solid, dotnet-async |
| `src/Execution/GetMovies/Delivery/GetMoviesEndpoint.cs` | endpoint | Execution | GetMovies | dotnet-api-documentation |
| `src/Execution/GetMovies/Delivery/GetMoviesResponse.cs` | response DTO (items+page+pageSize+totalCount) | Execution | GetMovies | dotnet-api-documentation |
| `src/Execution/GetMovies/Workflow/GetMoviesOrchestrator.cs` | orchestrator | Execution | GetMovies | dotnet-solid, dotnet-async |
| `src/Execution/GetMovies/Domain/ValueObjects/PagingParameters.cs` | value object paging | Execution | GetMovies | dotnet-structure |
| `src/Execution/GetMovies/BusinessActions/ResolvePagingParametersAction.cs` | action resolve paging | Execution | GetMovies | dotnet-solid |
| `src/Execution/GetMovies/BusinessActions/QueryMoviesAction.cs` | action query | Execution | GetMovies | dotnet-performance, dotnet-async |

### Tests

| File | Purpose | Rules |
|---|---|---|
| `tests/Architecture.Tests/Architecture.Tests.csproj` | architecture consistency tests | dotnet-testing |
| `tests/Architecture.Tests/NamespaceTests.cs` | namespace = folder | dotnet-testing |
| `tests/Execution.Tests/Execution.Tests.csproj` | slice tests | dotnet-testing |
| `tests/Execution.Tests/AddMovie/AddMovieOrchestratorTests.cs` | unit tests orchestrator | dotnet-testing |
| `tests/Execution.Tests/GetMovies/QueryMoviesActionTests.cs` | unit tests query + paging | dotnet-testing |
| `tests/Execution.Tests/Integration/MovieEndpointsTests.cs` | integration test 4 endpoints | dotnet-testing |

الزامات:
- مسیرها با معماری سازگارند (Screaming + Vertical Slice).
- داخل هر slice، پوشه `Delivery/`, `Workflow/`, `BusinessActions/`, `Domain/`, `Infrastructure/` فقط در صورت نیاز استفاده می‌شوند.
- فایل‌های Domain فقط در شش زیرپوشه مجاز قرار می‌گیرند.
- پوشه‌های `Common`, `Utils`, `Helpers` ساخته نمی‌شوند.

---

## ۲.۶ — Coverage Matrix

| نیازمندی PRD/Story | BR | US | SVC | DF | Task |
|---|---|---|---|---|---|
| Foundation (solution, props, projects) | — | — | — | — | T-001..T-009 |
| SharedKernel (Result, Error, IClock) | BR-010 | — | SVC-D4, D5 | — | T-004 |
| Persistence (DbContext, MovieConfiguration, DI) | BR-001 (unique index) | — | SVC-D3 | — | T-006 |
| Migration Movie | — | — | — | — | T-009 |
| افزودن فیلم موفق | BR-001,002,003,008,009 | US-001 | SVC-001..004, K1 | DF-001 | T-010 |
| عنوان تکراری → 409 | BR-001,010 | US-001 AP1 | SVC-004 | DF-001 DP-001 | T-010 |
| ورودی نامعتبر → 400 | BR-002,003 | US-001 E1 | SVC-002 | DF-001 | T-010 |
| ویرایش فیلم موفق | BR-001,006 | US-002 | SVC-005..008, K2 | DF-002 | T-011 |
| ویرایش: فیلم ناموجود → 404 | BR-006,010 | US-002 AP1 | SVC-008 | DF-002 DP-002 | T-011 |
| ویرایش: تداخل عنوان → 409 | BR-001 | US-002 AP2 | SVC-004 | DF-002 DP-003 | T-011 |
| حذف فیلم موفق → 204 | BR-006 | US-003 | SVC-009,010, K3 | DF-003 | T-012 |
| حذف: فیلم ناموجود → 404 | BR-006,010 | US-003 AP1 | SVC-008 | DF-003 DP-004 | T-012 |
| لیست با pagination → 200 | BR-004,005 | US-004 | SVC-011,012, K4, K5 | DF-004 | T-013 |
| لیست خالی → items:[] | BR-012 | US-004 E2 | SVC-012 | DF-004 | T-013 |
| defaults از Config | BR-004 | US-004 AP1 | SVC-K4 | DF-004 DP-005 | T-013 |
| CancellationToken propagation | BR-011 | همه | همه | همه | T-010..T-013 |
| Policy/Role authorization (غیرفعال در Dev) | BR-007 | همه | endpoints | همه | T-014 |
| Config (defaultPageSize, maxPageSize) | BR-004 | US-004 | SVC-K4 | DF-004 | T-015 |
| dev profile + secret.json placeholder | — | — | — | — | T-015 |
| NuGet management (EF Core, Design) | — | — | — | — | T-016 |
| Tests (unit + integration) | — | همه | — | — | T-017, T-018 |
| Auth واقعی (Slice A) | — | — | — | — | Out-of-Scope (OOS-001) |
| Watchlist/Rating (Slice C) | — | — | — | — | Out-of-Scope (OOS-002) |
| averageRating/watchlistCount fields | — | — | — | — | Out-of-Scope (OOS-002) |
| search/filter/sort پیشرفته | — | — | — | — | Out-of-Scope (OOS-003) |

---

## ۲.۷ — Standards Mapping

| حوزه / لایه | فایل‌های قانون در `AI/rules/` | الزامات کلیدی | اعمال در |
|---|---|---|---|
| همه فایل‌های .cs | `dotnet-tooling` (`alwaysApply: true`) | editorconfig، dotnet format، analyzerها | همه تسک‌ها |
| Delivery (endpoints) | `dotnet-structure`, `dotnet-api-documentation`, `dotnet-error-handling` | file-scoped namespace، OpenAPI کامل، ProblemDetails، CorrelationId | T-010..T-014 |
| Workflow (orchestrators) | `dotnet-solid`, `dotnet-async`, `dotnet-clean-code` | SRP، Orchestrator فقط هماهنگ‌کننده، Async + CT، متد ≤ ۲۰-۳۰ خط | T-010..T-013 |
| BusinessActions | `dotnet-solid`, `dotnet-async`, `dotnet-error-handling` | SRP، Result pattern، CT، typed errors | T-010..T-013 |
| Domain | `dotnet-solid`, `dotnet-naming` | SRP، enum PascalCase، factory برای invariant | T-005, T-008 |
| Infrastructure (repository) | `dotnet-di`, `dotnet-performance`, `dotnet-security` | تزریق abstraction، AsNoTracking، parameterized query | T-006, T-008 |
| Persistence | `dotnet-di`, `dotnet-performance` | DI registration، unique index | T-006 |
| Tests | `dotnet-testing` | AAA، `Method_Scenario_Expected`، mock برای وابستگی | T-017, T-018 |
| Naming (همه) | `dotnet-naming` | PascalCase، _camelCase، interface با I، Async suffix | همه تسک‌ها |
| Tooling/NuGet | `dotnet-tooling` | Central Package Management، dotnet format | T-001, T-016 |

الزامات:
- قوانین `alwaysApply: true` (فقط `dotnet-tooling`) همیشه اعمال می‌شوند.
- سایر قوانین با `globs: **/*.cs` برای همه فایل‌های C# اعمال می‌شوند.
- `dotnet-api-documentation` با globs خاص (`**/Delivery/*Endpoint*.cs`) فقط برای endpointها.
- `dotnet-testing` با globs (`**/*Tests.cs, **/*Specs.cs`) فقط برای فایل‌های تست.
- هیچ rule exception برای داده حساس یا امنیت مجاز نیست.

---

## ۲.۸ — Open Decisions & Assumptions

### Assumptions

- `⚠ ASSUMPTION-000` — Repository کاملاً خالی است؛ این استوری کل foundation را هم می‌سازد.
- `⚠ ASSUMPTION-001` — ساختار پروژه‌ها طبق §0 انتخاب شد (Api, SharedKernel, Persistence, Migrations, Execution).
- `⚠ ASSUMPTION-002` — یکتایی عنوان case-insensitive با تکیه بر default collation SQL Server (case-insensitive) و یک unique index روی ستون Title پیاده می‌شود. اعتبارسنجی application layer با `==` انجام می‌شود که در SQL Server case-insensitive translate می‌شود.
- `⚠ ASSUMPTION-003` — Policyهای authorization در این slice تعریف می‌شوند ولی در محیط Development موقتاً غیرفعال‌اند (چون Slice A هنوز موجود نیست). هنگام ساخت Slice A فعال می‌شوند.
- `⚠ ASSUMPTION-004` — connection string از `secret.json` خوانده می‌شود؛ کاربر بعداً محتوای واقعی را اضافه می‌کند. فعلاً فقط placeholder و dev profile آماده می‌شود.
- `⚠ ASSUMPTION-005` — فیلدهای `averageRating` و `watchlistCount` در این slice ساخته نمی‌شوند (Slice C).
- `⚠ ASSUMPTION-006` — EF Core auto-migrate در API غیرفعال است؛ migrationها دستی از پروژه Migrations اجرا می‌شوند.
- `⚠ ASSUMPTION-007` — اسکریپت sync آفلاین پکیج‌ها در Repository فعلی موجود نیست؛ به‌عنوان Open Item ثبت می‌شود و استوری بدون ادعای sync آفلاین Done اعلام نمی‌شود.

### Open Decisions

- `OD-001` — مسیر دقیق endpointها (Routes) نیازمند سند طراحی تأییدشده است. در این slice با قرارداد معقول (مثلاً `/movies`) ادامه داده می‌شود.
- `OD-002` — نسخه‌های دقیق پکیج‌های NuGet باید در فاز اجرا تعیین شوند (آخرین stable سازگار با TargetFramework).

### Rule Exceptions

- (هیچ) — هیچ rule exception مجاز نیست.

### Out-of-Scope Conflicts

- `OOS-001` — Auth واقعی و مدیریت کاربر → Slice A (بعداً).
- `OOS-002` — Watchlist شخصی، Rating، و فیلدهای مشتق‌شده (`averageRating`, `watchlistCount`) → Slice C (بعداً).
- `OOS-003` — جستجو، فیلتر و مرتب‌سازی پیشرفته → استوری جدا.
- `OOS-004` — UI مستقل / داشبورد → خارج از دامنه API.
- `OOS-005` — IAM داخلی، Central Logging داخلی، Message Broker داخلی، Workflow Engine داخلی → ممنوع طبق معماری.
- `OOS-006` — Manual Review به‌عنوان قابلیت محصولی → ممنوع طبق معماری.
