# FastFood — ASP.NET Core MVC

![CI](https://github.com/EyaZouini/FastFood/actions/workflows/ci.yml/badge.svg)

A full-stack food ordering web application, used as a hands-on experiment in **agentic AI-assisted software development**.

---

## What this project is about

This is an academic project I built two years ago with ASP.NET Core MVC. I recently picked it back up — not to add features, but to use it as a real, existing codebase to experiment with **agentic AI**: I handed it to an AI agent (Claude Code) and asked it to audit, improve, and extend it — treating the AI as an autonomous engineer rather than a code autocomplete tool.

---

## What is Agentic AI development?

Standard AI usage means asking a question and getting an answer — one turn, one output.

**Agentic AI** is different. The AI:
- Holds a plan across multiple steps and sessions
- Uses real tools: reads and writes files, runs shell commands, executes tests, interacts with git and GitHub
- Makes decisions and recovers from errors autonomously
- Maintains context and state between work sessions

This project is a concrete demonstration of what that looks like on a real codebase.

---

## What the AI agent did

The project started as a basic scaffold with no tests, no architecture, security issues, and a bare-bones UI. The AI agent worked through a structured task list over multiple sessions:

| Task | What was done |
|------|--------------|
| **1.1 Security & quality** | Found and fixed publicly accessible admin routes, null-crash bugs, duplicated claims logic, async/sync mismatches |
| **1.2 Test infrastructure** | Set up xUnit + Moq + EF InMemory test project from scratch, wired into the solution |
| **1.3 Nullable safety** | Eliminated all C# nullable warnings across models, ViewModels, and controllers |
| **1.4 Service layer** | Extracted all business logic from controllers into typed services (`IItemService`, `ICartService`, `IOrderService`) — controllers went from 200+ lines to under 40 |
| **1.5 Admin dashboard** | Designed and built a full analytics dashboard: KPI cards, Chart.js revenue and order charts, top items — backed by a dedicated `IDashboardService` |
| **2 Data seeding** | Scraped and curated 24 food items into a CSV, built an idempotent `CsvSeeder` that creates categories and items on startup |
| **2.2 Full DB seed** | Built `OrderSeeder`: demo users, coupons (WELCOME10, SUMMER20, SAVE5), and 8 realistic orders spread over the last 7 days so the dashboard shows real chart data. Also diagnosed and fixed 4 broken image URLs by running HTTP HEAD requests against every image in the dataset |
| **3 UI redesign** | Complete visual overhaul: CSS design token system, Bootstrap primary color override, 15+ views rewritten, all per-page `<style>` blocks removed |
| **Refactoring** | Moved `ViewModels/` into `Models/ViewModels/` to reduce top-level folder clutter; sticky footer fix |

Every task followed the same discipline: **create branch → implement → run tests → commit → open PR → merge**.

---

## What makes this a strong agentic AI example

**1. Multi-session continuity**
Work ran across 3+ conversations. The agent maintained a `TASKS.md` (what's pending) and `CHANGELOG.md` (what's done), and used a persistent memory system — so each session resumed without re-explaining the project.

**2. Real tool use, not just text generation**
The agent read and wrote source files, ran `dotnet build`, `dotnet test`, `git`, and the GitHub CLI to create and merge pull requests — the same tools a human engineer would use.

**3. Proactive problem-solving**
Several issues were caught that were never in the original task:
- N+1 database queries in `CartService` and `OrderService`
- 4 broken Unsplash CDN image URLs (verified by running automated HEAD requests against all 24 images)
- `Items/Index` displaying a SubCategory ID instead of the title
- Broken HTML in the footer (unclosed tags)

**4. Architectural reasoning**
The agent didn't just execute instructions — it proposed the service layer pattern, designed the CSS token system, argued for the `Models/ViewModels/` consolidation, and explained the trade-offs before implementing.

**5. Error recovery**
When tests failed mid-session (e.g., an `OrderSeeder` test asserting totals on orders with missing items), the agent diagnosed the root cause, explained it, and fixed it with a targeted correction rather than a workaround.

---

## Tech stack

- **Framework:** ASP.NET Core MVC (.NET 9.0)
- **ORM:** Entity Framework Core 9.0 + SQL Server LocalDB
- **Auth:** ASP.NET Core Identity (Admin / Manager / Customer roles)
- **Frontend:** Bootstrap 5.3.3, Chart.js 4.4.0, Font Awesome 5
- **Tests:** xUnit, Moq, EF Core InMemory — 34 tests, 0 failures

---

## Project structure

```
Controllers/          ← thin controllers, delegate to services
Models/
  ├── *.cs            ← EF entities
  └── ViewModels/     ← view-specific DTOs
Services/             ← all business logic (interfaces + implementations)
Data/                 ← DbContext, CsvSeeder, OrderSeeder, DbInitializer
Utility/              ← ClaimsHelper, OrderStatus, PaymentStatus
Views/                ← Razor views (no per-page <style> blocks)
wwwroot/css/site.css  ← single design token system for the whole app
FastFood.Tests/       ← 34 unit tests
```

---

## Running the project

```bash
# Prerequisites: .NET 9 SDK, SQL Server LocalDB

dotnet restore
dotnet run
```

The database is created and seeded automatically on first launch:
- **Admin:** `admin@gmail.com` / `Admin@123`
- **Manager:** `manager@fastfood.com` / `Manager@123`
- 24 food items across 4 categories, 3 coupons, 8 demo orders

---

## Running the tests

```bash
dotnet test
# Result: 34 passed, 0 failed
```
