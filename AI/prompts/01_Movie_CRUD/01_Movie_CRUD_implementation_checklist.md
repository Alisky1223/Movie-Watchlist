# Implementation Checklist — Movie CRUD (Slice B)

- **Story ID:** `01`
- **Story Name:** `Movie_CRUD`
- **منشأ:** `AI/prompts/01_Movie_CRUD/01_Movie_CRUD_implementation_plan.md`
- **قانون:** این چک‌لیست از پلن فاز ۲ مشتق شده است. هر تسک atomic است و ترتیب بر اساس وابستگی (Topological Order) است.

> **نکته مهم برای اجرا:** Repository فعلی کاملاً خالی است (`⚠ ASSUMPTION-000`). تسک‌های T-001 تا T-009 ابتدا foundation را می‌سازند، سپس T-010 تا T-018 vertical sliceها و تست‌ها پیاده می‌شوند. هر تسک را فقط پس از انجام همه وابستگی‌هایش اجرا کنید.

---

## Phase A — Foundation (T-001 تا T-009)

### T-001: راه‌اندازی Solution و Central Package Management
```md
- [ ] T-001: راه‌اندازی Solution و Central Package Management
      Implements:    Foundation
      Depends on:    None
      Files:
        - MovieWatchlist.slnx
        - Directory.Packages.props
        - Directory.Build.props
        - .editorconfig
      Rules:
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - Directory.Packages.props با <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally> فعال است
        - Directory.Build.props شامل: TargetFramework (net9.0 یا net8.0 طبق SDK)، Nullable enable، ImplicitUsings enable، LangVersion latest
        - .editorconfig با تنظیمات base .NET (indent_style، naming، و غیره) ایجاد شده
        - MovieWatchlist.slnx از نوع solution معتبر است (پروژه‌ها در تسک‌های بعدی اضافه می‌شوند)
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - فایل‌ها بدون خطای syntax هستند
        - dotnet tooling قابل اجراست
```

### T-002: ساخت پروژه Api (Web host)
```md
- [ ] T-002: ساخت پروژه Api (Web host entry point)
      Implements:    Foundation
      Depends on:    T-001
      Files:
        - src/Api/Api.csproj
        - src/Api/Program.cs (نسخه اولیه Minimal API با Health placeholder)
      Rules:
        - dotnet-structure
        - dotnet-tooling (alwaysApply)
        - dotnet-naming
      Acceptance:
        - Api.csproj به‌صورت Web project (Microsoft.NET.Sdk.Web) ایجاد شده
        - namespace file-scoped و هم‌راستا با پوشه است
        - Program.cs Minimal API ساده با /health/live و /health/ready دارد (بدون وابستگی به DB طبق معماری)
        - پروژه به MovieWatchlist.slnx اضافه شده
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build src/Api/Api.csproj موفق است
```

### T-003: ساخت پروژه SharedKernel
```md
- [ ] T-003: ساخت پروژه SharedKernel
      Implements:    Foundation
      Depends on:    T-001
      Files:
        - src/SharedKernel/SharedKernel.csproj
      Rules:
        - dotnet-structure
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - SharedKernel.csproj به‌صورت classlib ایجاد شده
        - به MovieWatchlist.slnx اضافه شده
        - هیچ وابستگی به framework دامنه ندارد (فقط base BCL)
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build src/SharedKernel/SharedKernel.csproj موفق است
```

