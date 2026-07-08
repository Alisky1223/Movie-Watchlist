# Scenario / E2E Test Standard

## هدف

این سند استاندارد رسمی Scenario / E2E Test در پروژه است.

Scenario / E2E Test باید فقط برای مسیرهای حیاتی، high-risk و قابل مشاهده کاربر یا سیستم استفاده شود.

هدف این سطح تست، اطمینان از کارکرد یک جریان کامل مهم است، نه پوشش دادن تمام جزئیات داخلی.

---

## نقش

دستیار هوشمند هنگام طراحی یا نوشتن Scenario / E2E Test باید نقش‌های زیر را داشته باشد:

- Test Architect
- QA Automation Engineer
- Senior Software Engineer
- Technical Lead

---

## تعریف Scenario / E2E Test

Scenario / E2E Test یک مسیر کامل و معنی‌دار را از دید کاربر، سیستم مصرف‌کننده API بررسی می‌کند.

نمونه‌ها:

- Login Success
- Login Failure
- Logout
- Refresh Token
- Permission Denied
- Role Assignment سپس دسترسی مجاز
- Expired Token Rejection
- SSO Callback Success / Failure

---

## اصل محدودیت تعداد

Scenario / E2E Test باید کم باشد.

AI نباید برای هر Acceptance Criteria یک E2E Test بسازد.

E2E فقط زمانی لازم است که:

- جریان business-critical یا security-critical است.
- شکست آن اثر جدی دارد.
- Unit / Integration / API tests به‌تنهایی اطمینان کافی نمی‌دهند.
- جریان از چند boundary مهم عبور می‌کند.

---

## Scope Rules

### در Scope

- مسیرهای حیاتی کاربر یا سیستم
- جریان‌های کامل authentication / authorization
- خطاهای امنیتی مهم
- regressionهای high-risk
- contractهای حیاتی بین چند بخش
- در E2E test ها حتما باید لایه controller درگیر شود.

### خارج از Scope

- تست همه validationهای جزئی
- تست همه edge caseهای کوچک
- تست جزئیات UI غیرحیاتی
- تست implementation detail
- تست‌هایی که Unit یا API / Feature به‌خوبی پوشش می‌دهند

---

## Dependency Rules

Scenario / E2E Test تا حد امکان باید در محیط تست کنترل‌شده اجرا شود.

ممنوع:

- production database
- production secrets
- سرویس خارجی واقعی، مگر محیط sandbox رسمی و کنترل‌شده باشد
- داده واقعی کاربران
- وابستگی به وضعیت محیط بیرونی

مجاز:

- test database
- fake external provider
- sandbox provider کنترل‌شده، فقط اگر در پروژه پذیرفته شده باشد
- test users / roles / permissions ساخته‌شده توسط خود تست

---

## Given / When / Then

هر Scenario / E2E Test باید ساختار زیر را داشته باشد:

```text
Given
When
Then
```

Given باید وضعیت اولیه کامل سناریو را مشخص کند.

When باید جریان اصلی را اجرا کند.

Then باید outcome نهایی قابل مشاهده را بررسی کند.

---

## Naming Standard

نام تست باید جریان و نتیجه مورد انتظار را نشان دهد:

```text
ScenarioName_Condition_ExpectedOutcome
```

نمونه:

```text
LoginFlow_ValidCredentials_IssuesAccessTokenAndAllowsProtectedEndpoint
```

---

## Assertion Standards

Scenario / E2E Test باید assertionهای سطح جریان داشته باشد، نه assertionهای ریز داخلی.

موارد قابل assert:

- نتیجه نهایی جریان
- status نهایی
- دسترسی یا عدم دسترسی
- token یا session outcome بدون افشای secret
- تغییر state مهم
- audit/log قابل تست، اگر contract است

موارد غیرمجاز:

- assertion روی private method
- assertion روی جزئیات داخلی الگوریتم
- assertionهای زیاد روی UI غیرحیاتی
- assertionهای fragile وابسته به متن‌های غیرقراردادی

---

## Flaky Test Prevention

E2E بیشتر از سایر تست‌ها در معرض flaky شدن است.

برای جلوگیری:

- از sleep ثابت استفاده نشود.
- به زمان واقعی وابسته نشود.
- داده تست unique و کنترل‌شده باشد.
- محیط تست قابل reset باشد.
- parallel execution فقط با isolation کامل مجاز است.
- سرویس خارجی واقعی استفاده نشود.
- مدیریت زمان در E2E: در سناریوهای زمانی (مثل Lockout یا Token Expiration)، به‌جای تزریق Fake Clock به `TestHost`، داده‌های اولیه دیتابیس را مستقیماً بر اساس زمان واقعی سیستم (مثلاً `UtcNow.AddMinutes(15)`) Seed کنید.

اگر E2E flaky است، Story نباید READY اعلام شود.

---

## Scenario / E2E Test Output Format for AI Executor

برای هر Scenario / E2E Test Case این قالب باید تولید شود:

```text
Test ID:
Scenario Name:
Business / Security Risk:
Related Requirement / Acceptance Criteria:
Given:
When:
Then:
Critical Assertions:
Test Data:
Environment Requirements:
External Dependencies:
Isolation Strategy:
Expected Test Name:
Suggested File Path:
```

---

## Final Definition of Done for Scenario / E2E Test

یک Scenario / E2E Test فقط وقتی قابل قبول است که:

- سناریوی high-risk یا critical را پوشش دهد.
- تعداد آن محدود و هدفمند باشد.
- به production data یا secret واقعی وابسته نباشد.
- flaky نباشد.
- خروجی نهایی جریان را assert کند.
- جزئیات داخلی را تست نکند.
- به Requirement یا Acceptance Criteria قابل ردیابی باشد.
