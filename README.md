Contract Monthly Claim System (CMCS)

This workspace contains the prototype and Part 2 implementation for the PROG6212 Portfolio of Evidence assignment.

Structure:
- src/CMCS - ASP.NET Core MVC application (prototype + Part 2 features)

Notes:
- The repository uses an in-memory store for claims. No database is required to run the demo.
- To run this project you need .NET 7 SDK installed.

Status:
- Part 1: GUI prototype created (non-functional front-end available in Views).
- Part 2: Claim submission, approval/rejection, file upload wiring added (in-memory storage).
- Part 3: Lecturer automation view with real-time calculation, validation service, and enhanced UX.

Automation Highlights (Part 3):
- **Automated summary** – Lecturer claim form now surfaces a live payout card that recalculates totals as soon as hours and rates are entered.
- **Server-side calculation API** – `ClaimAutomationService` enforces business limits (hours per claim, hourly rate caps, and total claim ceilings) and streams warnings/errors back to the UI.
- **Claim validation** – Controller-level validation blocks invalid submissions before files are uploaded, and tests cover happy/edge cases for the automation logic.
- **Quick-fill helpers** – Common hour/rate shortcuts reduce manual typing time for lecturers when capturing similar claims.

Refer to `POE_Part2_Report.docx` for the formal Part 2 Word document required for submission.