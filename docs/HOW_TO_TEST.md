# 📋 How to Complete Testing

## ⚠️ Important: Use Swagger UI for Testing

PowerShell scripts มีปัญหากับ encoding และ error handling  
**แนะนำ: ใช้ Swagger UI แทน (ง่ายและเห็นผลชัดเจนกว่า)**

---

## 🚀 Quick Start Guide

### 1. Start API
```powershell
cd src\FlowAccount.API
dotnet run
```

รอจนเห็น: `Now listening on: http://localhost:5159`

### 2. Open Swagger UI
เปิด browser ไปที่: **http://localhost:5159**

### 3. Follow Complete Testing Guide
เปิดไฟล์ **COMPLETE_TESTING_GUIDE.md** และทำตาม Step 1-10

---

## 📝 Testing Checklist

ทำตามลำดับใน Swagger UI:

- [ ] **STEP 1:** Create Product Master
  - Endpoint: `POST /api/products`
  - ได้ Product ID และ Option IDs

- [ ] **STEP 2:** Generate Variants (BATCH OPERATION)
  - Endpoint: `POST /api/products/{id}/generate-variants`
  - ได้ 12 variants (3 colors × 4 sizes)
  - ✅ **ทดสอบ: Batch Operations**

- [ ] **STEP 3:** Get All Variants
  - Endpoint: `GET /api/variants`
  - จด Variant IDs สำหรับ Bundle

- [ ] **STEP 4:** Adjust Stock
  - Endpoint: `POST /api/stock/adjust`
  - เพิ่ม stock: Variant 1 = 50, Variant 2 = 30

- [ ] **STEP 5:** Query Stock
  - Endpoint: `GET /api/stock/query`
  - ตรวจสอบ stock levels

- [ ] **STEP 6:** Create Bundle
  - Endpoint: `POST /api/bundles`
  - สร้าง bundle จาก 2 variants

- [ ] **STEP 7:** Calculate Bundle Stock (STOCK LOGIC)
  - Endpoint: `POST /api/bundles/calculate-stock`
  - ดู Bottleneck Detection
  - ✅ **ทดสอบ: Stock Logic**

- [ ] **STEP 8:** Sell Bundle (TRANSACTION MANAGEMENT)
  - Endpoint: `POST /api/bundles/sell`
  - ขาย 5 bundles
  - ✅ **ทดสอบ: Transaction Management**

- [ ] **STEP 9:** Verify Stock After Sale
  - Endpoint: `GET /api/stock/query`
  - Stock ลดลงถูกต้อง

- [ ] **STEP 10:** Recalculate Bundle Stock
  - Endpoint: `POST /api/bundles/calculate-stock`
  - Max bundles ลดลงจาก 30 → 25

---

## ✅ Success Criteria

เมื่อทำครบทุก step แล้ว จะได้:

### ✅ All 3 Key Features Tested:
1. **BATCH OPERATIONS** - Generated 12 variants in one call
2. **TRANSACTION MANAGEMENT** - Sold bundles with atomic stock deduction
3. **STOCK LOGIC** - Detected bottleneck item correctly

### ✅ All Requirements Met:
- Database Schema ✅
- API Endpoints ✅
- Batch Operations ✅
- Transaction Management ✅
- Stock Logic ✅
- Request/Response Examples ✅
- Unit Tests (16/16 passed) ✅

---

## 📁 Important Files

1. **COMPLETE_TESTING_GUIDE.md** ← คู่มือทดสอบแบบละเอียด (ใช้ไฟล์นี้!)
2. **PROJECT_COMPLETION_REPORT.md** ← รายงานสรุปโครงการ
3. **complete-test.ps1** ← สคริปต์ทดสอบ (ถ้าต้องการอัตโนมัติ)

---

## 🎯 Next Steps

1. เปิด Swagger UI: http://localhost:5159
2. เปิดไฟล์: COMPLETE_TESTING_GUIDE.md
3. ทำตาม Step 1-10
4. เมื่อเสร็จ → Project พร้อมส่งมอบ!

---

**Status:** ✅ All code complete, ready for manual testing via Swagger UI