### T-004: پیاده‌سازی Result/Error/IClock در SharedKernel
```md
- [ ] T-004: پیاده‌سازی Result, Error و IClock در SharedKernel
      Implements:    BR-010; SVC-D4, SVC-D5
      Depends on:    T-003
      Files:
        - src/SharedKernel/Domain/ValueObjects/Result.cs
        - src/SharedKernel/Domain/ValueObjects/Error.cs
        - src/SharedKernel/Domain/Services/IClock.cs
        - src/SharedKernel/Domain/Services/SystemClock.cs
      Rules:
        - dotnet-solid
        - dotnet-error-handling
        - dotnet-structure
        - dotnet-naming
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - Result<T> و Result (non-generic) با متدهای Success/Failure وجود دارند
        - Error به‌صورت abstract/record base با Code و Message تعریف شده
        - IClock یک abstraction (interface) با متد گرفتن DateTimeOffset.UtcNow است
        - SystemClock پیاده‌سازی IClock است
        - فایل‌های Domain فقط در زیرپوشه‌های مجاز (ValueObjects/Services) قرار دارند
        - هیچ متدی null برنمی‌گرداند (BR-012)
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است
        - بازبینی دستی در برابر فایل‌های Rules مرتبط
```

### T-005: ساخت پروژه Persistence و DbContext shell
```md
- [ ] T-005: ساخت پروژه Persistence و DbContext shell
      Implements:    Foundation
      Depends on:    T-001
      Files:
        - src/Persistence/Persistence.csproj
        - src/Persistence/MovieWatchlistDbContext.cs
        - src/Persistence/DependencyInjection.cs (AddMovieWatchlistPersistence)
      Rules:
        - dotnet-structure
        - dotnet-di
        - dotnet-tooling (alwaysApply)
        - dotnet-naming
      Acceptance:
        - Persistence.csproj به‌صورت classlib و ارجاع به EF Core SqlServer + EF Core Design دارد (بدون Version)
        - MovieWatchlistDbContext یک DbSet<Movie> دارد (پس از T-008 کامل می‌شود؛ فعلاً shell)
        - متد الحاقی AddMovieWatchlistPersistence(IServiceCollection, string) برای ثبت DbContext با SQL Server وجود دارد
        - API خودکار migrate نمی‌کند (⚠ ASSUMPTION-006)
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است (پس از T-016 برای NuGet)
      Notes:
        - این تسک نیازمند NuGet است؛ اگر پکیج اضافه شد، T-016 را هم در همان حلقه اجرا کن
```

### T-006: EF mapping و unique index برای Movie
```md
- [ ] T-006: EF mapping و unique index برای Movie
      Implements:    BR-001; SVC-D3
      Depends on:    T-005, T-008 (Movie entity)
      Files:
        - src/Persistence/Configurations/MovieConfiguration.cs
        - src/Persistence/MovieWatchlistDbContext.cs (complete)
      Rules:
        - dotnet-performance
        - dotnet-structure
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - MovieConfiguration از IEntityTypeConfiguration<Movie> پیاده می‌کند
        - Unique index روی Title (case-insensitive با default collation) اعمال شده
        - mapping برای Id, Title, Genre (enum to string یا int), CreatedAt تعریف شده
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است
```

### T-007: ساخت پروژه Migrations (MovieWatchlist.Migrations)
```md
- [ ] T-007: ساخت پروژه Migrations
      Implements:    Foundation
      Depends on:    T-005, T-006
      Files:
        - src/Migrations/Migrations.csproj
        - src/Migrations/MovieWatchlistDbContextFactory.cs (IDesignTimeDbContextFactory)
      Rules:
        - dotnet-tooling (alwaysApply)
        - dotnet-structure
      Acceptance:
        - Migrations.csproj ارجاع به Persistence دارد
        - نام پروژه/assembly MovieWatchlist.Migrations است (طبق معماری)
        - IDesignTimeDbContextFactory برای EF tooling وجود دارد (connection string از env/args)
        - به MovieWatchlist.slnx اضافه شده
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است
```

