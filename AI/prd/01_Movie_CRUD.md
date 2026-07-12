# User Story: Movie CRUD (Slice B)

**Story ID:** `01`
**Slice:** B — Movie CRUD
**Epic(s):** EPIC-01 (Core Movie Management)
**Status:** Ready
**Priority:** Must

---

> ## 🎯 جایگاه این استوری در نقشه کلی
>
> این استوری، استوری شماره **۱** از سه استوری است که API لیست فیلم‌های تماشا را می‌سازد:
>
> | Slice | محتوا | وضعیت |
> |---|---|---|
> | **A — Auth & Users** | ثبت‌نام، لاگین، لاگ‌اوت، مدیریت کاربر توسط ادمین | ⏳ بعداً |
> | **B — Movie CRUD** | افزودن، ویرایش، حذف، مشاهده لیست فیلم | ✅ **این استوری** |
> | **C — Watchlist & Rating** | افزودن فیلم به واتچ‌لیست شخصی، امتیازدهی | ⏳ بعداً |
>
> این استوری **فقط** دامنه مدیریت فیلم را پوشش می‌دهد. Auth و Watchlist/Rating در استوری‌های جداگانه پیاده می‌شوند.

---

## 1. عنوان تسک
پیاده‌سازی CRUD فیلم‌ها: افزودن، ویرایش، حذف و مشاهده لیست فیلم‌ها با pagination.

## 2. هدف و ارزش بیزینسی (از دیدگاه PRD)
هسته اصلی محصول را می‌سازد: کاربران نویسنده بتوانند فیلم‌ها را تعریف، ویرایش و حذف کنند و همه کاربران بتوانند لیست فیلم‌ها را ببینند. بدون این قابلیت، سایر sliceها (Watchlist/Rating) داده‌ای برای کار کردن ندارند.

## 3. خلاصه ساده استوری (Simple Summary)
یک API می‌سازیم که فیلم‌ها را ذخیره می‌کند. کاربران نقش نویسنده می‌توانند فیلم اضافه، ویرایش یا حذف کنند. همه کاربران می‌توانند لیست فیلم‌ها را با صفحه‌بندی ببینند. فیلم شامل عنوان و ژانر است.

## 4. داستان کاربر (User Story)

> **به عنوان** یک کاربر با نقش **Contributor** (نویسنده)
> **می‌خواهم** فیلم اضافه، ویرایش و حذف کنم
> **تا بتوانم** کاتالوگ فیلم‌های موجود در سیستم را مدیریت کنم.

> **به عنوان** یک کاربر با نقش **Viewer** (فقط‌خوان)
> **می‌خواهم** لیست فیلم‌ها را با صفحه‌بندی ببینم
> **تا بتوانم** از فیلم‌های موجود آگاه شوم (برای افزودن به واتچ‌لیست در Slice C).

## 5. مفروضات (Assumptions)

- `⚠ ASSUMPTION` این استوری **فقط CRUD فیلم** را شامل می‌شود. Watchlist، Rating و Auth در استوری‌های جدا (Slice A و C) هستند و در اینجا پیاده نمی‌شوند.
- `⚠ ASSUMPTION` **وابستگی به Auth (Slice A):** چون Slice A هنوز ساخته نشده، endpointها فعلاً با **Policy/Role-based authorization تعریف می‌شوند**، ولی در محیط `Development` بدون احراز هویت واقعی در دسترس هستند تا قابل تست باشند. به‌محض اتصال Slice A، Policyها به آن متصل می‌شوند.
- `⚠ ASSUMPTION` **فیلدهای مشتق‌شده از Slice C:** `averageRating` و `watchlistCount` در این استوری وجود ندارند و در response نمایش داده نمی‌شوند. این فیلدها در Slice C به response اضافه می‌شوند.
- `⚠ ASSUMPTION` **فیلم موجودیت سراسری (global) است**؛ متعلق به یک کاربر خاص نیست و توسط Contributor ایجاد می‌شود.
- `⚠ ASSUMPTION` **هیچ محدودیتی** برای تعداد فیلم‌ها وجود ندارد.
- `⚠ ASSUMPTION` شناسه فیلم `Guid` است.
- `⚠ ASSUMPTION` **جلوگیری از تکرار فیلم** بر اساس **Title یکتا (case-insensitive)** است.
- `⚠ ASSUMPTION` connection string دیتابیس توسط کاربر بعداً در فایل `secret.json` اضافه می‌شود؛ در این استوری فقط placeholder و dev environment آماده می‌شود.

