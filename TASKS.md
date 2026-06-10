# FastFood — Improvement Roadmap

> Tracking file for all changes made with Claude Code. Updated before and after each action.
> Goal: Demonstrate agentic AI-assisted development — refactoring, feature building, and data seeding with Claude.

---

## Task 1 — Code Refactoring & Improvement

### Subtask 1.1 — Quick Security & Quality Fixes
**Status:** 🔲 Not started  
**Files:** CouponsController, UsersController, ApplicationDbContext, HomeController, CartsController, OrdersController

- [ ] Add `[Authorize(Roles="Admin")]` to `CouponsController`
- [ ] Extract `ClaimsHelper.GetUserId(User)` to eliminate repetition across 4 controllers
- [ ] Fix `files[0]` null guards in `CouponsController` (2 places)
- [ ] Replace `SaveChanges()` → `SaveChangesAsync()` in `UsersController`
- [ ] Remove `OrderDetailsViewModel` DbSet misuse from `ApplicationDbContext`
- [ ] Add null-propagation on claim extraction across all controllers

### Subtask 1.2 — Fix Nullable Warnings
**Status:** 🔲 Not started  
**Files:** Models/ApplicationUser.cs, Models/OrderHeader.cs, Models/Item.cs, Models/SubCategory.cs

- [ ] Add `required` keyword or `?` to all non-nullable string properties
- [ ] Ensure consistency across all models

### Subtask 1.3 — Service Layer (Repository Pattern)
**Status:** 🔲 Not started  
**Files:** New: Services/, Interfaces/ — Refactor: CartsController, OrdersController, HomeController

- [ ] Create `IItemService` + `ItemService`
- [ ] Create `IOrderService` + `OrderService`
- [ ] Create `ICartService` + `CartService`
- [ ] Wire services via DI in `Program.cs`
- [ ] Refactor controllers to use services (no direct DbContext access)

### Subtask 1.4 — Admin Dashboard (New Visible Feature)
**Status:** 🔲 Not started

- [ ] Orders per day chart
- [ ] Most popular items
- [ ] Revenue summary
- [ ] Accessible at `/Admin/Dashboard`

---

## Task 2 — Data Seeding (Scraping + CSV)
**Status:** 🔲 Not started

### Planned
- [ ] Scrape food item images (mix of international + local/Tunisian)
- [ ] Build CSV: Title, Description, Price, Category, SubCategory, ImageUrl
- [ ] Write a `CsvSeeder` that reads the CSV and populates the DB

---

## Completed
| Date | Action |
|------|--------|
| 2026-06-10 | Project setup, DB created, pushed to EyaZouini/FastFood |
| 2026-06-10 | Full code audit completed — 6 security/quality issues, 4 subtasks defined |

---

## Change Log
| Date | Subtask | Files Changed | Notes |
|------|---------|---------------|-------|
| 2026-06-10 | Setup | Migrations/, TASKS.md | Initial migration + tracking |