### T-008: ساخت پروژه Execution + Domain Movie (entity, enum, errors, repository interface, repository impl)
```md
- [ ] T-008: ساخت پروژه Execution و مدل دامنه Movie
      Implements:    BR-002, BR-008, BR-009; SVC-D1, SVC-D2, SVC-D3, SVC-D6, SVC-D7
      Depends on:    T-003 (SharedKernel), T-005 (Persistence)
      Files:
        - src/Execution/Execution.csproj
        - src/Execution/Domain/Entities/Movie.cs
        - src/Execution/Domain/ValueObjects/MovieGenre.cs
        - src/Execution/Domain/ValueObjects/MovieErrors.cs
        - src/Execution/Domain/Repositories/IMovieRepository.cs
        - src/Execution/Infrastructure/MovieRepository.cs
      Rules:
        - dotnet-solid
        - dotnet-async
        - dotnet-error-handling
        - dotnet-naming
        - dotnet-structure
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - Execution.csproj ارجاع به SharedKernel و Persistence دارد؛ به MovieWatchlist.slnx اضافه شده
        - Movie entity: Id (Guid), Title, Genre (MovieGenre), CreatedAt؛ factory Create(title, genre, IClock) (applies BR-008, BR-009)؛ متد ApplyChanges برای ویرایش
        - MovieGenre enum با مقادیر: Action, Comedy, Drama, Horror, SciFi, Romance, Thriller, Animation, Documentary, Other (BR-002)
        - MovieAlreadyExistsError و MovieNotFoundError به‌عنوان typed Error (فرزند Error) تعریف شده‌اند (BR-010)
        - IMovieRepository با متدهای async و CancellationToken: GetByIdAsync, GetByTitleAsync, AddAsync, UpdateAsync, DeleteAsync, GetPagedAsync
        - MovieRepository پیاده‌سازی EF Core با AsNoTracking برای queryهای read-only
        - هیچ متدی null برنمی‌گرداند (BR-012)؛ از IReadOnlyCollection<T> استفاده شده
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است
        - بازبینی دستی در برابر فایل‌های Rules مرتبط
```

### T-009: ایجاد Migration اولیه برای Movie
```md
- [ ] T-009: ایجاد Migration اولیه برای Movie
      Implements:    BR-001 (unique index در migration)
      Depends on:    T-006, T-007, T-008
      Files:
        - src/Migrations/Migrations/<timestamp>_InitialCreate.cs
        - src/Migrations/Migrations/MovieWatchlistDbContextModelSnapshot.cs
      Rules:
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - Migration با نام InitialCreate تولید شده و جدول Movie با unique index روی Title دارد
        - ModelSnapshot تولید شده
        - API خودکار migrate نمی‌کند؛ migrationها فقط از پروژه Migrations اجرا می‌شوند
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet ef migrations script (یا لیست migrations) بدون خطا
        - بررسی دستی: unique index روی Title موجود است
```

---

## Phase B — Vertical Slices (T-010 تا T-013)

### T-010: Slice AddMovie (end-to-end)
```md
- [ ] T-010: پیاده‌سازی slice AddMovie به‌صورت end-to-end
      Implements:    BR-001,002,003,008,009,010,011,012; US-001; SVC-001,002,003,004,K1; DF-001
      Depends on:    T-008, T-009
      Files:
        - src/Execution/AddMovie/Delivery/AddMovieEndpoint.cs
        - src/Execution/AddMovie/Delivery/AddMovieRequest.cs
        - src/Execution/AddMovie/Delivery/AddMovieResponse.cs
        - src/Execution/AddMovie/Delivery/AddMovieValidator.cs
        - src/Execution/AddMovie/Workflow/AddMovieOrchestrator.cs
        - src/Execution/AddMovie/Domain/ValueObjects/AddMovieCommand.cs
        - src/Execution/AddMovie/Domain/ValueObjects/AddMovieResult.cs
        - src/Execution/AddMovie/BusinessActions/EnsureTitleUniqueAction.cs
        - src/Execution/AddMovie/BusinessActions/PersistNewMovieAction.cs
      Rules:
        - dotnet-solid
        - dotnet-async
        - dotnet-error-handling
        - dotnet-clean-code
        - dotnet-api-documentation
        - dotnet-structure
        - dotnet-naming
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - AddMovieEndpoint با Minimal API: POST، .WithName/.WithSummary/.WithTags(Execution)/.Produces<AddMovieResponse>(201)/.ProducesProblem(400/409/500)
        - CorrelationId header طبق dotnet-api-documentation
        - AddMovieValidator اعتبارسنجی title (BR-003) و genre (BR-002)
        - AddMovieOrchestrator فقط هماهنگ‌کننده است؛ Execute(command, ct) برمی‌گرداند Result<AddMovieResult>
        - EnsureTitleUniqueAction بررسی یکتایی (BR-001) و خروجی MovieAlreadyExistsError در صورت تکرار (BR-010)
        - Movie.Create از IClock استفاده می‌کند (BR-009)؛ Id با Guid.NewGuid (BR-008)
        - CancellationToken در کل زنجیره عبور داده می‌شود (BR-011)
        - Endpoint Result را به 201/409/400 نگاشت می‌کند (خطای typed نه exception)
        - هیچ null return یا List<T> در API داخلی نیست (BR-012)
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است
        - تست یکپارچه‌سازی AddMovie در T-018 پوشش داده می‌شود
        - بازبینی دستی در برابر فایل‌های Rules مرتبط
```