## 6. پیش‌نیازها (Dependencies)

- ساختار پایه پروژه `MovieWatchlist.slnx` و پروژه‌ها طبق معماری (Screaming Architecture + Vertical Slice) آماده باشد.
- پروژه `Persistence` (EF Core + SQL Server) به‌صورت shell آماده باشد؛ DbContext و DI registration موجود باشد.
- فایل `secret.json` برای نگه‌داری connection string توسط کاربر ایجاد شود.
- Central Package Management (`Directory.Packages.props`) فعال باشد.
- `Open Dependency`: سیستم احراز هویت واقعی (Slice A) هنوز موجود نیست؛ Policyها آماده ولی غیرفعال‌شده در Dev طراحی می‌شوند.

## 7. جریان کاربری (User Flow) — *در صورت داشتن UI*

> ⚠ طبق معماری، این پروژه **Backend/API-only** است و UI مستقل نمی‌سازد. این بخش اعمال نمی‌شود. جریان‌ها از طریق فراخوانی مستقیم API (Swagger/Postman) انجام می‌شوند.

## 8. معیارهای پذیرش (Acceptance Criteria)

*(فرمت BDD)*

### ۸.۱ — افزودن فیلم (Add Movie)

- **سناریو ۱: افزودن موفق توسط Contributor**
  - **فرض کنید (Given):** کاربر نقش `Contributor` (یا `Admin`) دارد. (در حال حاضر Policy غیرفعال است؛ در محیط Dev همه می‌توانند فراخوانی کنند.)
  - **وقتی که (When):** کاربر درخواست افزودن فیلم با `title` و `genre` معتبر می‌فرستد.
  - **آنگاه (Then):** فیلم ساخته می‌شود، `201 Created` با بدنه شامل `id`, `title`, `genre`, `createdAt` برمی‌گردد.

- **سناریو ۲: عنوان تکراری**
  - **فرض کنید:** فیلمی با همان `title` (case-insensitive) از قبل وجود دارد.
  - **وقتی که:** کاربر دوباره همان عنوان را می‌فرستد.
  - **آنگاه:** پاسخ `409 Conflict` با خطای typed برمی‌گردد و فیلم ساخته نمی‌شود.

- **سناریو ۳: ورودی نامعتبر (ولیدیشن)**
  - **فرض کنید:** `title` خالی است یا `genre` عضو enum معتبر نیست.
  - **وقتی که:** کاربر درخواست می‌فرستد.
  - **آنگاه:** پاسخ `400 Bad Request` با جزئیات خطای اعتبارسنجی برمی‌گردد.

### ۸.۲ — ویرایش فیلم (Update Movie)

- **سناریو ۱: ویرایش موفق**
  - **فرض کنید:** فیلم با `movieId` وجود دارد و کاربر نقش `Contributor`/`Admin` دارد.
  - **وقتی که:** کاربر فیلدهای `title` و/یا `genre` را ویرایش می‌کند.
  - **آنگاه:** فیلم آپدیت می‌شود و `200 OK` با داده جدید برمی‌گردد.

- **سناریو ۲: فیلم یافت نشد**
  - **فرض کنید:** `movieId` وجود ندارد.
  - **وقتی که:** کاربر ویرایش می‌فرستد.
  - **آنگاه:** پاسخ `404 Not Found` برمی‌گردد.

- **سناریو ۳: تداخل عنوان هنگام ویرایش**
  - **فرض کنید:** `title` جدید با عنوان فیلم دیگری (case-insensitive) تداخل دارد.
  - **وقتی که:** کاربر ویرایش می‌فرستد.
  - **آنگاه:** پاسخ `409 Conflict` برمی‌گردد.

### ۸.۳ — حذف فیلم (Delete Movie)

- **سناریو ۱: حذف موفق**
  - **فرض کنید:** فیلم وجود دارد و کاربر نقش `Contributor`/`Admin` دارد.
  - **وقتی که:** کاربر درخواست حذف با `movieId` می‌فرستد.
  - **آنگاه:** فیلم حذف می‌شود و `204 No Content` برمی‌گردد.

