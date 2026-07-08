# Unit Test Standard

## هدف

این سند استاندارد رسمی Unit Test در پروژه است.

هر زمان در `test_strategy.md` یا هر Instruction دیگر از Unit Test صحبت می‌شود، رعایت این فایل اجباری است.

---

# نقش

دستیار هوشمند هنگام طراحی یا نوشتن Unit Test باید نقش‌های زیر را داشته باشد:

- Test Architect
- Senior Software Engineer
- QA Automation Engineer
- Technical Lead

---

# Testing School

مکتب تست‌نویسی پروژه برای Unit Test:

```text
Classicist Testing
```

در مکتب Classicist، هدف Unit Test بررسی رفتار واحدهای کوچک و منسجم سیستم است، نه mock کردن همه چیز.

اگر چند کلاس کوچک با cohesion بالا با هم یک رفتار واحد را شکل می‌دهند، تست کردن آن‌ها با هم مجاز است؛ به شرطی که تست همچنان سریع، مستقل، deterministic و بدون وابستگی خارجی باشد.

---

# Definition of Unit

## تعریف Unit

unit در مکتب کلاسیک در واقع یک رفتار بیزینسی مشخص است که چند کلاس کوچک با cohesion بالا شکل گرفته و نقطه ورود تست هم کلاس leader یا orchestrator است.

یک Unit باید با اصول زیر سازگار باشد:

## Cohesion

Unit باید cohesion بالا داشته باشد.

یعنی اجزای داخلی آن همگی حول یک هدف مشخص کار کنند.

اگر یک کلاس یا تابع چند رفتار نامرتبط را انجام می‌دهد، احتمالاً Unit مناسبی نیست یا SRP را نقض کرده است.

## SRP

Unit باید فقط یک مسئولیت داشته باشد.

اگر Unit بیش از یک دلیل برای تغییر دارد، باید قبل از تست، طراحی آن مورد تردید قرار گیرد.

Unit Test نباید ضعف طراحی را پنهان کند.

## Examples of Units

نمونه Unitهای قابل تست:

- Value Object
- Domain Service
- Business Action
- Validator
- Policy
- Mapper
- Orchestrator Behavior
- Error Handling Logic
- Decision Point
- Small Application Service

## Non-Unit Examples

موارد زیر به‌تنهایی Unit Test محسوب نمی‌شوند:

- تست کامل API با HTTP واقعی
- تست دیتابیس واقعی
- تست چند Slice با هم
- تست کامل Login Flow از ابتدا تا انتها
- تست وابسته به سرویس خارجی
- تست وابسته به زمان واقعی یا شبکه واقعی

این موارد باید در Integration یا E2E بررسی شوند.

---

# Unit Test Design Structure

## Mandatory Structure: Given / When / Then

هر Test Case باید Given / When / Then شفاف داشته باشد.  
در طراحی Test Case فقط از ساختار زیر استفاده شود:

```text
Given
When
Then
```

تعریف:

## Given

وضعیت اولیه تست، داده‌ها، preconditionها و dependencyهای کنترل‌شده.

## When

عملی که روی Unit انجام می‌شود.

## Then

نتیجه قابل مشاهده و قابل سنجش.

نتایجی که بررسی میکنی باید بر اساس مکتب کلاسیک باشد و وارد جزییات کلاس ها نشود. در مکتب کلاسیک خروجی ها سنجیده می شود.

---

# Code Structure

در کد تست، ساختار پیاده‌سازی می‌تواند به شکل زیر باشد:

```text
// Given
...

// When
...

// Then
...
```

از ترکیب چند رفتار نامرتبط در یک تست خودداری شود.

---

# Naming Standard

نام تست باید این ساختار را رعایت کند:

```text
MethodUnderTest_Scenario_ExpectedBehavior
```

نمونه:

```text
ExecuteAsync_InvalidPassword_ReturnsInvalidCredentialsError
```

نام تست باید نقش مستندات داشته باشد.

