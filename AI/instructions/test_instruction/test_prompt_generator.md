# Test Prompt Generator

مسیر: `AI/instructions/test_instruction/test_prompt_generator.md`

این فایل به `test_strategy.md` (همان پوشه) لینک دارد و برای تولید Prompt تست به ازای هر Story استفاده می‌شود.

## Input

- test_strategy.md
- PRD استوری
- Checklist اجراشده استوری
- Analyze PRD, checklist, and change files from related commits in git history and git status to:
Identify units, behaviors, and dependencies.
- کد پروژه

## Steps

1. فایل `test_strategy.md` را بخوان و نوع Story را تشخیص بده (Business / Security / Security Critical / Infrastructure / Refactoring).
2. بر اساس نوع Story، هرم تست (نسبت Unit / Integration / E2E) را انتخاب کن.
3. فایل `[STORY_ID]_[STORY_NAME]_test_scope.md` استوری را تولید کن. این فایل شامل موارد زیر است:
  Story Classification
   Test Pyramid Decision
   Requirement Traceability
   رفتارهایی که در هر سطح از هرم تست باید تست شوند را براساس PRD و کدهای ایجاد شده مشخص کن.
   لیست یونیت ها، برای unit test
   لیست bounded ها برای integration test
   لیست entry point  ها یا  API ها برای E2E test
4. در فایل test_scope به ازای هر نوع تست، استانداردهایی که لازم است در آن رعایت شود را براساس الگوهای ذکر شده در test_strategy، به طور کاملا شفاف و صریح ذکر کن و به فایل های استاندارد لینک نده.
5. لیست Test Caseها را با توجه به فایل test_scope ایجاد و در فایل [STORY_ID]_[STORY_NAME]_test_cases.md بنویس. در نوشتن test case ها موارد زیر را در نظر بگیر:
  - در نوشتن test case  ها نوشتن استانداردهای test_strategy الزامی است.
  - به ازای هر test case رفتاری که باید تست شود را مشخص کن.
  - به ازای هر test case ، entry point  اصلی را مشخص کن.
  - مشخص کن در این test case  رعایت کردن کدام استانداردها حیاتی است. فقط موارد حیاتی را ذکر کن و اضافه گویی نکن.
  - اسم test case  ها باید به شکل کاملا شفاف، رفتار case را منعکس کند. از اسم های صرفا فنی خودداری کن.

خروجی تو باید فایل های زیر باشد

- `[STORY_ID]_[STORY_NAME]_test_scope.md`
- `[STORY_ID]_[STORY_NAME]_test_cases.md`
- `[STORY_ID]_[STORY_NAME]_test_execution_checklist.md`
- `[STORY_ID]_[STORY_NAME]_test_prompt.md`
- `[STORY_ID]_[STORY_NAME]_test_result_report.md`

نحوه ایجاد test_prompt  به شرح زیر است:

## Generate main prompt file

با در نظر گرفتن همه موارد بالا، با الگوبرداری از فرمت زیر یک پرامپت مهندسی‌شده بنویس که نوشتن کدهای تست استوری مدنظر را برای دستیار هوش مصنوعی به‌قدری شفاف کند که دستیار بدون هیچ خطایی، بدون هیچ سوالی و به‌طور کامل تمامی کدهای لازم برای هرم تست استوری را با مصرف بهینه توکن بنویسد. این پرامپت را در فایل `[STORY_ID]_[STORY_NAME]_test_prompt.md` در پوشه همان استوری ذخیره کن.
دقت کن تو فقط پرامپت را ایجاد میکنی و آن پرامپت وظیفه نوشتن کدهای لازم را دارد.

```text
AI/prompts/[STORY_ID]_[STORY_NAME]/[STORY_ID]_[STORY_NAME]_test_prompt.md
```

```text
Generate Test code Prompt for Story [STORY_ID]_[STORY_NAME]
Input Folder: AI/prompts/[STORY_ID]_[STORY_NAME]/
Governing Files:
   - [STORY_ID]_[STORY_NAME]_test_scope.md
   - [STORY_ID]_[STORY_NAME]_test_cases.md
   - [STORY_ID]_[STORY_NAME]_test_execution_checklist.md
Steps:
1. Load and Understanding [STORY_ID]_[STORY_NAME]_test_scope.md file related to story
2. Load and Understanding [STORY_ID]_[STORY_NAME]_test_cases.md file related to story
3. Generate Unit, Integration, and E2E tests according to the below files
   - [STORY_ID]_[STORY_NAME]_test_scope.md
   - [STORY_ID]_[STORY_NAME]_test_cases.md
   - [STORY_ID]_[STORY_NAME]_test_execution_checklist.md
     and check items in [STORY_ID]_[STORY_NAME]_test_execution_checklist.md file
   - CRITICAL: Use the EXACT test method names and assertions must be exactly what is in the [STORY_ID]_[STORY_NAME]_test_cases.md (e.g., `ExecuteAsync_PersistRefreshFails_ReturnsErrorWithoutAuditSuccess`)
   - DO NOT change, abbreviate, or adapt test names - use them exactly as written in the [STORY_ID]_[STORY_NAME]_test_cases.md
   - Just implement cases that are in the [STORY_ID]_[STORY_NAME]_test_cases.md file.
4. Checked items in [STORY_ID]_[STORY_NAME]_test_execution_checklist.md after implement each test case.
5. Check all test cases related to this story (if exist) and fix or update if needed.
6. Execute the project test runner (FAST fail)
7. Update [STORY_ID]_[STORY_NAME]_test_result_report.md
8. Repeat until all tests pass
Output: Updated [STORY_ID]_[STORY_NAME]_test_result_report.md and test files (Unit / Integration / E2E). dont generate extra report file.
```