- **سناریو ۲: فیلم یافت نشد**
  - **فرض کنید:** `movieId` وجود ندارد.
  - **وقتی که:** کاربر حذف می‌فرستد.
  - **آنگاه:** پاسخ `404 Not Found` برمی‌گردد.

> `⚠ ASSUMPTION`: در این استوری فیلم وابستگی به جدول دیگری (مثل واتچ‌لیست/امتیاز) ندارد (آن‌ها در Slice C هستند). cascade در Slice C هنگام ایجاد آن جداول تنظیم می‌شود.

### ۸.۴ — مشاهده لیست فیلم‌ها (Get Movies List)

- **سناریو ۱: دریافت لیست با pagination**
  - **فرض کنید:** چند فیلم در سیستم وجود دارد.
  - **وقتی که:** کاربر درخواست با پارامتر `page` و `pageSize` می‌فرستد.
  - **آنگاه:** پاسخ `200 OK` شامل لیست فیلم‌ها (مرتب بر اساس جدیدترین `CreatedAt` نزولی) همراه با اطلاعات pagination برمی‌گردد.

- **سناریو ۲: لیست خالی**
  - **فرض کنید:** هیچ فیلمی وجود ندارد.
  - **وقتی که:** کاربر درخواست می‌فرستد.
  - **آنگاه:** پاسخ `200 OK` با آرایه خالی (نه `null`) برمی‌گردد.

- **سناریو ۳: مقادیر پیش‌فرض pagination**
  - **فرض کنید:** کاربر `page` و `pageSize` را ارسال نکرده است.
  - **وقتی که:** کاربر درخواست می‌فرستد.
  - **آنگاه:** مقادیر پیش‌فرض از Config (مثلاً `page=1`, `pageSize=10`) به‌عنوان Fallback اعمال می‌شوند.

### ۸.۵ — موارد مشترک فنی

- **سناریو: لغو عملیات (Cancellation)**
  - **فرض کنید:** درخواست در حال اجراست.
  - **وقتی که:** کلاینت درخواست را لغو می‌کند.
  - **آنگاه:** `CancellationToken` در کل زنجیره عبور داده شده و عملیات بدون نشت منابع متوقف می‌شود.

- **سناریو: خطای دامنه‌ای typed**
  - **فرض کنید:** شرط دامنه‌ای نقض می‌شود (مثلاً تکرار عنوان یا فیلم یافت نشدن).
  - **وقتی که:** orchestrator خطا را پردازش می‌کند.
  - **آنگاه:** خطا به‌صورت typed (`Result<T>` / typed error) مدیریت می‌شود، نه با exception.

## 9. نیازمندی‌های داده‌ای و فنی (Technical & Data Notes)

### ۹.۱ — ماتریس نقش‌ها و دسترسی (Role/Permission Matrix)

| Use Case | Viewer | Contributor | Admin |
|---|:---:|:---:|:---:|
| مشاهده لیست فیلم‌ها | ✅ | ✅ | ✅ |
| افزودن فیلم | ❌ | ✅ | ✅ |
| ویرایش فیلم | ❌ | ✅ | ✅ |
| حذف فیلم | ❌ | ✅ | ✅ |

> `⚠ ASSUMPTION`: چون Slice A هنوز ساخته نشده، Policyها **آماده و تعریف می‌شوند ولی در محیط `Development` موقتاً غیرفعال‌اند** تا endpointها قابل تست باشند. هنگام اتصال Slice A، Policyها فعال می‌شوند.

### ۹.۲ — تغییرات دیتابیس (Database Changes)

> `Open Decision`: طراحی دقیق Schema نیازمند سند طراحی تأییدشده (HLD/LLD) است (طبق معماری). موارد زیر طرح مفهومی است، نه schema نهایی.

موجیت **Movie** (در bounded context `Execution` — موجودیت سراسری):

| فیلد | نوع | توضیح |
|---|---|---|
| `Id` | `Guid` | کلید اصلی |
| `Title` | `string` | یکتا، case-insensitive، ۱ تا ۲۰۰ کاراکتر |
| `Genre` | `enum` | Action, Comedy, Drama, Horror, Sci-Fi, Romance, Thriller, Animation, Documentary, Other |
| `CreatedAt` | `DateTimeOffset` | زمان ایجاد، از `IClock` |

**Enum ژانر** (در `Domain/ValueObjects/`):

```
Action, Comedy, Drama, Horror, Sci-Fi, Romance, Thriller, Animation, Documentary, Other
```