نام‌های زیر ممنوع هستند:

```text
Test1
ShouldWork
LoginTest
ValidTest
InvalidTest
```

---

# Test as Documentation

Unit Test باید مانند مستندات زنده سیستم باشد.

یعنی یک توسعه‌دهنده با خواندن نام تست و Given / When / Then باید بفهمد:

- رفتار مورد انتظار چیست
- در چه شرایطی رخ می‌دهد
- خروجی مورد انتظار چیست
- کدام Rule یا Scenario را محافظت می‌کند

بنابراین:

- نام تست باید دقیق باشد
- داده تست باید قابل فهم باشد
- assertion باید معنای رفتاری داشته باشد
- تست نباید به جزئیات پیاده‌سازی داخلی وابسته باشد

---

# Assertion Standards

## اصل اصلی

Assertion باید رفتار مورد انتظار را به‌صورت روشن، مستقیم و قابل فهم بررسی کند.

## ممنوعیت کپی منطق Production

هرگز منطق اصلی کد Production را در تست کپی نکن تا خروجی را محاسبه کنی.

اشتباه:

```csharp
var expected = Hash(password, salt); // کپی منطق production
Assert.Equal(expected, result.Hash);
```

در چنین شرایطی تست فقط همان منطق تکراری را تایید می‌کند و ممکن است bug واقعی را پیدا نکند.

بهتر:

- از مقدار ثابت قابل اعتماد استفاده کن
- behavior را بسنج
- contract را بسنج
- outcome را بسنج
- از test double کنترل‌شده استفاده کن

## Assertهای بی‌ارزش ممنوع

موارد زیر ممنوع هستند:

```csharp
Assert.True(true);
Assert.NotNull(result); // اگر behavior اصلی را نمی‌سنجد
Assert.True(result != null); // وقتی assertion دقیق‌تر ممکن است
```

## Assertion باید Specific باشد

به‌جای assertion کلی:

```csharp
Assert.True(result.IsFailure);
```

بهتر است خطای دقیق بررسی شود:

```csharp
Assert.True(result.IsFailure);
Assert.Equal(LoginErrors.InvalidCredentials, result.Error);
```

## One Behavior, Focused Assertions

هر تست باید یک رفتار مشخص را بررسی کند.

چند Assert مجاز است، اگر همگی یک رفتار واحد را توضیح دهند.

مثال مجاز:

```csharp
Assert.True(result.IsFailure);
Assert.Equal(LoginErrors.InvalidCredentials, result.Error);
Assert.False(tokenService.WasCalled);
```

چون هر سه مربوط به یک رفتار هستند: پسورد اشتباه نباید Token صادر کند.

## Avoid Over-Assertion

جزئیات غیرمرتبط را assert نکن.

اگر تست برای خطای پسورد نامعتبر است، نیازی نیست تمام فیلدهای response را بررسی کند مگر در contract آن رفتار مهم باشد.

## Assert Observable Behavior

Assertion باید خروجی یا اثر قابل مشاهده را بررسی کند:

- Result
- Error
- State Change
- Published Event
- Called Dependency
- Returned DTO
- Validation Failure
- Security Decision

## Avoid Implementation Detail Assertion

موارد داخلی غیررفتاری را assert نکن:

- تعداد دقیق متدهای private
- ساختار داخلی الگوریتم
- ترتیب داخلی عملیات، مگر بخشی از contract باشد
- نام متغیر یا جزئیات implementation

## Failure Message Clarity

در صورت امکان assertion باید خطای قابل فهم تولید کند.

اگر assertion شکست خورد، باید مشخص باشد چه رفتاری شکسته است.

## Negative Assertion

برای رفتارهای امنیتی، assertion منفی مهم است.

مثال:

- وقتی password اشتباه است، token صادر نشود.
- وقتی user unauthorized است، action اجرا نشود.
- وقتی validation شکست خورد، side effect رخ ندهد.

---

