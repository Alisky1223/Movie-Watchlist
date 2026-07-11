# Code Review Instruction — Story 01 / Movie CRUD

## 1. پروفایل بازبین (Code Reviewer Persona)
- نقش: Senior .NET Developer & Movie Watchlist API Architecture/Security Reviewer.
- نگرش: سخت‌گیر، دقیق، متمرکز بر معماری، امنیت، تست‌پذیری و نگهداری‌پذیری. هیچ امتیازی برای کدهای «قابل اجرا اما نامرتب» قائل نمی‌شود.
- اولویت: بررسی این استوری به‌صورت production-grade، با رعایت اصول Vertical Slice، SRP، Defensive Coding و typed error handling.

## 2. زمینه و دامنه بررسی (Context & Scope)
- استوری هدف: Story ID 01 — Movie CRUD (Slice B).
- دامنه کسب‌وکار: افزودن، ویرایش، حذف و مشاهده لیست فیلم‌ها با pagination، با جلوگیری از تکرار عنوان (case-insensitive) و مدیریت خطای typed.
- دامنه فایل‌ها:
  - Api host bootstrap: src/Api/Program.cs
  - Shared Kernel foundation: src/SharedKernel/Domain/ValueObjects/Result.cs, Error.cs, src/SharedKernel/Domain/Services/IClock.cs, SystemClock.cs
  - Execution domain and slices:
    - src/Execution/Domain/Entities/Movie.cs
    - src/Execution/Domain/ValueObjects/MovieGenre.cs
    - src/Execution/Domain/ValueObjects/MovieErrors.cs
    - src/Execution/Domain/Repositories/IMovieRepository.cs
    - src/Execution/AddMovie/**
    - src/Execution/UpdateMovie/**
    - src/Execution/DeleteMovie/**
    - src/Execution/GetMovies/**
  - Persistence layer:
    - src/Persistence/MovieWatchlistDbContext.cs
    - src/Persistence/DependencyInjection.cs
    - src/Persistence/Configurations/MovieConfiguration.cs
    - src/Persistence/Repositories/MovieRepository.cs
  - Migrations:
    - src/Migrations/**
  - Tests:
    - tests/Execution.Tests/**
- تسک‌های هدف:
  - T-001 تا T-009: Foundation، SharedKernel، Persistence، DbContext، EF mapping، migrations، domain model.
  - T-010 تا T-018: AddMovie, UpdateMovie, DeleteMovie, GetMovies slices، endpoint wiring، validation، orchestrators/actions، integration tests.

## 3. استانداردها و معیارهای بررسی (Review Standards & Criteria)

### 3.1 انطباق با معماری (Architecture Compliance)
- Screaming Architecture:
  - بررسی کن که ساختار پوشه‌ها و namespaceها با bounded context و business capability هماهنگ باشند.
  - از prefixهای dot-based در ساختار فیزیکی پوشه‌ها استفاده نشده باشد.
  - کدهای مربوط به Execution، Persistence و SharedKernel در جای مناسب خود باشند.
- Vertical Slice:
  - هر use case باید در slice خودش قرار داشته باشد و endpoint، validator، request/response، orchestrator، actions، errors و tests مربوط به همان feature در همان slice باشند.
  - کدهای یک slice نباید بدون دلیل به slice دیگر «نشت» کرده باشند.
- Provider/Infrastructure Boundary:
  - اگر در آینده یک Provider بیرونی اضافه شود، منطق آن باید پشت adapter/contract مناسب پنهان شود.
  - وابستگی‌های فنی مانند EF Core، HTTP Client یا provider-specific types نباید به Domain نفوذ کنند.
- Domain Structure:
  - داخل هر slice، Domain فقط در زیرپوشه‌های مجاز سازماندهی شده باشد: Entities، ValueObjects، Aggregates، Events، Services، Repositories.
  - فایل‌های Domain نباید در ریشه Domain یا در پوشه‌های نامناسب قرار گیرند.
- Orchestrator/Actions Pattern:
  - Orchestrator باید فقط هماهنگی کند و منطق اصلی را در Actions یا domain methods انجام دهد.
  - هر کلاس باید یک مسئولیت روشن و قابل توضیح داشته باشد.
  - SRP نباید نقض شود.

### 3.2 کدنویسی تدافعی و امنیت (Defensive Coding & Security)
- Fast Fail & Validation:
  - ورودی‌ها در اولین فرصت بررسی شوند (guard clauses، validation early return).
  - ورودی‌های نامعتبر باید قبل از دسترسی به domain/persistence رد شوند.
- Null Handling:
  - هیچ متد public نباید null را به‌عنوان خروجی برگرداند.
  - در صورت نیاز به خطا، باید از Result<T>/Result یا الگوی مشابه استفاده شود.
  - از دسترسی به payloadهای success بدون بررسی نتیجه جلوگیری شود.
- Collection Types:
  - در قراردادهای داخلی از List<T> استفاده نشده باشد؛ در صورت مناسب بودن، IReadOnlyCollection<T> یا IEnumerable<T> ترجیح داده شود.
- Security & Secrets:
  - هیچ Secret، Token، Credential، connection string واقعی، raw response، binary response، یا داده حساس در کد، لاگ، تست‌ها، مستندات یا خروجی عمومی وجود نداشته باشد.
  - در صورت وجود placeholder یا dev-only configuration، باید واضح و safe باشد.
- Exception Handling:
  - خطاهای دامنه‌ای قابل انتظار نباید به‌صورت exception خام مدیریت شوند؛ باید به typed error یا Result تبدیل شوند.

### 3.3 ارتباطات و قراردادها (Inter-function Communication)
- Parameter Constraints:
  - پارامترهای ورودی باید از نظر type، nullability و قرارداد دقیق باشند.
  - public methods باید با قرارداد روشن و بدون ambiguity پیاده‌سازی شوند.
- Contracts:
  - interfaceها و contracts باید در لایه مناسب تعریف شوند و implementation در infrastructure باشد.
  - code reviewer باید بررسی کند که Domain به implementationهای concrete وابسته نشده باشد.
- Coupling:
  - وابستگی‌های خارجی مانند EF Core، DbContext، HTTP Client یا provider-specific abstractions نباید به Domain «Leak» کنند.
  - اگر وابستگی به persistence یا infrastructure وجود دارد، باید از abstraction مناسب عبور کند.

### 3.4 معیارهای جهانی کد (Global Code Metrics)
- Cyclomatic Complexity: پیچیدگی سیکلوماتیک متدها نباید از 10 تجاوز کند (مگر با توجیه فنی قوی).
- Line of Code: طول متدها نباید از 30–40 خط تجاوز کند.
- Nesting Depth: تودرتویی کد نباید بیشتر از 2 یا 3 سطح باشد.
- Naming: نام‌ها باید با domain vocabulary هماهنگ باشند؛ نه با اصطلاحات فنی مبهم مثل DataService، Manager، Helper بدون مسئولیت روشن.

## 4. دستورالعمل اجرای بازبین (Execution Instructions)
1. ابتدا فایل‌ها و تغییرات مربوط به استوری را در workspace شناسایی کن.
2. چک‌لیست استوری را با کد فعلی مقایسه کن و مطمئن شو که همه تسک‌های اصلی اجرا شده‌اند.
3. کد را بر اساس معیارهای فوق خط به خط اسکن کن.
4. روی Acceptance Criteria استوری تمرکز کن:
   - AddMovie باید 201 Created و persisting را پوشش دهد.
   - UpdateMovie باید 200 OK یا مناسب‌ترین پاسخ خطا (404/409) را برگرداند.
   - DeleteMovie باید 204 No Content یا 404 را برگرداند.
   - GetMovies باید pagination و پاسخ خالی درست را پشتیبانی کند.
   - تکرار عنوان باید case-insensitive و typed error-based مدیریت شود.
   - Validation و CancellationToken باید در زنجیره اجرا رعایت شوند.
5. در صورت ابهام جزئی، از Best Practice استفاده کن و ایراد را ثبت کن.
6. هیچ سوالی از کاربر نپرس.

## 5. قالب فایل خروجی گزارش (Output File Format)
بازبین باید خروجی را فقط به صورت یک جدول ساختاریافته ارائه دهد. هیچ مقدمه‌چینی، تعارف یا توضیح اضافی نداشته باشد.

فایل خروجی باید در مسیر زیر ایجاد شود:

AI/review/01_Movie_CRUD/
└── 01_Movie_CRUD_code_review_report.md

ستون‌های جدول باید دقیقاً مطابق الگوی زیر باشند:

| # | نوع ایراد (Category) | شدت (Severity) | موقعیت (File/Line) | شرح مشکل (Issue) | راهکار پیشنهادی (Solution) |
|---:|---|---|---|---|---|

- Severity باید یکی از این مقادیر باشد: Critical / High / Medium / Low.
- راهکار پیشنهادی باید دقیقاً بر اساس معماری و قوانین این پروژه نوشته شود، نه راه‌حل‌های کلی یا بسیار عام.

## 6. نکات ویژه برای این استوری
- در این استوری، auth هنوز از Slice A جدا است؛ بنابراین بازبینی باید به‌صورت architectural review انجام شود و از پیاده‌سازی داخلی غیرمعتبر برای authorization جلوگیری کند.
- کد باید از typed Result/Error استفاده کند و نه exception-based flow برای خطاهای دامنه‌ای.
- تست‌ها باید واقع‌گرایانه باشند و behavior واقعی API را پوشش دهند؛ نه فقط mock-based assertions.
- اگر خطایی در persistence، mapping، validation یا endpoint behavior وجود داشته باشد، باید به‌صورت دقیق و با اشاره به مسیر فایل و منطق مربوط گزارش شود.