**ایندکس‌ها:**
- ایندکس یکتا (unique) روی `Movie.Title` (case-insensitive).
- ایندکس روی `Movie.CreatedAt` (برای مرتب‌سازی سریع جدیدترین‌ها).

> `⚠ ASSUMPTION`: در این slice جدول دیگری وجود ندارد. `averageRating` و `watchlistCount` در Slice C اضافه می‌شوند و نیازی به ستون placeholder نیست.
>
> Migrationها از مسیر کنترل‌شده `MovieWatchlist.Migrations` اجرا می‌شوند؛ API خودکار migrate نمی‌کند (طبق معماری).

### ۹.۳ — اندپوینت‌ها / APIها (Endpoints/APIs)

> طبق معماری، مسیر دقیق (Route) بدون سند طراحی تأییدشده قطعی نمی‌شود. فقط use caseها اشاره می‌شوند. هر کدام در slice خودش با الگوی Orchestrator/Actions پیاده می‌شود.

1. **افزودن فیلم** — use case `AddMovie`
2. **ویرایش فیلم** — use case `UpdateMovie`
3. **حذف فیلم** — use case `DeleteMovie`
4. **مشاهده لیست فیلم‌ها** — use case `GetMovies` (با pagination)

### ۹.۴ — نمونه داده (Mock Data / Example)

```json
// === Add Movie Request (Contributor/Admin) ===
POST .../movies
{
  "title": "Inception",
  "genre": "Sci-Fi"
}
// Add Movie Response (201)
{
  "id": "3b8a1234-5678-90ab-cdef-1234567890ab",
  "title": "Inception",
  "genre": "Sci-Fi",
  "createdAt": "2026-07-11T12:00:00Z"
}

// === Update Movie Request ===
PUT .../movies/3b8a1234-5678-90ab-cdef-1234567890ab
{
  "title": "Inception (2010)",
  "genre": "Sci-Fi"
}
// Update Movie Response (200)
{
  "id": "3b8a1234-5678-90ab-cdef-1234567890ab",
  "title": "Inception (2010)",
  "genre": "Sci-Fi",
  "createdAt": "2026-07-11T12:00:00Z"
}

// === Delete Movie Response (204) ===
DELETE .../movies/3b8a1234-5678-90ab-cdef-1234567890ab
// (no body)

// === Get Movies Response (200, with pagination) ===
GET .../movies?page=1&pageSize=10
{
  "items": [
    {
      "id": "3b8a1234-5678-90ab-cdef-1234567890ab",
      "title": "Inception",
      "genre": "Sci-Fi",
      "createdAt": "2026-07-11T12:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 1
}
```

### ۹.۵ — منطق پردازشی بک‌اند (Backend Logic)

- یکتایی `Title`: مقایسه case-insensitive (مثلاً با `EF.Functions.ILike` در SQL Server یا normalizing در لایه domain).
- مرتب‌سازی لیست: بر اساس `CreatedAt` نزولی (جدیدترین اول).
- Pagination: اعمال مقادیر پیش‌فرض Config هنگام نبود پارامترها (`page=1`, `pageSize=10`)؛ اعمال سقف (`maxPageSize` از Config).
- تمام عملیات async باید `CancellationToken` دریافت و عبور دهند (طبق defensive coding).
- تاریخ‌ها از abstraction `IClock` تولید شوند (طبق معماری).
- خطاهای قابل انتظار دامنه‌ای باید typed و با `Result<T>` برگردانده شوند، نه با exception:
  - `MovieAlreadyExistsError` (برای تکرار عنوان)
  - `MovieNotFoundError` (برای فیلم یافت نشدن)
- حذف: اگر فیلم یافت نشد، `MovieNotFoundError` برگردانده می‌شود.
- **ممنوعیت null return** (طبق معماری): collection خالی به‌صورت empty collection برگردد، نه `null`. به‌جای `null` از `Result<T>` استفاده شود.
- **ممنوعیت `List<T>`/`T[]` در APIهای داخلی**: از `IReadOnlyCollection<T>` یا `IEnumerable<T>` استفاده شود.

### ۹.۶ — اعتبارسنجی‌ها (Validations)