# Isolation Rules

## تعریف Isolation

ایزوله بودن یعنی هر تست مستقل از تست‌های دیگر اجرا شود و نتیجه آن تحت تاثیر اجرای تست‌های دیگر قرار نگیرد.

## Independent Test

تست مستقل یعنی:

1. داده مورد نیاز خود را خودش ایجاد کند.
2. به ترتیب اجرای تست‌ها وابسته نباشد.
3. از state مشترک mutable استفاده نکند.
4. نتیجه آن به زمان واقعی، random واقعی، شبکه، دیتابیس واقعی یا سرویس خارجی وابسته نباشد.
5. بتوان آن را به‌تنهایی اجرا کرد و همان نتیجه اجرای گروهی را بدهد.

## Mandatory Isolation Rules

### Rule 1

Test A نباید روی Test B اثر بگذارد.

### Rule 2

هیچ تستی نباید به ترتیب اجرا وابسته باشد.

### Rule 3

هر تست باید داده تست خودش را بسازد.

### Rule 4

Shared Mutable State ممنوع است.

### Rule 5

Static Mutable State ممنوع است.

### Rule 6

Unit Test نباید از دیتابیس واقعی استفاده کند.

### Rule 7

Unit Test نباید از شبکه واقعی استفاده کند.

### Rule 8

Unit Test نباید به زمان واقعی وابسته باشد.

از `IClock` یا fake clock استفاده شود.

### Rule 9

Unit Test نباید به random واقعی وابسته باشد.

از deterministic random یا test double استفاده شود.

### Rule 10

Fixture مشترک فقط زمانی مجاز است که immutable و بدون side effect باشد.

---

# External Dependencies in Unit Test

در Unit Test موارد زیر باید fake, stub یا test double شوند:

- Database
- HTTP Client
- File System
- Message Broker
- Email Provider
- SMS Provider
- Identity Provider
- Token Provider خارجی
- Real Clock
- Real Random
- Cache خارجی

---

# Mocking Rules

Mock فقط زمانی استفاده شود که واقعاً لازم است.

## Mock مجاز است وقتی:

- dependency خارجی است
- side effect باید بررسی شود
- failure باید شبیه‌سازی شود
- interaction بخشی از contract رفتاری است

## Mock نامناسب است وقتی:

- فقط برای افزایش coverage استفاده می‌شود
- تست را به implementation detail وابسته می‌کند
- باعث شکنندگی تست می‌شود
- رفتار اصلی با state یا output قابل سنجش است

---

# Protection Against Regression

Regression Policy جداگانه وجود ندارد.

اما در طراحی Test Case باید محافظت در برابر Regression رعایت شود.

یعنی اگر یک Business Rule، Security Rule، Validation یا Edge Case مهم وجود دارد، باید تستی طراحی شود که اگر در آینده آن رفتار خراب شد، تست شکست بخورد.

دستیار باید هنگام طراحی Test Case از خود بپرسد:

```text
اگر این Rule در آینده اشتباه تغییر کند، آیا این تست شکست می‌خورد؟
```

اگر پاسخ نه بود، تست ارزش کافی ندارد.

---

# Good Unit Test Indicators

یک Unit Test خوب باید ویژگی‌های زیر را داشته باشد:

## Fast

FAST در Unit Test یعنی سرعت اجرای تست‌ها باید بالا باشد.

Unit Test باید در حد میلی‌ثانیه یا بسیار سریع اجرا شود.

تست کند معمولاً نشانه dependency خارجی، setup سنگین یا طراحی نامناسب است.

## Isolated

تست باید مستقل باشد و روی تست دیگر اثر نگذارد.

## Repeatable

تست باید در هر محیطی با همان ورودی، همان نتیجه را بدهد.

## Deterministic

تست نباید وابسته به زمان، random، ترتیب اجرا، شبکه یا وضعیت بیرونی باشد.

## Behavior Focused

تست باید رفتار را بررسی کند، نه implementation detail را.

