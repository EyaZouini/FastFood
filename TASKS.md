# Tasks

> What needs to be done. See CHANGELOG.md for completed work.
> **Rule:** after every subtask, tests must pass before merging.

---

## Task 1 — Code Refactoring & Improvement

### Subtask 1.1 — Security & Quality Fixes
**Status:** ✅ Done → see CHANGELOG.md

### Subtask 1.2 — Set Up Test Project
**Status:** 🔲 Not started  
**Why first:** Tests must exist before we refactor — they catch regressions on every future subtask.

- [ ] Create `FastFood.Tests` xUnit project
- [ ] Add to solution
- [ ] Install: `xunit`, `Moq`, `Microsoft.EntityFrameworkCore.InMemory`
- [ ] Write first tests: `ClaimsHelperTests` (already built in 1.1)
- [ ] Write model validation tests (Item, Coupon, OrderHeader)
- [ ] Confirm `dotnet test` passes — becomes the gate before every merge

### Subtask 1.3 — Fix Nullable Warnings
**Status:** 🔲 Not started  
**Files:** `Models/`, `ViewModels/`

- [ ] Add `required` or `?` to all non-nullable string properties in Models
- [ ] Fix nullable warnings in ViewModels
- [ ] Build must reach 0 warnings
- [ ] Run tests ✅

### Subtask 1.4 — Service Layer (Repository Pattern)
**Status:** 🔲 Not started  
**Files:** New: `Services/`, `Interfaces/` — Refactor: CartsController, OrdersController, HomeController

- [ ] Create `IItemService` + `ItemService`
- [ ] Create `IOrderService` + `OrderService`
- [ ] Create `ICartService` + `CartService`
- [ ] Register services via DI in `Program.cs`
- [ ] Refactor controllers to use services (no direct DbContext access)
- [ ] Write service unit tests using InMemory DB
- [ ] Run tests ✅

### Subtask 1.5 — Admin Dashboard (New Feature)
**Status:** 🔲 Not started

- [ ] Orders per day chart
- [ ] Most popular items
- [ ] Revenue summary
- [ ] Accessible at `/Admin/Dashboard`
- [ ] Run tests ✅

---

## Task 2 — Data Seeding (Scraping + CSV)
**Status:** 🔲 Not started

- [ ] Scrape food item images (mix: international + Tunisian)
- [ ] Build CSV: Title, Description, Price, Category, SubCategory, ImageUrl
- [ ] Write `CsvSeeder` that reads the CSV and populates the DB
- [ ] Run tests ✅
