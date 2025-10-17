# 📊 Flow Diagrams

## Visual Process Flows for Key Features

This document provides **visual flow diagrams** for complex processes in the FlowAccount system.

---

## 🎯 1. Generate Variants Flow

**Process:** Batch generation of product variants from attribute combinations

```
┌─────────────────────────────────────────────────────────────────────┐
│                     GENERATE VARIANTS FLOW                          │
└─────────────────────────────────────────────────────────────────────┘

1. API Request
   ┌──────────────────────────┐
   │ POST /api/products/      │
   │   generate-variants      │
   │                          │
   │ Body:                    │
   │  ProductMasterId: 1      │
   │  Attributes: [           │
   │    {Size: [S,M,L]},      │
   │    {Color: [Red,Blue]}   │
   │  ]                       │
   └──────────┬───────────────┘
              │
              ▼
2. Controller → Service
   ┌──────────────────────────┐
   │ ProductsController       │
   │  ▶ GenerateVariants()    │
   └──────────┬───────────────┘
              │
              ▼
3. Validation
   ┌──────────────────────────┐
   │ ProductService           │
   │  ▶ ValidateRequest()     │
   │    - Check ProductMaster │
   │      exists              │
   │    - Validate attributes │
   └──────────┬───────────────┘
              │
              ├─── ❌ Validation Fails
              │    └─→ Return 404 Not Found
              │
              ▼ ✅ Validation Passes
4. Calculate Combinations
   ┌──────────────────────────┐
   │ VariantGenerator         │
   │  ▶ GenerateCombinations()│
   │                          │
   │  Cartesian Product:      │
   │  Size × Color            │
   │  = [S,Red], [S,Blue],    │
   │    [M,Red], [M,Blue],    │
   │    [L,Red], [L,Blue]     │
   │                          │
   │  Result: 6 variants      │
   └──────────┬───────────────┘
              │
              ▼
5. Create Entities
   ┌──────────────────────────┐
   │ ProductService           │
   │  ▶ CreateVariantEntities│
   │                          │
   │  For each combination:   │
   │    - Create ProductVariant│
   │    - SKU = "PM001-S-Red" │
   │    - Create Attributes   │
   └──────────┬───────────────┘
              │
              ▼
6. Database Transaction
   ┌──────────────────────────┐
   │ BEGIN TRANSACTION        │
   └──────────┬───────────────┘
              │
              ▼
   ┌──────────────────────────┐
   │ Repository.AddRangeAsync │
   │  - Bulk insert 6 variants│
   │  - Bulk insert attributes│
   └──────────┬───────────────┘
              │
              ├─── ❌ Insert Fails
              │    ┌──────────────┐
              │    │ ROLLBACK     │
              │    │ Return 500   │
              │    └──────────────┘
              │
              ▼ ✅ Insert Succeeds
   ┌──────────────────────────┐
   │ COMMIT TRANSACTION       │
   └──────────┬───────────────┘
              │
              ▼
7. Map to DTOs
   ┌──────────────────────────┐
   │ AutoMapper               │
   │  - Map entities → DTOs   │
   │  - Include attributes    │
   └──────────┬───────────────┘
              │
              ▼
8. Return Response
   ┌──────────────────────────┐
   │ 200 OK                   │
   │                          │
   │ {                        │
   │   "totalCount": 6,       │
   │   "variants": [...]      │
   │ }                        │
   └──────────────────────────┘
```

**Performance:**
- 6 variants: ~200ms
- 250 variants: ~2,044ms (tested)

---

## 📦 2. Bundle Stock Calculation Flow

**Process:** Real-time calculation of available bundle stock based on component inventory

