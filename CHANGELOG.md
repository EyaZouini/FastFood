# Changelog

> All completed changes, in reverse chronological order.
> Tests must pass before every merge — see `FastFood.Tests/`.

---

## [2026-06-10] Subtask 1.3 — Fix Nullable Warnings
**Branch:** `feature/subtask-1.3-nullable-fixes`  
**Files touched:** All 7 models, 3 ViewModels, `Controllers/CartsController.cs`

### What changed
- **Models** — applied `required` to mandatory string properties (`Name`, `Title`, `Description`, `Phone`, etc.) and `= null!` to EF navigation properties loaded via `Include` (`Item`, `SubCategory`, `ApplicationUser`, `OrderHeader`, `Category`)
- **`Coupon.CouponPicture`** — defaulted to `Array.Empty<byte>()` instead of null
- **`CouponViewModel.CouponPicture`** — changed to `IFormFile?` (optional on edit)
- **ViewModels** — `= null!` on required references, `= new()` on `ListofCart`
- **`CartsController.Summary`** — refactored to load user first, then build the full `OrderHeader` in a single initializer (also fixes a N+1 query)
- **Build result:** 0 errors, 0 warnings ✅
- **Tests:** 9/9 passing ✅

---

## [2026-06-10] Subtask 1.2 — Test Project Setup
**Branch:** `feature/subtask-1.2-test-setup`  
**Files touched:** `FastFood.Tests/` (new project), `FastFood.csproj`, `FastFood.sln`

### What changed
- Created `FastFood.Tests` xUnit project inside the repo
- Added to `FastFood.sln`
- Installed packages: `Moq 4.20.72`, `Microsoft.EntityFrameworkCore.InMemory 9.0.0`
- Added project reference to main `FastFood.csproj`
- Excluded `FastFood.Tests/` from main project compilation (SDK wildcard fix)
- **9 tests written and passing:**
  - `ClaimsHelperTests` — 3 tests (returns userId, empty string on missing claim, empty string on null identity)
  - `ItemValidationTests` — 2 tests (valid item, zero price allowed)
  - `CouponValidationTests` — 2 tests (valid coupon, IsActive defaults to false)
  - `OrderHeaderValidationTests` — 2 tests (total calculation, nullable optional fields)

---

## [2026-06-10] Subtask 1.1 — Security & Quality Fixes
**Commit:** `0e4bdea`  
**Files touched:** `Controllers/CouponsController.cs`, `Controllers/UsersController.cs`, `Controllers/HomeController.cs`, `Controllers/CartsController.cs`, `Controllers/OrdersController.cs`, `Data/ApplicationDbContext.cs`, `Utility/ClaimsHelper.cs`

### What changed
- **Security:** Added `[Authorize(Roles="Admin")]` to `CouponsController` — all CRUD actions were publicly accessible before
- **New helper:** Created `Utility/ClaimsHelper.GetUserId(User)` — replaces 4 copies of repeated claims extraction logic across controllers
- **Null guard:** `CouponsController.Create` — `files[0]` was crashing if no file was uploaded; now guarded
- **Edit fix:** `CouponsController.Edit` — if no new image is uploaded, keeps the existing picture instead of crashing
- **Null check:** `HomeController.Details` — returns `NotFound()` if item doesn't exist (was crashing before)
- **Async fix:** `UsersController.DeleteConfirmed` — was calling `SaveChanges()` synchronously; changed to `SaveChangesAsync()`
- **DbContext cleanup:** Removed `OrderDetailsViewModel` DbSet — it's a ViewModel, not an entity; had no business being in the DbContext

---

## [2026-06-10] Project Setup
**Commit:** `5a79b83`  
**Files touched:** `Migrations/`

### What changed
- Created EF Core `InitialCreate` migration
- Applied migration — `FastFoodDB` created with all tables (Users, Items, Categories, SubCategories, Carts, Orders, Coupons)
- Seed runs on startup: roles (Admin, Manager, Customer) + default admin account (`admin@gmail.com` / `Admin@123`)
- Pushed project to private repo `EyaZouini/FastFood`
