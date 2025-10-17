# 🎥 สรุปสคริปต์การอัดวิดีโออธิบายโปรเจกต์

> **โจทย์:** อัดวิดีโอสั้นอธิบาย Project, Approach, และ Important Decisions  
> **ข้อกำหนดพิเศษ:** ใช้ AI 100% ในการพัฒนา  
> **เวลา:** 6-8 นาที

---

## 📋 โครงสร้างวิดีโอ (5 ส่วน)

### 🎬 ส่วนที่ 1: Project Introduction (1 นาที)

**พูดว่า:**
```
"สวัสดีครับ/ค่ะ วันนี้ผมจะมาอธิบายโปรเจกต์ FlowAccount API 
ที่เป็นระบบจัดการสินค้า Variant และ Bundle

โจทย์กำหนด 2 ข้อหลัก:
1️⃣ Database Schema (Backend) - 10 Entities พร้อม Relationships
2️⃣ API Endpoints (Backend) - CRUD พร้อม Business Logic

เทคโนโลยีที่ใช้:
- ASP.NET Core 8.0 Web API
- Entity Framework Core + SQL Server LocalDB
- Clean Architecture (DDD)

Database มี 10 Entities:
- ProductMaster, VariantOption, VariantOptionValue, ProductVariant
- Bundle, BundleItem
- Stock, Warehouse
- Category, ProductVariantAttribute"
```

**แสดงบนหน้าจอ:**
- เปิด `DATABASE_DESIGN_DETAILED.md` แสดง ER Diagram
- หรือเปิด `Domain/Entities/` folder แสดง 10 Entity files

---

### 🎬 ส่วนที่ 2: Architecture & Approach (1.5 นาที)

**Part 2.1: Database Schema (45 วินาที) - สำคัญ! ต้องพูด**

**พูดว่า:**
```
"ข้อแรกของโจทย์คือ Database Schema

ผมออกแบบ Database แบบ Relational 
มี 10 Entities แบ่งเป็น 3 กลุ่ม:

📦 กลุ่ม Product (4 Entities):
1. ProductMaster - หลักสินค้า
2. VariantOption - ตัวเลือก (Color, Size)
3. VariantOptionValue - ค่า (Red, Blue, S, M)
4. ProductVariant - สินค้าที่มี SKU และราคา

🎁 กลุ่ม Bundle (2 Entities):
5. Bundle - ชุดสินค้า
6. BundleItem - รายการในชุด (Variant + Quantity)

📊 กลุ่ม Supporting (4 Entities):
7. Stock - สต็อคตาม Variant + Warehouse
8. Warehouse - คลังสินค้า
9. Category - หมวดหมู่
10. ProductVariantAttribute - คุณสมบัติเพิ่มเติม

Relationships:
✅ One-to-Many: ProductMaster → Variants
✅ Many-to-Many: Bundle ↔ Variant (ผ่าน BundleItem)
✅ Foreign Keys ครบทุกความสัมพันธ์
✅ Indexes สำหรับ Performance"
```

**แสดงบนหน้าจอ:**
- เปิด `DATABASE_DESIGN_DETAILED.md` แสดง ER Diagram
- หรือเปิด `Domain/Entities/` folder แสดง 10 files
- Point ไปที่แต่ละ Entity

---

**Part 2.2: Clean Architecture (45 วินาที)**

**พูดว่า:**
```
"สำหรับ Architecture ผมเลือกใช้ Clean Architecture 
แบ่งเป็น 4 layers:

1. Domain Layer - Entities และ Business Rules
2. Application Layer - Use Cases, DTOs, Services
3. Infrastructure Layer - Database, EF Core
4. API Layer - Controllers, Swagger

เหตุผล:
✅ แยกความรับผิดชอบชัดเจน
✅ ทดสอบง่าย - ทำ Unit Test ได้
✅ เปลี่ยน Database ได้ง่าย
✅ ตรงกับ SOLID Principles"
```

**แสดงบนหน้าจอ:**
- Solution Structure ใน VS Code
```
src/
├── FlowAccount.Domain/          ← Core Business Logic
├── FlowAccount.Application/     ← Use Cases & Services
├── FlowAccount.Infrastructure/  ← Database & EF Core
└── FlowAccount.API/             ← REST API & Swagger
```

---

### 🎬 ส่วนที่ 3: Key Features Demo (2 นาที)

#### Feature 1: Batch Operations (45 วินาที)

**พูดพร้อมทำ:**
```
"ฟีเจอร์แรก คือ Batch Operations

สินค้า 1 ตัว มี 3 สี (Red, Blue, Green) และ 2 ขนาด (S, M)
→ ระบบสร้าง 6 Variants อัตโนมัติ ด้วย Cartesian Product

ผมจะ Demo จริงนะครับ..."
```

