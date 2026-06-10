# Changelog

> All completed changes, in reverse chronological order.

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
