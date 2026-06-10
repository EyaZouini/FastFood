# Tasks

> What needs to be done. See CHANGELOG.md for completed work.

---

## Task 1 — Code Refactoring & Improvement

### Subtask 1.1 — Security & Quality Fixes
**Status:** ✅ Done → see CHANGELOG.md

### Subtask 1.2 — Fix Nullable Warnings
**Status:** 🔲 Not started  
**Files:** `Models/ApplicationUser.cs`, `Models/OrderHeader.cs`, `Models/Item.cs`, `Models/SubCategory.cs`, `Models/Cart.cs`, `Models/Coupon.cs`, `Models/OrderDetails.cs`, `ViewModels/`

- [ ] Add `required` keyword or `?` to all non-nullable string properties in Models
- [ ] Fix nullable warnings in ViewModels
- [ ] Build must reach 0 warnings

### Subtask 1.3 — Service Layer (Repository Pattern)
**Status:** 🔲 Not started  
**Files:** New: `Services/`, `Interfaces/` — Refactor: CartsController, OrdersController, HomeController

- [ ] Create `IItemService` + `ItemService`
- [ ] Create `IOrderService` + `OrderService`
- [ ] Create `ICartService` + `CartService`
- [ ] Register services via DI in `Program.cs`
- [ ] Refactor controllers to use services (no direct DbContext access)

### Subtask 1.4 — Admin Dashboard (New Feature)
**Status:** 🔲 Not started

- [ ] Orders per day chart
- [ ] Most popular items
- [ ] Revenue summary
- [ ] Accessible at `/Admin/Dashboard`

---

## Task 2 — Data Seeding (Scraping + CSV)
**Status:** 🔲 Not started

- [ ] Scrape food item images (mix: international + Tunisian)
- [ ] Build CSV: Title, Description, Price, Category, SubCategory, ImageUrl
- [ ] Write `CsvSeeder` that reads the CSV and populates the DB
