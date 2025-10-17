# 🎥 สคริปต์อัดวิดีโออธิบายโปรเจกต์ (6-8 นาที)

> **โจทย์:** อธิบาย Project, Approach, Important Decisions  
> **ข้อกำหนด:** ใช้ AI 100%  
> **เวลา:** 6-8 นาที

---

## 🎬 สคริปต์ทั้งหมด 5 ส่วน

### ส่วนที่ 1: Intro (1 นาที)

**พูด:**
```
สวัสดีครับ/ค่ะ วันนี้จะอธิบายโปรเจกต์ FlowAccount API
ระบบจัดการสินค้า Variant และ Bundle

โจทย์มี 2 ข้อหลัก:
1️⃣ Database Schema - 10 Entities พร้อม Relationships
2️⃣ API Endpoints - CRUD + Business Logic

เทคโนโลยี:
- ASP.NET Core 8.0 Web API
- EF Core + SQL Server LocalDB  
- Clean Architecture (4 Layers)

Database 10 Entities:
📦 Product Group: ProductMaster, VariantOption, VariantOptionValue, ProductVariant
🎁 Bundle Group: Bundle, BundleItem
📊 Support Group: Stock, Warehouse, Category, ProductVariantAttribute
```

**แสดง:** 
- `DATABASE_DESIGN_DETAILED.md` (ER Diagram)
- หรือ `Domain/Entities/` folder (10 files)

---

### ส่วนที่ 2: Database Schema (1 นาที)

**พูด:**
```
ข้อแรก - Database Schema

ออกแบบ 10 Entities แบ่ง 3 กลุ่ม:

📦 Product (4 Entities):
- ProductMaster: หลักสินค้า
- VariantOption: ตัวเลือก (Color, Size)
- VariantOptionValue: ค่า (Red, Blue, S, M)  
- ProductVariant: สินค้าจริง มี SKU, ราคา

🎁 Bundle (2 Entities):
- Bundle: ชุดสินค้า
- BundleItem: รายการในชุด (Variant + Quantity)

📊 Supporting (4 Entities):
- Stock: สต็อคตาม Variant + Warehouse
- Warehouse: คลังสินค้า
- Category: หมวดหมู่
- ProductVariantAttribute: คุณสมบัติเพิ่ม

Relationships:
✅ ProductMaster 1→N Variants
✅ Bundle N↔N Variant (ผ่าน BundleItem)
✅ Foreign Keys + Indexes ครบ
✅ EF Core Configurations พร้อม
```

**แสดง:**
- ER Diagram ใน `DATABASE_DESIGN_DETAILED.md`
- ชี้แต่ละ Entity และ Relationship
- เปิด `Domain/Entities/` แสดง 10 files

---

### ส่วนที่ 3: Architecture (1 นาที)

**พูด:**
```
Clean Architecture - 4 Layers:

1. Domain Layer
   → Entities, Business Rules
   → ไม่ขึ้นกับเทคโนโลยี

2. Application Layer  
   → Services, DTOs, Use Cases
   → Business Logic หลัก

3. Infrastructure Layer
   → EF Core, Database
   → External Dependencies

4. API Layer
   → Controllers, Swagger
   → REST API Endpoints

เหตุผล:
✅ Testability สูง - Unit Test ง่าย
✅ Maintainability - แก้ไขง่าย
✅ Flexibility - เปลี่ยน DB ได้
✅ SOLID Principles
```

**แสดง:**
- Solution Explorer แสดงโครงสร้าง 4 folders
- หรือแสดง diagram

---

### ส่วนที่ 4: Demo Features (2 นาที)

#### 4.1 Batch Operations (45 วินาที)

**พูด + Demo:**
```
ฟีเจอร์ที่ 1: Batch Operations

สินค้า 1 ตัว มี:
- 3 สี (Red, Blue, Green)
- 2 ขนาด (S, M)

→ ระบบสร้าง 6 Variants อัตโนมัติ
→ ใช้ Cartesian Product Algorithm
→ รองรับสูงสุด 250 Variants
```