```
┌─────────────────────────────────────────────────────────────────────┐
│                  BUNDLE STOCK CALCULATION FLOW                      │
└─────────────────────────────────────────────────────────────────────┘

1. API Request
   ┌──────────────────────────┐
   │ GET /api/bundles/1/stock │
   └──────────┬───────────────┘
              │
              ▼
2. Load Bundle with Items
   ┌──────────────────────────┐
   │ BundleRepository         │
   │  ▶ GetByIdWithItemsAsync │
   │                          │
   │  SQL: JOIN BundleItem    │
   │       JOIN ProductVariant│
   │                          │
   │  Result:                 │
   │   Bundle: "Startup Kit"  │
   │   Items:                 │
   │    - Variant A, Qty: 2   │
   │    - Variant B, Qty: 1   │
   │    - Variant C, Qty: 3   │
   └──────────┬───────────────┘
              │
              ├─── ❌ Bundle Not Found
              │    └─→ Return 404 Not Found
              │
              ▼ ✅ Bundle Found
3. Fetch Current Stock
   ┌──────────────────────────┐
   │ ProductVariantRepository │
   │                          │
   │  Variant A: Stock = 100  │
   │  Variant B: Stock = 50   │
   │  Variant C: Stock = 60   │
   └──────────┬───────────────┘
              │
              ▼
4. Calculate Per-Item Possible Bundles
   ┌──────────────────────────┐
   │ BundleService            │
   │  ▶ CalculateStock()      │
   │                          │
   │  Formula for each item:  │
   │  Possible = Stock / Qty  │
   │                          │
   │  Item A: 100 / 2 = 50    │
   │  Item B: 50 / 1  = 50    │
   │  Item C: 60 / 3  = 20 ← BOTTLENECK │
   └──────────┬───────────────┘
              │
              ▼
5. Find Minimum (Bottleneck)
   ┌──────────────────────────┐
   │ MIN(50, 50, 20) = 20     │
   │                          │
   │ Bundle Stock = 20        │
   │ Bottleneck = Variant C   │
   └──────────┬───────────────┘
              │
              ▼
6. Return Response
   ┌──────────────────────────┐
   │ 200 OK                   │
   │                          │
   │ {                        │
   │   "bundleId": 1,         │
   │   "availableStock": 20,  │
   │   "bottleneckItem": {    │
   │     "variantId": 3,      │
   │     "name": "Variant C", │
   │     "stock": 60,         │
   │     "required": 3        │
   │   }                      │
   │ }                        │
   └──────────────────────────┘
```

**Key Points:**
- **Real-time:** No caching, always accurate
- **Performance:** ~100ms with proper indexes
- **Accuracy:** Prevents overselling

---

## 🛒 3. Bundle Sale with Stock Deduction Flow

**Process:** Sell a bundle and deduct stock atomically with transaction rollback