### T-011: Slice UpdateMovie (end-to-end)
```md
- [ ] T-011: پیاده‌سازی slice UpdateMovie به‌صورت end-to-end
      Implements:    BR-001,002,003,006,010,011; US-002; SVC-005,006,007,008,K2; DF-002
      Depends on:    T-010 (EnsureTitleUniqueAction قابل reuse/share است)
      Files:
        - src/Execution/UpdateMovie/Delivery/UpdateMovieEndpoint.cs
        - src/Execution/UpdateMovie/Delivery/UpdateMovieRequest.cs
        - src/Execution/UpdateMovie/Delivery/UpdateMovieResponse.cs
        - src/Execution/UpdateMovie/Delivery/UpdateMovieValidator.cs
        - src/Execution/UpdateMovie/Workflow/UpdateMovieOrchestrator.cs
        - src/Execution/UpdateMovie/Domain/ValueObjects/UpdateMovieCommand.cs
        - src/Execution/UpdateMovie/BusinessActions/LoadMovieAction.cs
        - src/Execution/UpdateMovie/BusinessActions/PersistMovieAction.cs
      Rules:
        - dotnet-solid, dotnet-async, dotnet-error-handling, dotnet-clean-code
        - dotnet-api-documentation, dotnet-structure, dotnet-naming
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - PUT endpoint با movieId در route و OpenAPI کامل (200/400/404/409/500)
        - UpdateMovieValidator اعتبارسنجی title/genre
        - LoadMovieAction فیلم را بارگذاری می‌کند؛ اگر نبود MovieNotFoundError (BR-006, BR-010) → 404
        - اگر title تغییر کرده، EnsureTitleUniqueAction (با استثنای id فعلی) بررسی می‌کند (BR-001) → 409 در تداخل
        - Movie.ApplyChanges تغییرات را اعمال می‌کند؛ createdAt ثابت می‌ماند
        - CancellationToken propagate می‌شود (BR-011)
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است
        - بازبینی دستی در برابر فایل‌های Rules مرتبط
      Notes:
        - EnsureTitleUniqueAction را طوری طراحی کن که بتواند id فعلی را برای استثنا قبول کند (SRP حفظ شود؛ اگر نیاز به پارامتر اضافی است، همان action با overload مجاز نیست — به‌جای آن command شامل currentId باشد)
```