**Demo:**
1. เปิด Swagger (http://localhost:5159)
2. `POST /api/products` - สร้างสินค้า

**Payload ตัวอย่าง:**
```json
{
  "name": "Basic T-Shirt",
  "description": "Cotton T-Shirt with multiple colors and sizes",
  "sku": "TSHIRT-001",
  "basePrice": 299,
  "baseCost": 150,
  "categoryId": 1,
  "variantOptions": [
    {
      "name": "Color",
      "values": ["Red", "Blue", "Green"]
    },
    {
      "name": "Size",
      "values": ["S", "M"]
    }
  ]
}
```

3. `POST /api/products/{id}/generate-variants` - สร้าง Variants (ใส่ id จาก step 2)
4. แสดง Response: 6 variants (3 สี × 2 ขนาด)

---

#### 4.2 Stock Logic (45 วินาที)

**พูด + Demo:**
```
ฟีเจอร์ที่ 2: Stock Logic

Bundle มี:
- T-Shirt (Red,M) x 2 ตัว
- Hat (Blue) x 1 ใบ

สต็อค:
- T-Shirt: 10 ตัว
- Hat: 5 ใบ

→ คำนวณได้: ขายได้ 5 ชุด (จำกัดด้วย Hat)
```

**Demo:**
- `POST /api/bundles/calculate-stock`

**Payload ตัวอย่าง:**
```json
{
  "bundleId": 1,
  "warehouseId": 1,
  "quantity": 10
}
```

**Response จะแสดง:**
```json
{
  "bundleId": 1,
  "bundleName": "Summer Combo",
  "requestedQuantity": 10,
  "availableQuantity": 5,
  "canFulfill": false,
  "limitingItems": [
    {
      "variantId": 2,
      "variantName": "Hat - Blue",
      "requiredPerBundle": 1,
      "availableStock": 5,
      "maxBundlesFromThisItem": 5
    }
  ]
}
```

- แสดง Request/Response บน Swagger

---

#### 4.3 Transaction (30 วินาที)

**พูด:**
```
ฟีเจอร์ที่ 3: Transaction Management

ขาย Bundle:
1. ตรวจสอบสต็อคทั้งหมดก่อน
2. ลดสต็อคทุกตัว ใน 1 Transaction
3. ถ้าผิดพลาด → Rollback ทั้งหมด

รับประกันความถูกต้อง (ACID)
```

**แสดง:**
- โค้ด `BundleService.cs` → `SellBundleAsync()`
- ชี้ `using var transaction = ...`

---

### ส่วนที่ 5: Important Decisions (2 นาที)

**พูด:**
```
การตัดสินใจสำคัญ:

🤖 0️⃣ ใช้ AI 100% (ตามโจทย์)
   → GitHub Copilot ช่วย:
      • ออกแบบ Database Schema
      • เขียนโค้ดทั้งหมด
      • Generate Unit Tests (16/16 passed)
      • เขียน Documentation (22 ไฟล์)
   → ผลลัพธ์: Production-ready Code

1️⃣ Clean Architecture
   → Testability, Maintainability
   → AI แนะนำและช่วยสร้าง

2️⃣ Repository + Unit of Work Pattern
   → แยก Data Access จาก Business Logic
   → เปลี่ยน Database ง่าย

3️⃣ Strategy Pattern (Stock Calculation)
   → ขยายได้ง่าย (Reserved Stock, Pre-order)

4️⃣ FluentValidation + Error Handling
   → Input Validation ครบถ้วน
   → Global Exception Handler

5️⃣ SQL Server + EF Core
   → Transaction Support
   → ACID Properties

6️⃣ Extensive Testing
   → Unit Tests: 100% (16/16)
   → Integration Tests: ครบทุก Use Case
   → Max Capacity: 250 Variants
```

**แสดง:**
- `PROJECT_COMPLETION_REPORT.md`
- Test Results
- **GitHub Copilot Icon** (สำคัญ!)

---

### ส่วนที่ 6: AI Showcase (30 วินาที)

**พูด:**
```
ตัวอย่าง AI Workflow:

[แสดง VS Code + Copilot Icon]

ผมถาม: 'Create ProductService with CRUD'
→ AI สร้าง Service + Methods

ผมถาม: 'Generate unit tests'
→ AI สร้าง Test Cases ทั้งหมด

ผลลัพธ์:
✅ Code 100% AI-Assisted
✅ Clean Code, Best Practices
✅ High Test Coverage
✅ Complete Documentation
```

**แสดง:**
- GitHub Copilot Chat/Icon
- Documentation (22 ไฟล์)

---

### ส่วนที่ 7: Closing (30 วินาที)

**พูด:**
```
สรุป:
🤖 ใช้ AI 100% ตามโจทย์
✅ ข้อ 1: Database 10 Entities ครบ
✅ ข้อ 2: API 14 Endpoints พร้อม Swagger
✅ Unit Tests: 100% Pass (16/16)
✅ Documentation: 22 ไฟล์
✅ Production-Ready Quality

พิสูจน์ว่า AI สามารถช่วยพัฒนา
Software ระดับ Production ได้จริง

ขอบคุณครับ/ค่ะ
```

**แสดง:**
- Swagger UI (14 Endpoints)
- Test Results Summary
- GitHub Copilot Logo

---

## 🛠️ เตรียมตัวก่อนอัด

### 1. เตรียม Environment (5 นาที)

```powershell
# Terminal 1
cd c:\Users\Chalermphan\source\flowaccout
.\demo-reset.ps1

# Terminal 2
cd src\FlowAccount.API
dotnet run

# Browser
# เปิด http://localhost:5159
```

### 2. Checklist
- [ ] API รันสำเร็จ
- [ ] Swagger UI เปิดได้
- [ ] Database มีข้อมูล
- [ ] ปิด Notifications
- [ ] ซ่อนข้อมูลส่วนตัว
- [ ] Font ใหญ่พอ
- [ ] Theme ชัดเจน

### 3. เปิดไฟล์เตรียมไว้

```
✅ ต้องเปิด:
1. DATABASE_DESIGN_DETAILED.md (ER Diagram)
2. Domain/Entities/ folder
3. Swagger UI (localhost:5159)
4. PROJECT_COMPLETION_REPORT.md
5. VS Code (แสดง GitHub Copilot Icon)

✅ อาจเปิด:
6. BundleService.cs
7. ProductService.cs  
8. Test Results
```

---

## 🎬 เครื่องมืออัดวิดีโอ (เลือก 1)

### ✅ OBS Studio (แนะนำ - ฟรี)
- Download: https://obsproject.com/
- Screen + Webcam
- คุณภาพสูง, Export MP4

### ✅ Loom (ง่ายสุด - ฟรี 5 นาที)
- Website: https://www.loom.com/
- Record ผ่าน Browser
- ได้ Link ทันที

### ✅ Windows Game Bar (มีอยู่แล้ว)
- กด `Win + G`
- Record ทันที
- ไม่ต้องติดตั้ง

---

## ✅ DO (ควรทำ)

1. **พูดชัดเจน ช้าพอดี**
2. **เน้นเรื่อง AI ตลอดวิดีโอ** ⭐
3. **แสดง GitHub Copilot Icon**
4. **Demo จริงใน Swagger**
5. **ชี้ส่วนสำคัญด้วยเมาส์**
6. **แสดงความมั่นใจ**
7. **ใช้ภาษาง่ายๆ**

## ❌ DON'T (ไม่ควรทำ)

1. ❌ พูดเร็วเกินไป
2. ❌ เกิน 10 นาที
3. ❌ แสดงข้อมูลส่วนตัว
4. ❌ Edit ซับซ้อน
5. ❌ ลืมพูดถึง AI

---

## 🎯 จุดสำคัญที่สุด

### เนื่องจากโจทย์กำหนด "ใช้ AI 100%"

**ต้องเน้นในวิดีโอ:**

1. **แสดง GitHub Copilot**
   - Icon บน VS Code
   - Chat Panel (ถ้าได้)
   - Inline Suggestions

2. **พูดถึง AI บ่อยๆ**
   - "AI ช่วยสร้าง..."
   - "ใช้ GitHub Copilot..."
   - "AI แนะนำ Pattern..."

3. **แสดงหลักฐาน**
   - 22 Documentation Files
   - 16 Test Cases
   - Clean Architecture

4. **สรุปท้ายวิดีโอ**
   - "ใช้ AI 100% ตามโจทย์"
   - "พิสูจน์ AI พัฒนา Production Code ได้"

---

## 📤 Export & Submit

**ขนาด:** 6-8 นาที → 150-250 MB (MP4, 1080p)

**Platform:** YouTube (Unlisted), Google Drive, OneDrive

**ชื่อไฟล์:** `FlowAccount_API_Project_Demo_[YourName].mp4`

---

## ✅ Final Checklist

- [ ] **เวลา:** 6-8 นาที (ไม่เกิน 10)
- [ ] **ครบ 3 หัวข้อ:**
  - [ ] Project Overview ✅
  - [ ] Your Approach ✅
  - [ ] Important Decisions ✅
- [ ] **เน้น AI Development** ⭐ สำคัญที่สุด!
- [ ] **แสดง Database Schema** (ข้อ 1)
- [ ] **Demo API Endpoints** (ข้อ 2)
- [ ] **แสดง GitHub Copilot Icon** ✅
- [ ] **เสียงชัด** ✅
- [ ] **หน้าจอชัด** ✅
- [ ] **ไม่มีข้อมูลส่วนตัว** ✅

---

## 🚀 Timeline

### เตรียมการ (30 นาที)
- รัน API
- เปิดไฟล์ทั้งหมด
- ซ้อมพูด 1-2 รอบ

### อัดวิดีโอ (15-30 นาที)
- Record
- พูดตามสคริปต์
- Demo ทุกส่วน

### Review & Upload (15 นาที)
- ดูวิดีโอ
- เช็คเสียง + หน้าจอ
- Upload

**รวม: 1-1.5 ชั่วโมง**

---

## 💡 คำพูดสำคัญที่ต้องมี

✅ "โจทย์มี 2 ข้อหลัก: Database Schema และ API Endpoints"  
✅ "ผมใช้ GitHub Copilot ช่วยพัฒนา..."  
✅ "AI แนะนำให้ใช้ Clean Architecture..."  
✅ "Copilot ช่วย Generate Tests ทั้งหมด..."  
✅ "ใช้ AI 100% ตามโจทย์..."  
✅ "พิสูจน์ว่า AI พัฒนา Production Code ได้..."

---

## 🎓 สรุปสั้นๆ

### โครงสร้างวิดีโอ:
1. **Intro** (1 นาที) - แนะนำโปรเจกต์
2. **Database** (1 นาที) - 10 Entities + ER Diagram
3. **Architecture** (1 นาที) - Clean Architecture 4 Layers
4. **Demo** (2 นาที) - 3 Features (Batch, Stock, Transaction)
5. **Decisions** (2 นาที) - เน้น AI 100%
6. **AI Showcase** (30 วินาที) - แสดง Copilot
7. **Closing** (30 วินาที) - สรุปผลงาน

### สิ่งที่ต้องแสดง:
📊 Database ER Diagram  
🏗️ Clean Architecture  
🎯 Swagger Demo  
🤖 GitHub Copilot Icon  
✅ Test Results  

### ข้อความสำคัญสุด:
> **"โปรเจกต์นี้พัฒนาด้วย AI (GitHub Copilot) 100% ตามโจทย์  
> พิสูจน์ว่า AI สามารถช่วยพัฒนา Software ระดับ Production ได้จริง"**

---

## 🎬 พร้อมอัดแล้ว!

**คุณทำโปรเจกต์นี้ได้ดีมาก!**  
**เชื่อมั่นในผลงานของคุณ!** 🚀

**Good Luck!** 🎥✨