```
┌─────────────────────────────────────────────────────────────────────┐
│              BUNDLE SALE WITH TRANSACTION FLOW                      │
└─────────────────────────────────────────────────────────────────────┘

1. API Request
   ┌──────────────────────────┐
   │ POST /api/bundles/       │
   │      1/sell              │
   │                          │
   │ Body:                    │
   │  {                       │
   │    "quantity": 2,        │
   │    "idempotencyKey":     │
   │      "sale-12345"        │
   │  }                       │
   └──────────┬───────────────┘
              │
              ▼
2. Check Idempotency
   ┌──────────────────────────┐
   │ BundleService            │
   │  ▶ CheckIdempotencyKey() │
   │                          │
   │  Query: SELECT * FROM    │
   │   BundleSales WHERE      │
   │   IdempotencyKey =       │
   │   "sale-12345"           │
   └──────────┬───────────────┘
              │
              ├─── ✅ Key Exists (Already Processed)
              │    └─→ Return 200 OK (cached result)
              │
              ▼ ❌ Key Not Found (New Request)
3. BEGIN TRANSACTION
   ┌──────────────────────────┐
   │ UnitOfWork.BeginTransaction()│
   │                          │
   │ Isolation Level:         │
   │  READ COMMITTED          │
   └──────────┬───────────────┘
              │
              ▼
4. Load Bundle + Items (with Lock)
   ┌──────────────────────────┐
   │ BundleRepository         │
   │  ▶ GetByIdWithItemsAsync │
   │                          │
   │  Items:                  │
   │   - Variant A: Qty 2     │
   │   - Variant B: Qty 1     │
   │   - Variant C: Qty 3     │
   └──────────┬───────────────┘
              │
              ▼
5. Load Variants with RowVersion
   ┌──────────────────────────┐
   │ ProductVariantRepository │
   │                          │
   │  Variant A:              │
   │   Stock = 100            │
   │   RowVersion = 0x123...  │
   │                          │
   │  Variant B:              │
   │   Stock = 50             │
   │   RowVersion = 0x456...  │
   │                          │
   │  Variant C:              │
   │   Stock = 60             │
   │   RowVersion = 0x789...  │
   └──────────┬───────────────┘
              │
              ▼
6. Calculate Required Stock
   ┌──────────────────────────┐
   │ BundleService            │
   │  ▶ CalculateRequired()   │
   │                          │
   │  Selling 2 bundles:      │
   │   - Variant A: 2 × 2 = 4 │
   │   - Variant B: 1 × 2 = 2 │
   │   - Variant C: 3 × 2 = 6 │
   └──────────┬───────────────┘
              │
              ▼
7. Validate Sufficient Stock
   ┌──────────────────────────┐
   │ BundleService            │
   │  ▶ ValidateStock()       │
   │                          │
   │  Check:                  │
   │   A: 100 >= 4 ✅         │
   │   B: 50 >= 2 ✅          │
   │   C: 60 >= 6 ✅          │
   └──────────┬───────────────┘
              │
              ├─── ❌ Insufficient Stock
              │    ┌──────────────┐
              │    │ ROLLBACK     │
              │    │ Return 409   │
              │    │  Conflict    │
              │    └──────────────┘
              │
              ▼ ✅ Stock Sufficient
8. Deduct Stock
   ┌──────────────────────────┐
   │ ProductVariantRepository │
   │  ▶ UpdateAsync()         │
   │                          │
   │  Variant A:              │
   │   Stock = 100 - 4 = 96   │
   │   RowVersion++           │
   │                          │
   │  Variant B:              │
   │   Stock = 50 - 2 = 48    │
   │   RowVersion++           │
   │                          │
   │  Variant C:              │
   │   Stock = 60 - 6 = 54    │
   │   RowVersion++           │
   └──────────┬───────────────┘
              │
              ▼
9. Save Changes
   ┌──────────────────────────┐
   │ UnitOfWork.SaveChanges() │
   │                          │
   │  EF Core checks:         │
   │   - RowVersion matches?  │
   └──────────┬───────────────┘
              │
              ├─── ❌ RowVersion Mismatch (Concurrency Conflict)
              │    ┌──────────────────────┐
              │    │ DbUpdateConcurrency  │
              │    │ Exception            │
              │    │                      │
              │    │ ROLLBACK             │
              │    │ Return 409 Conflict  │
              │    │ {                    │
              │    │  "error":            │
              │    │   "Concurrent update"│
              │    │  "retryable": true   │
              │    │ }                    │
              │    └──────────────────────┘
              │
              ▼ ✅ Save Succeeds
10. Record Sale
   ┌──────────────────────────┐
   │ BundleSaleRepository     │
   │  ▶ AddAsync()            │
   │                          │
   │  {                       │
   │   BundleId: 1,           │
   │   Quantity: 2,           │
   │   IdempotencyKey:        │
   │    "sale-12345",         │
   │   SaleDate: now          │
   │  }                       │
   └──────────┬───────────────┘
              │
              ▼
11. COMMIT TRANSACTION
   ┌──────────────────────────┐
   │ UnitOfWork.Commit()      │
   │                          │
   │  All changes persisted:  │
   │   - Stock deducted       │
   │   - Sale recorded        │
   │   - Idempotency saved    │
   └──────────┬───────────────┘
              │
              ▼
12. Return Response
   ┌──────────────────────────┐
   │ 200 OK                   │
   │                          │
   │ {                        │
   │   "saleId": 42,          │
   │   "bundleId": 1,         │
   │   "quantity": 2,         │
   │   "stockDeducted": {     │
   │     "variantA": 4,       │
   │     "variantB": 2,       │
   │     "variantC": 6        │
   │   }                      │
   │ }                        │
   └──────────────────────────┘
```

**Critical Points:**
- **Atomic:** All stock deductions or none (transaction)
- **Retry-safe:** Idempotency key prevents duplicate sales
- **Concurrency:** RowVersion detects conflicts, triggers rollback

---

## 🔄 4. Optimistic Concurrency Flow (RowVersion)

**Process:** Handle concurrent updates with automatic conflict detection

