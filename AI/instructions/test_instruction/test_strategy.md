# Test Strategy

مسیر: `AI/instructions/test_instruction/test_strategy.md`

## هدف

این سند، سیاست سطح بالای تست برای Storyها را تعریف می‌کند.

هدف این فایل این است که قبل از تولید تست، نوع Story تشخیص داده شود و بر اساس ریسک و ماهیت Story، نسبت مناسب Unit / Integration / E2E مشخص گردد.

این فایل فقط سیاست کلی تست را مشخص می‌کند.  
جزئیات استانداردهای Unit Test در فایل زیر تعریف شده است:

[unit_test_standard.md](./unit_test_standard.md)

---

# نقش

دستیار هوشمند در زمان استفاده از این فایل باید نقش‌های زیر را داشته باشد:

- Test Architect
- Technical Lead
- Senior Software Engineer
- QA Automation Engineer

---

# ورودی‌های لازم برای تشخیص نوع Story

برای هر Story، دستیار باید این ورودی‌ها را بررسی کند:

1. PRD همان Story
2. Implementation Plan
3. Implementation Checklist
4. معماری پایه
5. کد پروژه
6. Git status، در صورت وجود

اگر Git status وجود نداشت، دستیار باید بر اساس PRD، Checklist و کد پروژه محدوده Story را تشخیص دهد.

---

# Story Classification

قبل از تعیین هرم تست، نوع Story باید تشخیص داده شود.

## 1. Business Story

Storyهایی که عمدتاً شامل منطق بیزینسی، اعتبارسنجی‌های معمول، پردازش داده یا تغییرات ساده در یک Slice هستند.

نمونه‌ها:

- ایجاد یا ویرایش یک Rule معمولی
- اضافه‌کردن یک فیلد ساده
- تغییر Validation غیرامنیتی
- تغییر Mapping
- تغییر Response یا Request
- تغییر ساده در Workflow

### Test Pyramid


| Test Level          | Ratio |
| ------------------- | ----- |
| Unit Test           | 70%   |
| Integration Test    | 20%   |
| E2E / Scenario Test | 10%   |


---

## 2. Security Story

Storyهایی که شامل رفتارهای امنیتی هستند اما جریان‌های بحرانی  را به‌صورت کامل تغییر نمی‌دهند.

نمونه‌ها:

- اعتبارسنجی ورودی‌های امنیتی
- تغییر Rule دسترسی
- تغییر ساده در Audit
- تغییر Policy محدود
- تغییر Error Message برای جلوگیری از نشت اطلاعات
- تغییر در Permission Check غیرمرکزی

### Test Pyramid


| Test Level          | Ratio |
| ------------------- | ----- |
| Unit Test           | 60%   |
| Integration Test    | 25%   |
| E2E / Scenario Test | 15%   |


---

## 3. Security Critical Story

Storyهایی که مستقیماً با هویت، احراز هویت، مجوز، Token، Secret، Session یا SSO درگیر هستند.

نمونه‌ها:

- Login
- Logout
- Refresh Token
- Token Rotation
- Token Revocation
- SSO
- OAuth
- OIDC
- MFA
- Password Reset
- Authorization Engine
- Permission Evaluation
- Client Secret Management
- Session Management

### Test Pyramid


| Test Level          | Ratio |
| ------------------- | ----- |
| Unit Test           | 50%   |
| Integration Test    | 30%   |
| E2E / Scenario Test | 20%   |


---

## 4. Infrastructure Story

Storyهایی که بیشتر روی زیرساخت، Persistence، DI، پکیج‌ها، تنظیمات، Pipeline یا ابزارها اثر دارند.

نمونه‌ها:

- تغییر Repository
- تغییر Persistence Mapping
- تغییر DI Registration
- تغییر Configuration
- تغییر Package / Dependency
- تغییر Logging
- تغییر Health Check
- تغییر Background Job

### Test Pyramid


