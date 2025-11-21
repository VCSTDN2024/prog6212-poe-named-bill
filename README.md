Contract Monthly Claim System (CMCS)

This workspace contains the prototype plus the full Part 3 automation deliverable for the PROG6212 Portfolio of Evidence assignment.

Structure:
- src/CMCS - ASP.NET Core MVC application (prototype + Part 2 features)

Notes:
- The repository uses an in-memory store for claims. No external database is required to run the demo.
- To run this project you need the .NET 8 SDK installed.

Status:
- Part 1: GUI prototype created (non-functional front-end available in Views).
- Part 2: Claim submission, approval/rejection, file upload wiring added (in-memory storage).
- Part 3: Lecturer automation with real-time calculation/validation, coordinator & manager workflow automation, and HR reporting tools.

Automation Highlights (Part 3):
- **Lecturer experience** – Auto-calculation card, validation service, and quick-fill helpers make it fast to capture compliant claims.
- **Coordinator workflow** – Dedicated dashboard runs automated policy checks (hours/rate/attachments) and allows one-click auto-verification when the risk score is low.
- **Manager workflow** – Verified claims surface in a manager dashboard with risk scoring, recommendations, and bulk auto-approval powered by `ClaimWorkflowService`.
- **HR tools** – HR dashboard summarises approved claims, exports CSV invoices via `HrReportingService`, and offers a lecturer directory for updating contact/banking details.
- **Quality gates** – Expanded unit tests (`ClaimReviewServiceTests`, `ClaimWorkflowServiceTests`, `HrReportingServiceTests`) keep automation logic regression-safe.

Getting Started:
1. `dotnet restore`
2. `dotnet test tests/CMCS.Tests/CMCS.Tests.csproj`
3. `dotnet run --project src/CMCS/CMCS.csproj`

Sample Roles:
- Lecturer: submit claims and view automation summary.
- Coordinator: `/Coordinator/Dashboard`
- Manager: `/Manager/Dashboard`
- HR: `/Hr/Index` and `/Hr/Lecturers`

Refer to `POE_Part2_Report.docx` for the formal Part 2 Word document required for submission.