```
┌─────────────────────────────────────────────────────────────────────┐
│            OPTIMISTIC CONCURRENCY CONTROL FLOW                      │
└─────────────────────────────────────────────────────────────────────┘

Timeline: Two Users Update Same Variant Simultaneously
───────────────────────────────────────────────────────────────────────

TIME T0: Initial State
┌──────────────────────────┐
│ ProductVariant (ID: 1)   │
│  Name: "T-Shirt Red M"   │
│  Stock: 100              │
│  Price: 250              │
│  RowVersion: 0x0001      │
└──────────────────────────┘


TIME T1: Both Users Read Variant
┌──────────────┐          ┌──────────────┐
│  User A      │          │  User B      │
│  GET /api/   │          │  GET /api/   │
│  variants/1  │          │  variants/1  │
│              │          │              │
│  Reads:      │          │  Reads:      │
│   Stock: 100 │          │   Stock: 100 │
│   RowVersion:│          │   RowVersion:│
│   0x0001     │          │   0x0001     │
└──────────────┘          └──────────────┘


TIME T2: User A Updates First (Add 50 Stock)
┌──────────────────────────┐
│ User A                   │
│ PUT /api/variants/1      │
│                          │
│ Body:                    │
│  {                       │
│   "stock": 150,          │
│   "rowVersion": "0x0001" │
│  }                       │
└──────────┬───────────────┘
           │
           ▼
   ┌───────────────────┐
   │ BEGIN TRANSACTION │
   └────────┬──────────┘
            │
            ▼
   ┌───────────────────────┐
   │ Load Variant          │
   │  Current RowVersion:  │
   │  0x0001               │
   │                       │
   │ Match? YES ✅         │
   └────────┬──────────────┘
            │
            ▼
   ┌───────────────────────┐
   │ Update:               │
   │  Stock = 150          │
   │  RowVersion = 0x0002  │
   └────────┬──────────────┘
            │
            ▼
   ┌───────────────────┐
   │ COMMIT            │
   │ Return 200 OK     │
   └───────────────────┘

Database Now:
┌──────────────────────────┐
│ ProductVariant (ID: 1)   │
│  Stock: 150              │
│  RowVersion: 0x0002      │ ← Changed!
└──────────────────────────┘


TIME T3: User B Tries to Update (Set Price = 300)
┌──────────────────────────┐
│ User B                   │
│ PUT /api/variants/1      │
│                          │
│ Body:                    │
│  {                       │
│   "price": 300,          │
│   "rowVersion": "0x0001" │ ← OLD!
│  }                       │
└──────────┬───────────────┘
           │
           ▼
   ┌───────────────────┐
   │ BEGIN TRANSACTION │
   └────────┬──────────┘
            │
            ▼
   ┌───────────────────────┐
   │ Load Variant          │
   │  Current RowVersion:  │
   │  0x0002               │ ← Database changed!
   │                       │
   │ Expected:             │
   │  0x0001               │
   │                       │
   │ Match? NO ❌          │
   └────────┬──────────────┘
            │
            ▼
   ┌───────────────────────┐
   │ DbUpdateConcurrency   │
   │ Exception             │
   └────────┬──────────────┘
            │
            ▼
   ┌───────────────────┐
   │ ROLLBACK          │
   └────────┬──────────┘
            │
            ▼
   ┌───────────────────────┐
   │ Return 409 Conflict   │
   │                       │
   │ {                     │
   │  "error":             │
   │   "Concurrency conflict"│
   │  "message":           │
   │   "Data was modified" │
   │   "by another user",  │
   │  "retryable": true,   │
   │  "currentRowVersion": │
   │   "0x0002"            │
   │ }                     │
   └───────────────────────┘


TIME T4: User B Retries with Latest Data
┌──────────────────────────┐
│ User B                   │
│ GET /api/variants/1      │
│  (Refresh)               │
│                          │
│ Reads:                   │
│  Stock: 150              │
│  RowVersion: 0x0002      │ ← Latest
└──────────┬───────────────┘
           │
           ▼
┌──────────────────────────┐
│ PUT /api/variants/1      │
│                          │
│ Body:                    │
│  {                       │
│   "price": 300,          │
│   "rowVersion": "0x0002" │ ← Updated!
│  }                       │
└──────────┬───────────────┘
           │
           ▼
   ┌───────────────────┐
   │ Match? YES ✅     │
   │ Update succeeds   │
   │ Return 200 OK     │
   └───────────────────┘

Final State:
┌──────────────────────────┐
│ ProductVariant (ID: 1)   │
│  Stock: 150              │ ← User A's change
│  Price: 300              │ ← User B's change
│  RowVersion: 0x0003      │
└──────────────────────────┘
```

**Key Takeaways:**
- **No Lost Updates:** Both changes preserved
- **Automatic Detection:** EF Core handles RowVersion check
- **User Feedback:** 409 Conflict tells client to retry

---

## 🔄 5. Retry Flow with Exponential Backoff

**Process:** Client retries failed requests with increasing delays

