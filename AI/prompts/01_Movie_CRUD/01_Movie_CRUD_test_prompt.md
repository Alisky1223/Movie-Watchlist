# Generate Test Code Prompt for Story 01_Movie_CRUD

Input Folder: AI/prompts/01_Movie_CRUD/

Governing Files:
- 01_Movie_CRUD_test_scope.md
- 01_Movie_CRUD_test_cases.md
- 01_Movie_CRUD_test_execution_checklist.md

Objective:
Generate the test code needed for the Movie CRUD story using the test scope, test cases, and execution checklist. The output must be implementation-ready and must follow the repository testing conventions.

Instructions:
1. Read and understand 01_Movie_CRUD_test_scope.md, 01_Movie_CRUD_test_cases.md, and 01_Movie_CRUD_test_execution_checklist.md.
2. Implement only the test cases listed in 01_Movie_CRUD_test_cases.md.
3. Use the exact test method names from 01_Movie_CRUD_test_cases.md. Do not rename, abbreviate, or adapt them.
4. Place unit tests under the existing test project structure in tests/Execution.Tests/.
5. For integration tests, use the real application pipeline and use the existing WebApplicationFactory-style host pattern where appropriate. Prefer the real EF Core in-memory provider for persistence behavior.
6. Do not use mock-only assertions for internal collaborators. Prefer real behavior and real application components.
7. Follow Given/When/Then structure in every test.
8. Keep tests deterministic, isolated, and free from production secrets or real user data.
9. For each test, assert the observable behavior required by the story acceptance criteria.
10. Ensure the resulting suite covers add, update, delete, and get-list behavior.
11. After implementing the tests, run the test project with dotnet test tests/Execution.Tests/Execution.Tests.csproj.
12. If failures occur, fix them and keep iterating until the suite passes.
13. Do not create any extra report file beyond the required result report.

Expected Output:
- Implemented test code files under tests/Execution.Tests/
- Updated 01_Movie_CRUD_test_result_report.md

Important Constraints:
- Do not change the test names from the test cases file.
- Do not add unrelated tests outside the listed cases.
- Keep the implementation aligned with the current project architecture and existing test patterns.