| Test Level          | Ratio |
| ------------------- | ----- |
| Unit Test           | 40%   |
| Integration Test    | 50%   |
| E2E / Scenario Test | 10%   |


---

## 5. Refactoring Story

Storyهایی که رفتار بیرونی سیستم را تغییر نمی‌دهند و هدف آن‌ها بهبود ساختار، خوانایی، SRP، حذف duplication یا جابه‌جایی کد است.

### Test Pyramid


| Test Level          | Ratio |
| ------------------- | ----- |
| Unit Test           | 80%   |
| Integration Test    | 15%   |
| E2E / Scenario Test | 5%    |


---

# Story Classification Rules

دستیار باید Story را با قوانین زیر طبقه‌بندی کند:

## Security Critical Detection

اگر Story شامل یکی از مفاهیم زیر باشد، باید Security Critical در نظر گرفته شود:

- Authentication
- Authorization
- Login
- Logout
- Token
- Refresh Token
- Access Token
- Password
- Secret
- SSO
- OAuth
- OIDC
- MFA
- Session
- Permission
- Role
- Policy Decision
- Client Secret

## Infrastructure Detection

اگر بیشترین تغییرات Story در این بخش‌ها باشد، Story زیرساختی است:

- Infrastructure
- Persistence
- Repository Implementation
- Configuration
- Dependency Injection
- Logging
- Observability
- Package / Dependency
- Build
- Tooling
- Health Check

## Refactoring Detection

اگر Story رفتار قابل مشاهده جدید اضافه نکند و Acceptance Criteria فقط به ساختار، تمیزی کد یا معماری مربوط باشد، Story از نوع Refactoring است.

## Business Detection

اگر Story نه Security Critical باشد، نه Infrastructure و نه Refactoring، به‌صورت پیش‌فرض Business Story در نظر گرفته شود.

## Classification Priority Rule (قانون اولویت‌بندی و تقدم)

در صورتی که یک Story شامل تغییراتی باشد که در بیش از یک دسته قرار می‌گیرد، دستیار هوشمند موظف است دسته‌بندی نهایی را بر اساس میزان ریسک و حساسیت، دقیقاً طبق سلسله‌مراتب آبشاری زیر (از بالاترین به پایین‌ترین اولویت) انتخاب کند:

1. **Security Critical (بالاترین اولویت):** اگر حتی بخش کوچکی از Story با مفاهیم هویت، احراز هویت، مجوزها یا Tokenها درگیر باشد، تمام دسته‌بندی‌های دیگر باطل شده و Story مستقیماً Security Critical در نظر گرفته می‌شود.
2. **Security:** در صورت عدم تطابق با سطح یک، اگر تغییری در اعتبارسنجی‌های امنیتی یا Policyها وجود داشت.
3. **Infrastructure:** در صورت عدم تطابق با سطوح امنیتی، اگر تغییرات زیرساختی، دیتابیس یا پیکربندی با تغییرات بیزینسی ترکیب شده باشند، به دلیل تاثیر سراسری زیرساخت، اولویت با این دسته است.
4. **Refactoring:** در صورتی که تغییرات صرفاً ساختاری باشند و هیچ رفتار جدیدی اضافه نکنند.
5. **Business (پایین‌ترین اولویت - پیش‌فرض):** اگر Story با هیچ‌یک از ۴ سطح بالاتر تطابق نداشت، یا تغییرات زیرساختی/امنیتی در آن وجود نداشت، به‌عنوان یک تغییر روتین بیزینسی طبقه‌بندی می‌شود.

**قانون توقف (Short-circuit Rule):** دستیار باید شرایط Story را از بالا به پایین (از شماره ۱ به ۵) بررسی کند. به محض رسیدن به **اولین تطابق**، فرآیند ارزیابی متوقف شده و هرم تست همان دسته باید انتخاب شود.

# Test Level Definitions

## Unit Test

Unit Test باید رفتار یک Unit کوچک، منسجم و مستقل را بررسی کند.

جزئیات دقیق Unit Test در فایل زیر اجباری است:

[unit_test_standard.md](./unit_test_standard.md)

در هر Story، Unit Test باید بیشترین تمرکز را روی موارد زیر داشته باشد:

- Business Rules
- Validation
- Decision Points
- Domain Rules
- Error Handling
- Policy Logic
- Value Objects
- Business Actions
- Orchestrator Behavior

---

## Integration Test

Integration Test بررسی می‌کند که دو یا چند Component واقعی سیستم، در یک مرز مشخص و کنترل‌شده، درست با هم کار می‌کنند.

هدف Integration Test بررسی همکاری اجزا است، نه کل مسیر کاربر از ابتدا تا انتها.

integration test  ها ترکیب معناداری از چند unit هستند اما به لایه endpoint  یا  API  نمی رسند، در واقع نقطه ورود integration test  ها یک یا چند کلاس orchestrator است.

رعایت کردن تمامی نکات موجود در فایل زیر اجباری است:

[integration_test_standard.md](./integration_test_standard.md)

Integration Test نباید جای Unit Test را بگیرد.

---

## E2E / Scenario Test

E2E یا Scenario Test فقط برای مسیرهای حیاتی و observable سیستم استفاده شود. به ازای هر API حتما باید یک E2E test  با حداقل دو سناریو وجود داشته باشد. در E2E test  های مرتبط با API ها، بحث validation  داده، سطح دسترسی ها و خروجی نهایی API حایز اهمیت است.

جزئیات دقیق Scenario / E2E Test در فایل زیر اجباری است:

[scenario_e2e_test_standard.md](./scenario_e2e_test_standard.md)

نمونه‌ها:

- Login Success
- Login Failure
- Invalid Token
- Expired Token
- Unauthorized Access
- Permission Denied
- SSO Callback Success
- SSO Callback Failure

این تست‌ها باید کم، هدفمند و فقط برای سناریوهای high-risk باشند. 

---

# Test Planning Output

برای هر Story، دستیار باید ابتدا فایل زیر را در پوشه همان Story تولید کند:

```text
AI/prompts/[STORY_ID]_[STORY_NAME]/[STORY_ID]_[STORY_NAME]_test_scope.md
```

این فایل باید شامل موارد زیر باشد:

## Story Classification

```text
Story Type:
Reason:
Selected Test Pyramid:
```

## Test Pyramid Decision


| Test Level          | Target Ratio | Reason |
| ------------------- | ------------ | ------ |
| Unit Test           | ...          | ...    |
| Integration Test    | ...          | ...    |
| E2E / Scenario Test | ...          | ...    |


## Requirement Traceability


| Requirement | Acceptance Criteria | Suggested Test Level | Reason |
| ----------- | ------------------- | -------------------- | ------ |


## Critical Common Rules

- در مرحله assertion  استفاده کردن از if else  ممنوع است.
- در مرحله when  یا همان act باید بر اساس case طراحی شده، عملیاتهای لازم روی SUT انجام شود.
- برای کاربران با سطح دسترسی های مختلف متدهای ActAS[AccsessLevel] مشترک بساز. وظیفه این متد فقط ایجاد کاربری با آن سطح دسترسی است و هیچ کار یگری انجام نمیدهد.
--قانون shared helper

---

# Rule for Output Given to AI Test Executor

خروجی این مرحله باید به شکلی باشد که کاربر بتواند آن را به دستیار هوشمند بدهد و دستیار بر اساس آن تست‌ها را اجرا کند.

بنابراین هر خروجی تست باید:

- مسیر فایل‌ها را مشخص کند
- Test Level را مشخص کند
- Test Caseها را مشخص کند
- ارتباط با Requirement و Acceptance Criteria را مشخص کند
- قابل اجرا توسط AI Agent باشد
- بدون نیاز به سوال مجدد از کاربر باشد

---

# Final Rule

هیچ تستی نباید قبل از تعیین نوع Story و انتخاب Test Pyramid تولید شود.