### T-012: Slice DeleteMovie (end-to-end)
```md
- [ ] T-012: پیاده‌سازی slice DeleteMovie به‌صورت end-to-end
      Implements:    BR-006,010,011; US-003; SVC-009,010,K3; DF-003
      Depends on:    T-011 (LoadMovieAction قابل reuse/share است)
      Files:
        - src/Execution/DeleteMovie/Delivery/DeleteMovieEndpoint.cs
        - src/Execution/DeleteMovie/Workflow/DeleteMovieOrchestrator.cs
        - src/Execution/DeleteMovie/BusinessActions/DeleteMovieAction.cs
      Rules:
        - dotnet-solid, dotnet-async, dotnet-error-handling, dotnet-clean-code
        - dotnet-api-documentation, dotnet-structure, dotnet-naming
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - DELETE endpoint با movieId در route و OpenAPI کامل (204/404/500)
        - LoadMovieAction (مشترک با UpdateMovie) وجود فیلم را تأیید می‌کند (BR-006)؛ نبود → 404
        - DeleteMovieAction حذف را انجام می‌دهد
        - CancellationToken propagate می‌شود (BR-011)
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است
        - بازبینی دستی در برابر فایل‌های Rules مرتبط
```

### T-013: Slice GetMovies (end-to-end)
```md
- [ ] T-013: پیاده‌سازی slice GetMovies به‌صورت end-to-end
      Implements:    BR-004,005,010,011,012; US-004; SVC-011,012,K4,K5; DF-004
      Depends on:    T-008
      Files:
        - src/Execution/GetMovies/Delivery/GetMoviesEndpoint.cs
        - src/Execution/GetMovies/Delivery/GetMoviesResponse.cs
        - src/Execution/GetMovies/Workflow/GetMoviesOrchestrator.cs
        - src/Execution/GetMovies/Domain/ValueObjects/PagingParameters.cs
        - src/Execution/GetMovies/BusinessActions/ResolvePagingParametersAction.cs
        - src/Execution/GetMovies/BusinessActions/QueryMoviesAction.cs
      Rules:
        - dotnet-solid, dotnet-async, dotnet-error-handling, dotnet-clean-code
        - dotnet-performance (AsNoTracking), dotnet-api-documentation
        - dotnet-structure, dotnet-naming
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - GET endpoint با query params (page?, pageSize?) و OpenAPI کامل (200/500)
        - ResolvePagingParametersAction defaults را از Config و سقف maxPageSize اعمال می‌کند (BR-004)
        - QueryMoviesAction با AsNoTracking، مرتب CreatedAt نزولی (BR-005)، skip/take
        - response: { items, page, pageSize, totalCount }؛ items هرگز null نیست (BR-012)
        - page بزرگ‌تر از موجود → items خالی، totalCount صحیح
        - CancellationToken propagate می‌شود (BR-011)
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است
        - بازبینی دستی در برابر فایل‌های Rules مرتبط
```

---

## Phase C — Cross-cutting, Config & Security (T-014 تا T-015)

### T-014: تعریف Policy/Role authorization (غیرفعال در Dev)
```md
- [ ] T-014: تعریف Policy/Role authorization (غیرفعال در Dev)
      Implements:    BR-007
      Depends on:    T-010, T-011, T-012
      Files:
        - src/Api/Program.cs (auth/policy registration)
        - (هر endpoint با RequireAuthorization("ContributorOrAdmin") یا مشابه)
      Rules:
        - dotnet-security
        - dotnet-di
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - Policy با نام مشخص (مثلاً "ContributorOrAdmin") برای write endpoints تعریف شده
        - در محیط Development، Policyها موقتاً غیرفعال‌اند (⚠ ASSUMPTION-003) تا endpointها قابل تست باشند
        - پیام‌های خطا اطلاعات داخلی سیستم را افشا نمی‌کنند
        - مسیرهای dev نباید در production فعال باشند
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است
        - تست دستی در Dev: endpointها بدون auth قابل فراخوانی‌اند
```