## Readable

تست باید ساده، قابل خواندن و قابل فهم باشد.

## Maintainable

تست نباید با تغییرات کوچک داخلی Production Code بی‌دلیل بشکند.

## Specific

تست باید دقیقاً مشخص کند چه چیزی را بررسی می‌کند.

## Meaningful Assertion

Assertion باید ارزش واقعی داشته باشد.

## Regression

تست باید در برابر Regression محافظت کند.

یعنی اگر یک Business Rule، Security Rule، Validation یا Edge Case مهم در آینده اشتباه تغییر کند، تست باید شکست بخورد.

یک Unit Test خوب رفتارهای مهم را قفل می‌کند تا تغییرات ناخواسته در آینده زود کشف شوند.

---

# False Positive ممنوع

تستی طراحی نشود که false positive باشد.

False Positive یعنی تست سبز می‌شود، اما رفتار واقعی درست نیست.

نمونه‌های رایج:

- Assert.True(true)
- فقط Assert.NotNull بدون بررسی behavior
- تستی که هیچ assertion واقعی ندارد
- تستی که فقط mock setup را تایید می‌کند نه outcome را
- تستی که منطق اشتباه production را در خودش تکرار کرده است

---

# Flaky Test ممنوع

تست‌ها نباید flaky باشند.

Flaky Test یعنی تستی که یک بار سبز می‌شود و بار دیگر بدون تغییر واقعی در کد قرمز می‌شود.

دلایل رایج flaky test:

- وابستگی به زمان واقعی
- وابستگی به ترتیب اجرا
- وابستگی به random
- وابستگی به شبکه
- race condition
- دیتای مشترک
- sleep یا delay غیرقابل اعتماد
- environment dependency

اگر تست flaky تشخیص داده شد، Story نباید READY اعلام شود.

---

# Unit Test Output Format for AI Executor

خروجی طراحی Unit Test باید به شکلی باشد که کاربر بتواند آن را مستقیماً به دستیار هوشمند بدهد و دستیار تست را اجرا کند.

برای هر Unit Test Case این قالب باید تولید شود:

```text
Test ID:
Unit:
Method Under Test:
Scenario:
Given:
When:
Then:
Assertions:
Protection Against Regression:
Test Data:
Dependencies:
Isolation Notes:
Expected Test Name:
Suggested File Path:
```

## Example

```text
Test ID: UT-LOGIN-001
Unit: ValidateCredentialsAction
Method Under Test: ExecuteAsync
Scenario: invalid password
Given: a user exists with a different password hash
When: ExecuteAsync is called with an invalid password
Then: it returns InvalidCredentials error and does not issue token
Assertions:
- result is failure
- result error equals InvalidCredentials
- token issuer is not called
Protection Against Regression:
- protects against accidental token issuance for invalid credentials
Test Data:
- username: valid.user@example.com
- password: WrongPassword
Dependencies:
- fake user repository
- fake password hasher
- fake token issuer
Isolation Notes:
- no real database
- no real token provider
- no real clock
Expected Test Name:
ExecuteAsync_InvalidPassword_ReturnsInvalidCredentialsError
Suggested File Path:
tests/<ModuleName>.Tests/Login/BusinessActions/ValidateCredentialsActionTests.cs
```

---

# Final Definition of Done for Unit Test

یک Unit Test فقط وقتی قابل قبول است که:

- Given / When / Then داشته باشد.
- نام آن `MethodUnderTest_Scenario_ExpectedBehavior` باشد.
- رفتار را بررسی کند، نه implementation detail را.
- assertionهای معنادار داشته باشد.
- منطق production را در تست کپی نکرده باشد.
- false positive نباشد.
- flaky نباشد.
- سریع اجرا شود.
- مستقل باشد.
- deterministic باشد.
- محافظت در برابر Regression داشته باشد.
- به Requirement یا Acceptance Criteria قابل ردیابی باشد.