- `title`: غیرخالی، طول ۱ تا ۲۰۰ کاراکتر، یکتا (case-insensitive).
- `genre`: عضو enum معتبر.
- `page` ≥ ۱ (در صورت نبود، مقدار پیش‌فرض).
- `pageSize` در بازه `[1, maxPageSize]` (در صورت نبود، مقدار پیش‌فرض؛ در صورت بیشتر از سقف، سقف اعمال می‌شود).

### ۹.۷ — امنیت و دسترسی (Security & Auth)

- تعریف Policy/Role-based authorization برای جداسازی دسترسی نقش‌ها (Contributor برای افزودن/ویرایش/حذف؛ همه برای مشاهده).
- `⚠ ASSUMPTION`: در این slice، Policyها تعریف می‌شوند ولی در محیط `Development` موقتاً غیرفعال‌اند تا endpointها قابل تست باشند (چون Slice A هنوز موجود نیست).
- پیام‌های خطا نباید اطلاعات داخلی سیستم را افشا کنند.
- توکن/credential نباید در log، exception یا response افشا شوند.
- مسیرهای dev/sandbox نباید در production فعال باشند (طبق معماری — با فلگ محیطی کنترل شود).

### ۹.۸ — پیکربندی و Persistence (Config & Persistence)

- `defaultPageSize`، `maxPageSize` و تنظیمات مشابه در `appsettings.json` (Config) باشند، نه Hardcode.
- Connection string دیتابیس از فایل `secret.json` خوانده شود (`⚠ ASSUMPTION`: کاربر بعداً محتوای واقعی را اضافه می‌کند).
- یک **profilename به نام `Development`** برای اجرای محلی آماده شود (local dev environment).
- EF Core + SQL Server با الگوی `Persistence` (DbContext shell، `AddMovieWatchlistPersistence` در DI) مطابق معماری.
- Migrationها از پروژه `MovieWatchlist.Migrations` اجرا شوند؛ API خودکار migrate نکند.

### ۹.۹ — پکیج‌ها و ابزارها (Packages/Tools)

> `⚠ ASSUMPTION`: نسخه‌ها در `Directory.Packages.props` (Central Package Management) ثبت شوند و `PackageReference` بدون `Version` در `.csproj` قرار گیرد.

- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Design` (برای tooling/migrations)
- پکیج‌های استاندارد ASP.NET Core (در صورت نیاز).

## 10. ملاحظات تجربه کاربری و طراحی (UI/UX Notes)

> این پروژه API-only است و UI مستقل ندارد. این بخش اعمال نمی‌شود. تنها نکته مرتبط: خطاهای API باید پاسخ‌های ساختاریافته (JSON) و قابل فهم برای کلاینت باشند.

## 11. موارد لبه و خارج از محدوده (Edge Cases & Out of Scope)

### Edge Cases
- درخواست با `CancellationToken` لغو شده در حین نوشتن به دیتابیس → باید بدون رکورد ناقص مدیریت شود.
- رقابت (race condition) روی افزودن همزمان دو فیلم با عنوان یکسان → با unique constraint در دیتابیس و تبدیل خطای تکرار به `MovieAlreadyExistsError` هندل شود.
- `page`/`pageSize` نامعتبر → مقادیر پیش‌فرض از Config به‌عنوان Fallback استفاده شوند.
- `page` بزرگ‌تر از تعداد صفحات موجود → برگرداندن آرایه خالی با `totalCount` صحیح (نه خطا).
- عنوان با فقط فضای خالی (whitespace) → باید به‌عنوان نامعتبر رد شود (پس از trim).

### Out of Scope (نباید در این استوری انجام شود)
- **Auth واقعی و مدیریت کاربر** — این Slice A است و بعداً انجام می‌شود. در این slice فقط Policyها تعریف می‌شوند (غیرفعال در Dev).
- **Watchlist شخصی و Rating** — این Slice C است و بعداً انجام می‌شود. فیلدهای `averageRating` و `watchlistCount` در response نیستند.
- **جستجو، فیلتر و مرتب‌سازی پیشرفته** لیست فیلم‌ها — فقط pagination + جدیدترین در این استوری.
- **داشبورد و UI** — خارج از دامنه API.
- **خودکارسازی Migration توسط API** — ممنوع (طبق معماری).
- **تنظیمات production-grade** — فقط dev/local environment.

## 12. چک‌لیست توسعه‌دهنده (Dynamic Developer Checklist)

- [ ] ساخت پروژه‌ها/پوشه‌های bounded context طبق معماری و افزودن به `MovieWatchlist.slnx` (`⚠ ASSUMPTION`: اگر `MovieWatchlist.slnx` وجود نداشت یا نام‌گذاری نادرست بود، حتماً اخطار داده شود).
- [ ] ساخت Model و Migration برای `Movie` (EF Core + SQL Server).
- [ ] آماده‌سازی `Persistence` (DbContext shell، DI) بدون auto-migrate.
- [ ] ساخت چهار vertical slice: `AddMovie`, `UpdateMovie`, `DeleteMovie`, `GetMovies` (هر کدام با Delivery/Workflow/BusinessActions/Domain).
- [ ] پیاده‌سازی Orchestrator/Actions برای هر use case (طبق معماری، SRP).
- [ ] پیاده‌سازی typed errors (`MovieAlreadyExistsError`, `MovieNotFoundError`) در `Domain/ValueObjects/`.
- [ ] پیاده‌سازی Policy/Role-based authorization (تعریف Policyها؛ موقتاً غیرفعال در Dev).
- [ ] اعتبارسنجی بک‌اند برای همه ورودی‌ها (title, genre, page, pageSize).
- [ ] عبور `CancellationToken` در کل زنجیره.
- [ ] استفاده از `IClock` برای تولید `CreatedAt`.
- [ ] استفاده از `IReadOnlyCollection<T>` / `IEnumerable<T>` به‌جای `List<T>`/`T[]` در APIهای داخلی.
- [ ] ثبت مقادیر Config (`defaultPageSize`, `maxPageSize` و...) در `appsettings.json`.
- [ ] آماده‌سازی profile `Development` برای اجرای محلی.
- [ ] ثبت پکیج‌ها در `Directory.Packages.props` و اجرای `dotnet restore --force-evaluate`.
- [ ] بررسی موارد امنیتی (عدم افشای اطلاعات داخلی در خطاها و log).
- [ ] تست دستی Happy Path برای چهار endpoint.
- [ ] تست دستی سناریوهای خطا (تکرار عنوان، فیلم یافت نشدن، ورودی نامعتبر، pagination نامعتبر).
- [ ] نوشتن تست‌های واحد برای منطق (ولیدیشن title یکتا، pagination).
- [ ] **تعریف اتمام (Definition of Done):** API در محیط `Development` محلی بدون خطا build و run شود، Migrationها دستی اعمال شوند، Happy Path هر چهار endpoint در Swagger/Postman کار کند، PR ثبت و code review شود. ⚠ **اسکریپت آفلاین پکیج‌ها در Repository فعلی موجود نیست؛ بدون ثبت آن، استوری را Done اعلام نکنید.**

---

## 13. Open Decisions (تصمیم‌های باز)

- **O-1:** مسیر دقیق endpointها (Routes) نیازمند سند طراحی تأییدشده (HLD/LLD) است (طبق معماری).
- **O-2:** Schema دقیق دیتابیس نیازمند سند طراحی تأییدشده است.
- **O-3:** نحوه اتصال Policyها به Slice A هنگام ساخت آن باید هماهنگ شود.

---

## 14. Reflexion — جمع‌بندی، فرض‌ها و ریسک‌ها

- **تصمیم کلیدی:** scope به Slice B (Movie CRUD) محدود شد. این استوری حالا یک دامنه واحد، مستقل و قابل اجرا توسط یک برنامه‌نویس Junior است.
- **وابستگی اصلی:** وابستگی به Auth (Slice A) با تعریف Policyهای غیرفعال در Dev مدیریت شد تا توسعه مستقل این slice ممکن باشد.
- **فرض‌های اصلی:** فیلم سراسری است؛ Title یکتا case-insensitive؛ `averageRating`/`watchlistCount` در این slice نیستند؛ connection string بعداً از `secret.json`.
- **ریسک‌ها:**
  - اتصال Policyها به Slice A بعداً باید با دقت انجام شود تا سطح دسترسی‌ها صحیح فعال شوند.
  - عدم وجود اسکریپت آفلاین پکیج‌ها در Repository فعلی — به‌عنوان Open Item ثبت شد.
- **معیار پذیرش نهایی:** هر چهار use case (Add, Update, Delete, Get) با سناریوهای موفق و خطای بخش ۸ برآورده شوند، API در `Development` محلی اجرا شود، و نقضی در SRP، defensive coding یا امنیت وجود نداشته باشد.