### T-015: Config، dev profile و secret.json placeholder
```md
- [ ] T-015: تنظیم Config، dev profile و placeholder برای secret.json
      Implements:    BR-004 (Config); ⚠ ASSUMPTION-004
      Depends on:    T-002
      Files:
        - src/Api/appsettings.json
        - src/Api/appsettings.Development.json
        - (مستندات/README کوتاه درباره secret.json — نه خود فایل secret)
      Rules:
        - dotnet-security
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - appsettings.json شامل بخش MovieWatchlist با defaultPageSize و maxPageSize (نه Hardcode در کد)
        - appsettings.Development.json پروفایل Dev را تنظیم می‌کند
        - هیچ connection string یا secret واقعی در appsettings نیست (⚠ ASSUMPTION-004: از secret.json خوانده می‌شود)
        - ConnectionString placeholder یا reference به secret.json با کامنت مستند شده
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet build موفق است
        - بررسی دستی: هیچ secret واقعی commit نشده
```

---

## Phase D — NuGet Management (T-016)

### T-016: مدیریت NuGet (Central Package Management + restore + build)
```md
- [ ] T-016: افزودن/به‌روزرسانی پکیج‌های NuGet (EF Core SqlServer, EF Core Design و سایر مورد نیاز)
      Implements:    Foundation NuGet
      Depends on:    T-001
      Files:
        - Directory.Packages.props
        - src/Persistence/Persistence.csproj
        - src/Migrations/Migrations.csproj
        - (سایر csprojهای affected)
        - packages.lock.json (پروژه‌های affected)
        - offline-packages/*.nupkg (اگر مکانیزم آفلاین وجود دارد)
      Rules:
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - PackageVersion در Directory.Packages.props ثبت شده است
        - PackageReference بدون Version در csproj ثبت شده است
        - dotnet restore MovieWatchlist.slnx --force-evaluate موفق است
        - اگر sync آفلاین وجود دارد، اجرا شده و پکیج در offline-packages موجود است
        - dotnet build MovieWatchlist.slnx --no-restore بدون خطاست
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - build green
        - packages.lock.jsonهای affected به‌روز شده‌اند
        - وضعیت offline-packages بررسی و در گزارش نهایی ثبت شده است
      Notes:
        - ⚠ ASSUMPTION-007: Repository فعلی script آفلاین ندارد؛ این موضوع به‌عنوان Open Item ثبت شود و Story بدون ثبت آن Done اعلام نشود
```

---

## Phase E — Tests (T-017 تا T-018)

### T-017: Unit tests برای منطق دامنه و orchestratorها
```md
- [ ] T-017: نوشتن unit tests برای منطق و orchestratorها
      Implements:    US-001..US-004 (logic verification)
      Depends on:    T-010, T-011, T-012, T-013
      Files:
        - tests/Execution.Tests/Execution.Tests.csproj
        - tests/Execution.Tests/AddMovie/AddMovieOrchestratorTests.cs
        - tests/Execution.Tests/GetMovies/QueryMoviesActionTests.cs
      Rules:
        - dotnet-testing
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - نام‌گذاری Method_Scenario_ExpectedResult
        - AAA pattern رعایت شده
        - mock برای repository و IClock استفاده شده
        - موارد: AddMovie موفق، AddMovie تکرار عنوان، GetMovies با defaults، GetMovies لیست خالی، paging صحیح
        - داده مصنوعی استفاده شده، نه داده واقعی افراد
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet test موفق است
        - بازبینی دستی در برابر dotnet-testing
```

### T-018: Integration tests برای چهار endpoint
```md
- [ ] T-018: نوشتن integration tests برای چهار endpoint
      Implements:    US-001..US-004 (end-to-end)
      Depends on:    T-017, T-014, T-015
      Files:
        - tests/Execution.Tests/Integration/MovieEndpointsTests.cs
      Rules:
        - dotnet-testing
        - dotnet-tooling (alwaysApply)
      Acceptance:
        - تست‌های یکپارچه‌سازی با TestServer/WebApplicationFactory
        - سناریوهای موفق و خطا پوشش داده شده: Add 201/409/400، Update 200/404/409، Delete 204/404، Get 200/خالی/pagination
        - CancellationToken در تست‌ها قابل قطع نیست ولی propagate شدنش در unit test بررسی شده
        - داده مصنوعی استفاده شده
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - dotnet test موفق است
```