```
┌─────────────────────────────────────────────────────────────────────┐
│                    RETRY FLOW (CLIENT SIDE)                         │
└─────────────────────────────────────────────────────────────────────┘

Attempt 1: Initial Request
   ┌───────────────────┐
   │ POST /api/bundles │
   │  create           │
   │                   │
   │ IdempotencyKey:   │
   │  "bundle-xyz"     │
   └────────┬──────────┘
            │
            ▼
   ┌───────────────────┐
   │ Network Timeout   │
   │ (No response)     │
   └────────┬──────────┘
            │
            ▼ Wait 1 second

Attempt 2: Retry #1
   ┌───────────────────┐
   │ POST /api/bundles │
   │  create           │
   │                   │
   │ IdempotencyKey:   │
   │  "bundle-xyz"     │ ← SAME KEY!
   └────────┬──────────┘
            │
            ▼
   ┌───────────────────┐
   │ 409 Conflict      │
   │ (Concurrency)     │
   │ Retryable: true   │
   └────────┬──────────┘
            │
            ▼ Wait 2 seconds (exponential)

Attempt 3: Retry #2
   ┌───────────────────┐
   │ POST /api/bundles │
   │  create           │
   │                   │
   │ IdempotencyKey:   │
   │  "bundle-xyz"     │ ← SAME KEY!
   └────────┬──────────┘
            │
            ▼
   ┌───────────────────┐
   │ 200 OK            │
   │ (Bundle created)  │
   └───────────────────┘

Result: ✅ Only ONE bundle created (idempotency prevented duplicates)
```

**Exponential Backoff:**
- Attempt 1: Immediate
- Attempt 2: Wait 1s
- Attempt 3: Wait 2s
- Attempt 4: Wait 4s
- Attempt 5: Wait 8s
- Max attempts: 5

---

## 🧪 6. Transaction Rollback Flow (Integration Test)

**Process:** Verify automatic rollback on error

```
┌─────────────────────────────────────────────────────────────────────┐
│                 TRANSACTION ROLLBACK TEST FLOW                      │
└─────────────────────────────────────────────────────────────────────┘

Setup: Arrange
   ┌───────────────────────┐
   │ Create Test Data      │
   │                       │
   │ ProductMaster: "Test" │
   │ Variant A: Stock 100  │
   │ Variant B: Stock 100  │
   │                       │
   │ Bundle: "Test Bundle" │
   │  - Variant A × 2      │
   │  - Variant B × 1      │
   └────────┬──────────────┘
            │
            ▼

Act: Execute Failing Operation
   ┌───────────────────────┐
   │ BEGIN TRANSACTION     │
   └────────┬──────────────┘
            │
            ▼
   ┌───────────────────────┐
   │ Deduct Stock:         │
   │  Variant A: 100 → 98  │
   └────────┬──────────────┘
            │
            ▼
   ┌───────────────────────┐
   │ Simulate Error:       │
   │  throw Exception()    │ ← Intentional error!
   └────────┬──────────────┘
            │
            ▼
   ┌───────────────────────┐
   │ ROLLBACK (Automatic)  │
   └───────────────────────┘

Assert: Verify
   ┌───────────────────────┐
   │ Query Database:       │
   │                       │
   │ Variant A: Stock = ?  │
   └────────┬──────────────┘
            │
            ▼
   ┌───────────────────────┐
   │ Expected: 100         │ ← Original value
   │ Actual: 100           │ ← Rolled back!
   │                       │
   │ Test: PASSED ✅       │
   └───────────────────────┘

Result: Proves rollback works correctly
```

**Test Evidence:** See `tests/FlowAccount.Tests/BundleServiceTests.cs`

---

## 📋 Summary

**6 Flow Diagrams Created:**

1. ✅ **Generate Variants Flow** - Batch creation with cartesian product
2. ✅ **Bundle Stock Calculation Flow** - Real-time bottleneck calculation
3. ✅ **Bundle Sale Flow** - Atomic stock deduction with transaction
4. ✅ **Optimistic Concurrency Flow** - RowVersion conflict detection
5. ✅ **Retry Flow** - Exponential backoff with idempotency
6. ✅ **Transaction Rollback Flow** - Integration test validation

**All Critical Processes Visualized** ✅

---

**Created:** October 17, 2025  
**Purpose:** Visual process flows for complex features  
**Format:** ASCII diagrams for easy viewing in any text editor