**Demo:**
1. เปิด Swagger UI (http://localhost:5159)
2. `POST /api/products` - สร้างสินค้า
3. `POST /api/products/{id}/generate-variants` - สร้าง Variants
4. แสดง Response ที่ได้ 6 variants

---

#### Feature 2: Stock Logic (45 วินาที)

**พูดพร้อมทำ:**
```
"Bundle มี:
- T-Shirt (Red, M) จำนวน 2 ตัว
- Hat (Blue) จำนวน 1 ใบ

สต็อคที่มี:
- T-Shirt มี 10 ตัว
- Hat มี 5 ใบ

ระบบคำนวณได้ว่า → ขายได้ 5 ชุด (จำกัดด้วย Hat)"
```

**Demo:**
1. `POST /api/bundles/calculate-stock`
2. แสดง Request/Response

---

#### Feature 3: Transaction Management (30 วินาที)

**พูดพร้อมทำ:**
```
"เมื่อขาย Bundle:
1. ตรวจสอบสต็อคทั้งหมดก่อน
2. ลดสต็อคทุกตัว ใน Transaction เดียว
3. ถ้ามีปัญหา → Rollback ทั้งหมด

ทำให้มั่นใจว่าข้อมูลถูกต้องเสมอ"
```

**แสดง:**
- โค้ด `BundleService.cs` → `SellBundleAsync()`
- ชี้ส่วน `using var transaction = ...`

---

### 🎬 ส่วนที่ 4: Important Decisions (2 นาที)

**พูดว่า:**
```
"การตัดสินใจสำคัญ:

🤖 0️⃣ ใช้ AI (GitHub Copilot) 100%
   → ตามโจทย์ที่กำหนด
   → ใช้ Copilot สำหรับ:
      • สร้าง Architecture และ Database Design
      • เขียนโค้ดทั้งหมด
      • Generate Unit Tests (16/16 passed)
      • เขียน Documentation ทั้ง 22 ไฟล์

1️⃣ Clean Architecture
   → Testability และ Maintainability สูง

2️⃣ Repository + Unit of Work Pattern
   → แยก Data Access Logic
   → เปลี่ยน Database ง่าย

3️⃣ Strategy Pattern สำหรับ Stock
   → ขยายได้ง่าย (Reserved Stock, Pre-order)

4️⃣ FluentValidation & Error Handling
   → Input Validation ครบถ้วน
   → Global Exception Handler

5️⃣ SQL Server + EF Core
   → Transaction Support (ACID)

6️⃣ Extensive Testing
   → Unit Tests: 100%
   → Integration Tests: ครบทุก Use Case
   → Max Capacity: 250 Variants"
```

**แสดงบนหน้าจอ:**
- `PROJECT_COMPLETION_REPORT.md`
- `AI_COPILOT_WORKFLOW.md` (ถ้ามี)
- Test Results
- **GitHub Copilot Icon ใน VS Code** ⚠️ สำคัญ!

---

### 🎬 ส่วนที่ 4.5: AI Showcase (30 วินาที - เพิ่มเติม)

**พูดและแสดง:**
```
"ตัวอย่าง AI Workflow:

[แสดง VS Code พร้อม Copilot]

ผมถาม: 'Create ProductService with CRUD'
→ AI สร้าง Service Class พร้อม Methods

ผมถาม: 'Generate unit tests'
→ AI สร้าง Test Cases ทั้งหมด

ผลลัพธ์:
✅ โค้ด 100% AI-Assisted
✅ Clean Code ตาม Best Practices
✅ Test Coverage สูง
✅ Documentation ครบถ้วน"
```

**แสดง:**
- VS Code + GitHub Copilot Icon
- Copilot Chat Panel (ถ้าเปิดได้)
- Inline Suggestions (สีเทา)
- Documentation Files (22 ไฟล์)

---

### 🎬 ส่วนที่ 5: Closing (30 วินาที)

**พูดว่า:**
```
"สรุปผลงาน:
🤖 ใช้ AI 100% ตามโจทย์
✅ ครบทุก Requirements
✅ Database Design สมบูรณ์
✅ 14 API Endpoints พร้อม Swagger
✅ Unit Tests 100% Pass (16/16)
✅ Documentation 22 ไฟล์
✅ Production-ready Quality
✅ AI-Powered Development

พิสูจน์ว่า AI สามารถช่วยพัฒนา
Software ระดับ Production ได้จริง

ขอบคุณครับ/ค่ะ"
```

**แสดง:**
- Swagger UI พร้อม Endpoints
- Test Results Summary
- GitHub Copilot Logo

---

## 🛠️ การเตรียมตัว

### ก่อนอัดวิดีโอ (10 นาที)

```powershell
# 1. รีเซ็ต Database
cd c:\Users\Chalermphan\source\flowaccout
.\demo-reset.ps1

# 2. รัน API
cd src\FlowAccount.API
dotnet run

# 3. เปิด Browser → http://localhost:5159
```

### Checklist:
- [ ] API รันสำเร็จ
- [ ] Swagger UI เปิดได้
- [ ] Database มีข้อมูล
- [ ] ปิด Notifications ทั้งหมด
- [ ] ซ่อนข้อมูลส่วนตัว
- [ ] ขนาดตัวอักษรใหญ่พอ
- [ ] Theme ชัดเจน

---

## 🎬 เครื่องมืออัดวิดีโอ (เลือก 1 อย่าง)

### ✅ แนะนำ: OBS Studio (ฟรี)
- ดาวน์โหลด: https://obsproject.com/
- Screen Recording + Webcam
- คุณภาพสูง
- Export เป็น MP4

### ✅ ง่าย: Loom
- เว็บไซต์: https://www.loom.com/
- ฟรี 5 นาที/วิดีโอ
- Record ผ่าน Browser
- ได้ Link ทันที

### ✅ มีอยู่แล้ว: Windows Game Bar
- กด `Win + G`
- Record หน้าจอได้เลย
- ไม่ต้องติดตั้ง

---

## 📝 ไฟล์ที่ต้องเปิดไว้

```
✅ ต้องแสดง (สำคัญมาก):
1. DATABASE_DESIGN_DETAILED.md (ER Diagram) ⭐ สำหรับข้อ 1
2. Domain/Entities/ folder (แสดง 10 Entity files)
3. Swagger UI (http://localhost:5159) ⭐ สำหรับข้อ 2
4. PROJECT_COMPLETION_REPORT.md (Checklist)

✅ อาจแสดง:
5. Infrastructure/Data/Configurations/ (EF Core configs)
6. BundleService.cs (Transaction code)
7. ProductService.cs (Batch generation)
8. Test Results
9. AI_COPILOT_WORKFLOW.md
```

---

## 📊 การแสดง Database Schema (ข้อ 1)

### วิธีแสดงให้เห็น Database Design:

**Option 1: แสดง ER Diagram (แนะนำ)**
- เปิด `docs/DATABASE_DESIGN_DETAILED.md`
- Scroll ไปที่ ER Diagram ด้านบน
- อธิบายความสัมพันธ์แต่ละ Entity

**Option 2: แสดง Entity Files**
- เปิด `src/FlowAccount.Domain/Entities/`
- แสดง 10 ไฟล์:
  1. ProductMaster.cs
  2. VariantOption.cs
  3. VariantOptionValue.cs
  4. ProductVariant.cs
  5. Bundle.cs
  6. BundleItem.cs
  7. Stock.cs
  8. Warehouse.cs
  9. Category.cs
  10. ProductVariantAttribute.cs

**Option 3: แสดง EF Core Configuration**
- เปิด `src/FlowAccount.Infrastructure/Data/Configurations/`
- แสดง ProductMasterConfiguration.cs
- อธิบาย Foreign Keys, Indexes

**💡 คำพูดที่ควรเน้น:**
```
"ข้อแรกของโจทย์ Database Schema - ผมออกแบบ 10 Entities ครบ
พร้อม Primary Keys, Foreign Keys, และ Indexes
ใช้ EF Core สำหรับ Database Mapping
AI ช่วยออกแบบ Relationships ให้ถูกต้อง"
```

---

## ✅ DO's (ควรทำ)

1. **พูดชัดเจน ช้าพอสมควร**
2. **แสดงความมั่นใจ**
3. **Demo ใน Swagger จริงๆ**
4. **เน้นเรื่อง AI ตลอดวิดีโอ** ⭐ สำคัญที่สุด!
5. **แสดง GitHub Copilot Icon/Panel**
6. **ใช้ภาษาง่ายๆ เข้าใจง่าย**
7. **ใช้เมาส์ชี้ส่วนสำคัญ**

---

## ❌ DON'T's (ไม่ควรทำ)

1. ❌ พูดเร็วเกินไป
2. ❌ ใช้เวลานานเกิน 10 นาที
3. ❌ แสดงข้อมูลส่วนตัว (API Keys, Passwords)
4. ❌ Edit วิดีโอซับซ้อนเกินไป
5. ❌ ลืมพูดถึง AI Development

---

## 🎯 จุดเน้นสำคัญที่สุด

### ⚠️ เนื่องจากโจทย์กำหนดให้ใช้ AI 100%

**ต้องเน้นย้ำในวิดีโอ:**

1. **แสดง GitHub Copilot ในหน้าจอ**
   - Icon บน VS Code
   - Chat Panel (ถ้าเปิดได้)
   - Inline Suggestions

2. **พูดถึง AI บ่อยๆ**
   - "AI ช่วยสร้าง..."
   - "ใช้ GitHub Copilot..."
   - "AI แนะนำ Pattern นี้..."

3. **แสดงหลักฐาน AI**
   - Documentation 22 ไฟล์ (AI สร้าง)
   - Test Cases (AI Generate)
   - Clean Architecture (AI แนะนำ)

4. **สรุปท้ายวิดีโอ**
   - "ใช้ AI 100% ตามโจทย์"
   - "พิสูจน์ AI พัฒนา Production Code ได้"

---

## 📤 Export & Submit

### ขนาดไฟล์:
- 6-8 นาที → ประมาณ 150-250 MB (MP4, 1080p)

### Platform:
- YouTube (Unlisted)
- Google Drive
- OneDrive

### ชื่อไฟล์:
```
FlowAccount_API_Project_Demo_[YourName].mp4
```

---

## ✅ Final Checklist ก่อนส่ง

- [ ] **เวลา:** 6-8 นาที (ไม่เกิน 10 นาที)
- [ ] **ครบ 3 หัวข้อ:**
  - [ ] Project Overview ✅
  - [ ] Your Approach ✅
  - [ ] Important Decisions ✅
- [ ] **เน้น AI Development** ⭐ สำคัญที่สุด!
- [ ] **Demo จริงใน Swagger** ✅
- [ ] **อธิบาย Architecture** ✅
- [ ] **แสดง GitHub Copilot** ✅
- [ ] **เสียงชัด** ได้ยินทุกคำ ✅
- [ ] **หน้าจอชัด** อ่านโค้ดได้ ✅
- [ ] **ไม่มีข้อมูลส่วนตัว** ✅
- [ ] **ไฟล์เปิดได้** ✅

---

## 🚀 Step-by-Step Process

### 1. เตรียมการ (30 นาที)
- [ ] รัน API
- [ ] เปิดไฟล์ทั้งหมด
- [ ] เขียนสคริปต์
- [ ] ซ้อมพูด 1-2 รอบ

### 2. อัดวิดีโอ (15-30 นาที)
- [ ] กด Record
- [ ] พูดตามสคริปต์
- [ ] Demo ทุกส่วน
- [ ] ปิด Record

### 3. Review (5 นาที)
- [ ] ดูวิดีโอทั้งหมด
- [ ] เช็คเสียง + หน้าจอ
- [ ] อัดใหม่ถ้าจำเป็น

### 4. Upload & Submit (10 นาที)
- [ ] Export เป็น MP4
- [ ] Upload ไปยัง Platform
- [ ] ส่ง Link

**เวลารวม: ประมาณ 1-1.5 ชั่วโมง**

---

## 💡 Quick Tips

### เวลาพูดในวิดีโอ:

✅ "ผมใช้ GitHub Copilot ช่วยพัฒนา..."  
✅ "AI แนะนำให้ใช้ Clean Architecture..."  
✅ "Copilot ช่วย Generate Test Cases ทั้งหมด..."  
✅ "ใช้ AI 100% ตามโจทย์..."  
✅ "พิสูจน์ว่า AI สามารถพัฒนา Production Code ได้..."

### เวลาแสดงหน้าจอ:

📱 แสดง GitHub Copilot Icon  
📱 แสดง Swagger UI (Demo จริง)  
📱 แสดง Architecture (4 Layers)  
📱 แสดง Test Results (16/16)  
📱 แสดง Documentation (22 Files)

---

## 🎓 สรุป

### สิ่งที่ต้องทำ:
1. ✅ เตรียม Environment
2. ✅ เขียนสคริปต์
3. ✅ ซ้อมพูด
4. ✅ อัดวิดีโอ 6-8 นาที
5. ✅ **เน้น AI Development** ⭐
6. ✅ Review & Upload

### ข้อความสำคัญที่ต้องพูด:
> **"โปรเจกต์นี้พัฒนาด้วย AI (GitHub Copilot) 100% ตามโจทย์ที่กำหนด  
> พิสูจน์ให้เห็นว่า AI สามารถช่วยพัฒนา Software ระดับ Production ได้จริง"**

---

## 🎬 Good Luck!

**คุณทำโปรเจกต์นี้ได้ดีมาก!**  
**เชื่อมั่นในผลงานและ AI Workflow ของคุณ!** 🚀

พร้อมอัดวิดีโอแล้ว! 🎥✨
