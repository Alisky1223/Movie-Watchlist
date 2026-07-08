# Integration Test Standard

## هدف

این سند استاندارد رسمی Integration Test در پروژه است.

Integration Test باید رفتار همکاری چند Component واقعی سیستم را بررسی کند، بدون اینکه وارد تست کامل مرورگر یا سناریوی end-to-end شود.
تست لایه API  مرتبط با E2E test  ها می باشد.

---

## نقش

دستیار هوشمند هنگام طراحی یا نوشتن Integration Test باید نقش‌های زیر را داشته باشد:

- Test Architect
- Senior Software Engineer
- QA Automation Engineer
- Technical Lead

---

## تعریف Integration Test

Integration Test بررسی می‌کند که دو یا چند Component واقعی سیستم، در یک مرز مشخص و کنترل‌شده، درست با هم کار می‌کنند.
با توجه به تعریف unit در مکتب کلاسیک اگر تست نوشته شود که بیش از یک یونیت را درگیر کند این تست integration  است و یا اگر یک یونیت به همراه سرویس دیگری تست شود که این تعامل منجر به درگیر شدن یک وابستگی خارجی شود، این ترکیب هم یک  integration test است.
integration test  ها ترکیب معناداری از چند unit هستند اما به لایه endpoint  یا  API  نمی رسند، در واقع نقطه ورود integration test  ها یک یا چند کلاس orchestrator است.

هدف این سطح تست بررسی همکاری اجزا است، نه کل مسیر کاربر از ابتدا تا انتها.

نمونه‌های قابل قبول:

- Repository + Test Database
- DI Registration + Service Resolution
- Application Service + Repository + Unit of Work
- Middleware / Pipeline بخشی
- Serialization / Deserialization
- Authorization Pipeline بخشی
- Configuration Binding
- Background Job Handler + Fake external dependency

---

## مرز Integration Test

Integration Test نباید با Unit Test اشتباه گرفته شود.

اگر فقط یک کلاس کوچک با dependencyهای fake تست می‌شود، Unit Test است.

Integration Test نباید با E2E اشتباه گرفته شود.

اگر تست مرورگر واقعی، UI کامل یا چند سرویس بیرونی واقعی را اجرا می‌کند، E2E یا Scenario Test است.
در integration test  ها لایه controller نباید درگیر شود.

---

## Dependency Rules (مدیریت وابستگی‌ها)

دستیار هوشمند باید برای جلوگیری از تست‌های Flaky، وابستگی‌ها را دقیقاً طبق ۳ قانون زیر مدیریت کند:

1. **Internal (داخلی):** تعامل سرویس‌ها و لایه‌های داخل پروژه. **قانون: Mock ممنوع.** اجزای واقعی باید با هم کار کنند.
2. **Controllable External (خارجی تحت کنترل):** زیرساخت‌هایی مثل Database، Redis و Queue. **قانون: Mock ممنوع.** صرفاً از نمونه‌های واقعیِ ایزوله (مثل TestContainers یا Test DB) استفاده شود.
3. **Uncontrollable External (خارجی غیرقابل کنترل):** APIها و سرویس‌های Third-Party بیرونی (مثل شاهکار). **قانون: Mock/Fake اجباری.** تست نباید به شبکه یا پایداری سرویس‌های خارج از سیستم وابسته باشد.

---

## Database Rules

اگر Integration Test به دیتابیس نیاز دارد:

1. دیتابیس باید مخصوص تست باشد.
2. داده تست باید توسط خود تست ساخته شود.
3. تست باید مستقل و repeatable باشد.
4. بعد از تست، وضعیت دیتابیس باید پاک یا rollback شود.
5. تست نباید به seedهای production وابسته باشد.
6. assertion باید رفتار را بررسی کند، نه فقط وجود رکورد را.
7. ایزوله‌سازی InMemory: برای جلوگیری از تداخل State در اجرای موازی، نام دیتابیس‌های In-Memory باید در هر Test Case کاملاً یکتا (مثلاً با `Guid.NewGuid().ToString()`) تولید شود.