---

## Phase F — Final (T-019)

### T-019: بازبینی نهایی و build کامل
```md
- [ ] T-019: بازبینی نهایی، restore و build کامل پروژه
      Implements:    Definition of Done
      Depends on:    همه تسک‌های قبلی
      Files:
        - (تمام فایل‌های پروژه)
      Rules:
        - dotnet-tooling (alwaysApply)
        - (بازبینی همه فایل‌های rules)
      Acceptance:
        - dotnet restore MovieWatchlist.slnx --force-evaluate موفق
        - dotnet build MovieWatchlist.slnx --no-restore بدون خطا و بدون warningهای تحلیل‌گر
        - dotnet test موفق
        - dotnet format اجرا شده و بدون تغییر لازم
        - بررسی: هیچ TODO باز، import بلااستفاده، کد مرده وجود ندارد
        - Coverage Matrix در گزارش نهایی تکمیل شده
        - انطباق با تمام قوانین فهرست‌شده در Rules
      Verification:
        - خروجی build و test در final verification report (فاز ۵) ثبت می‌شود
```

---

## Coverage Matrix (به‌روزرسانی‌شده با Task IDs)

| نیازمندی PRD/Story | BR | US | SVC | DF | Task |
|---|---|---|---|---|---|
| Foundation | — | — | — | — | T-001..T-009 |
| SharedKernel | BR-010 | — | D4,D5 | — | T-004 |
| Persistence + unique index | BR-001 | — | D3 | — | T-005, T-006 |
| Migration Movie | BR-001 | — | — | — | T-009 |
| AddMovie | BR-001..003,008,009 | US-001 | 001..004,K1 | DF-001 | T-010 |
| UpdateMovie | BR-001,006 | US-002 | 005..008,K2 | DF-002 | T-011 |
| DeleteMovie | BR-006 | US-003 | 009,010,K3 | DF-003 | T-012 |
| GetMovies | BR-004,005,012 | US-004 | 011,012,K4,K5 | DF-004 | T-013 |
| Authorization (Dev غیرفعال) | BR-007 | همه | endpoints | همه | T-014 |
| Config + dev profile | BR-004 | US-004 | K4 | DF-004 | T-015 |
| NuGet management | — | — | — | — | T-016 |
| Unit tests | — | همه | — | — | T-017 |
| Integration tests | — | همه | — | — | T-018 |
| Final build/verify | — | — | — | — | T-019 |
| CancellationToken | BR-011 | همه | همه | همه | T-010..T-013 |
| No null / No List<T> | BR-012 | همه | همه | همه | T-004, T-008..T-013 |
| Auth واقعی (Slice A) | — | — | — | — | Out-of-Scope (OOS-001) |
| Watchlist/Rating (Slice C) | — | — | — | — | Out-of-Scope (OOS-002) |
| search/filter/sort پیشرفته | — | — | — | — | Out-of-Scope (OOS-003) |

---

## Definition of Done

- [ ] تمام تسک‌های قابل اجرای فاز B-E `- [x]` شده‌اند
- [ ] Coverage Matrix بدون سطر ناقص است یا Out-of-Scopeها دلیل روشن دارند
- [ ] dotnet restore موفق
- [ ] dotnet build موفق (بدون warning تحلیل‌گر)
- [ ] dotnet test موفق
- [ ] اگر NuGet تغییر کرده، lock file بررسی شده (offline package به‌عنوان Open Item)
- [ ] هیچ Secret/Token/Credential در کد یا مستندات نیست
- [ ] هیچ UI مستقل / IAM داخلی / Manual Review domain پیاده نشده
- [ ] خروجی با معماری و AI/rules منطبق است
- [ ] ⚠ Open Item: اسکریپت sync آفلاین پکیج‌ها موجود نیست — قبل از Done واقعی باید تأیید شود