---

## Given / When / Then

هر Integration Test باید ساختار زیر را داشته باشد:

```text
Given
When
Then
```

Given:
- باید مشخص کند کدام اجزای واقعی استفاده می‌شوند و کدام dependencyها fake هستند.
- در صورت لزوم دیتابیس را از داده های قبلی پاک کن تا استقلال تست حفظ شود.
- در صورت لزوم داده های پیش نیاز را در دیتابیس ایجاد کن.
- در صورت لزوم داده های ورودی orchestrator را اماده کن.

When:
- باید عمل اصلی را اجرا کند.
- اجرای عملیات باید از بالاترین لایه هماهنگ کننده باشد.
- Act انجام شده باید کاملا شفاف و مرتبط با هدف تست باشد.
- استفاده از Sleep یا Delay   نباید اتفاق بیفتد. به جای آن باید از الگوهای انتظار هوشمند مثل ابزارهای awaitality یا pooling های شرطی استفاده کند.

Then:
- باید خروجی یا اثر قابل مشاهده را بررسی کند.
- حتما خروجی سرویس orchestrator بررسی شود.
- در صورت لزوم data integrity  تایید شود.
- در صورت لزوم تغییر وضعیت سایر جداول بررسی شود.
Teardown:
- تست یکپارچگی نباید ردپای خود را در دیتابیس بگذارد.

---

## Naming Standard

نام تست باید رفتار integration را مشخص کند:

```text
ComponentOrFeature_Scenario_ExpectedIntegratedBehavior
```

نمونه:

```text
UserRepository_SaveAndLoadUser_PersistsRequiredFields
```

---

## Assertion Standards

Assertion باید یکی از موارد زیر را بررسی کند:

- ذخیره و بازیابی درست داده
- اعمال شدن transaction یا rollback
- resolve شدن dependency از DI
- اجرای درست validation یا mapping در کنار component دیگر
- رفتار درست pipeline
- خطای قابل کنترل در حالت failure
- side effect قابل مشاهده و کنترل‌شده

Assertionهای بی‌ارزش مانند `Assert.NotNull(result)` بدون بررسی رفتار ممنوع هستند.

---

## Failure Path Coverage

Integration Test فقط مسیر موفق نیست.

برای هر رفتار پرریسک باید failure path هم بررسی شود:

- دیتای نامعتبر
- constraint violation
- عدم وجود رکورد
- عدم مجوز
- conflict
- failure در dependency fake شده

---

## Flaky Test Prevention

Integration Test ممکن است از Unit Test کندتر باشد، اما نباید flaky باشد.

دلایل رایج flaky در Integration Test:

- دیتای مشترک
- database cleanup ناقص
- وابستگی به زمان واقعی
- race condition
- parallel execution بدون isolation
- dependency محیطی

اگر تست flaky است، Story نباید READY اعلام شود.

---

## Integration Test Output Format for AI Executor

برای هر Integration Test Case این قالب باید تولید شود:

```text
Test ID:
Integrated Components:
Scenario:
Related Requirement / Acceptance Criteria:
Given:
When:
Then:
Assertions:
Test Data:
Real Dependencies:
Fake Dependencies:
Database / Persistence Notes:
Isolation Strategy:
Expected Test Name:
Suggested File Path:
```

---

## Final Definition of Done for Integration Test

یک Integration Test فقط وقتی قابل قبول است که:

- اجزای واقعی مشخص داشته باشد.
- dependencyهای fake و real را شفاف اعلام کند.
- به production database یا سرویس خارجی واقعی وابسته نباشد.
- داده تست خودش را ایجاد کند.
- deterministic و repeatable باشد.
- assertion رفتاری و معنادار داشته باشد.
- failure pathهای مهم را پوشش دهد.
- به Requirement یا Acceptance Criteria قابل ردیابی باشد.
