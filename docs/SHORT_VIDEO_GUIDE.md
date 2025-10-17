# 🎥 Short Video Recording Guide

## คู่มือการอัดวิดีโอสั้นอธิบายโปรเจกต์

---

## 📝 เนื้อหาที่ต้องอธิบาย (ตามที่โจทย์กำหนด)

1. **Project Overview** - โปรเจกต์ที่พัฒนาคืออะไร
2. **Your Approach** - แนวทางการพัฒนา
3. **Important Decisions** - การตัดสินใจสำคัญๆ

---

## ⏱️ โครงสร้างวิดีโอที่แนะนำ (6-8 นาที)

> **สำคัญ:** เน้นย้ำเรื่อง AI Development ตลอดวิดีโอ เพราะโจทย์ระบุให้ใช้ AI 100%

### 🎬 ส่วนที่ 1: Project Introduction (1 นาที)

**สิ่งที่ต้องพูด:**
```
"สวัสดีครับ/ค่ะ วันนี้ผมจะมาอธิบายโปรเจกต์ FlowAccount API 
ที่เป็นระบบจัดการสินค้า Variant และ Bundle 

โปรเจกต์นี้พัฒนาด้วย:
- ASP.NET Core 8.0 Web API
- Entity Framework Core
- SQL Server LocalDB
- Clean Architecture (Domain-Driven Design)

มี 3 ส่วนหลัก:
1. Product Master & Variants - จัดการสินค้าและตัวเลือก (สี, ขนาด)
2. Product Bundle - สร้างชุดสินค้า
3. Stock Management - จัดการสต็อคแบบ Transaction"
```

**สิ่งที่ต้องแสดงบนหน้าจอ:**
- เปิด `PROJECT_COMPLETION_REPORT.md` แสดงโครงสร้างโปรเจกต์
- หรือแสดง Solution Explorer ให้เห็น Architecture

---

### 🎬 ส่วนที่ 2: Architecture & Approach (1.5 นาที)

**สิ่งที่ต้องพูด:**
```
"สำหรับ Architecture ผมเลือกใช้ Clean Architecture 
แบ่งเป็น 4 layers:

1. Domain Layer - Entities และ Business Rules
2. Application Layer - Use Cases, DTOs, Services
3. Infrastructure Layer - Database, EF Core
4. API Layer - Controllers, Swagger

เหตุผลที่เลือกใช้ Clean Architecture:
✅ แยกความรับผิดชอบชัดเจน
✅ ทดสอบง่าย - ทำ Unit Test ได้
✅ เปลี่ยน Database ได้ง่าย
✅ ตรงกับ SOLID Principles"
```

**สิ่งที่ต้องแสดงบนหน้าจอ:**
```
src/
├── FlowAccount.Domain/          ← Core Business Logic
├── FlowAccount.Application/     ← Use Cases & Services
├── FlowAccount.Infrastructure/  ← Database & EF Core
└── FlowAccount.API/             ← REST API & Swagger
```

---

### 🎬 ส่วนที่ 3: Key Features Demo (2 นาที)

**Feature 1: Batch Operations (45 วินาที)**

**พูดพร้อมทำ:**
```
"ฟีเจอร์แรก คือ Batch Operations สำหรับสร้าง Variants

สมมติสินค้ามี 3 สี (Red, Blue, Green) และ 2 ขนาด (S, M)
ระบบจะสร้าง 6 Variants โดยอัตโนมัติ ด้วย Cartesian Product

ผมจะ Demo จริงนะครับ..."
```

**แสดงบนหน้าจอ:**
1. เปิด Swagger UI (http://localhost:5159)
2. ไปที่ `POST /api/products` → สร้างสินค้า
3. ไปที่ `POST /api/products/{id}/generate-variants`
4. แสดง Response ที่สร้าง 6 variants

---

**Feature 2: Stock Logic (45 วินาที)**

**พูดพร้อมทำ:**
```
"ฟีเจอร์ที่สอง คือ Stock Logic สำหรับ Bundle

สมมติมี Bundle ที่มี:
- T-Shirt (Red, M) จำนวน 2 ตัว
- Hat (Blue) จำนวน 1 ใบ

ระบบจะคำนวณว่าขายได้กี่ชุด ตามสต็อคที่มี
เช่น T-Shirt มี 10 ตัว, Hat มี 5 ใบ → ขายได้ 5 ชุด"
```

**แสดงบนหน้าจอ:**
1. ไปที่ `POST /api/bundles/calculate-stock`
2. แสดง Request/Response
3. อธิบาย Logic การคำนวณ

---

**Feature 3: Transaction Management (30 วินาที)**

**พูดพร้อมทำ:**
```
"ฟีเจอร์สุดท้าย คือ Transaction Management

เมื่อขาย Bundle ระบบจะ:
1. ตรวจสอบสต็อคทั้งหมดก่อน
2. ลดสต็อคทุกตัว ใน Transaction เดียว
3. ถ้ามีปัญหา → Rollback ทั้งหมด

ทำให้มั่นใจว่าข้อมูลถูกต้องเสมอ"
```

**แสดงบนหน้าจอ:**
- แสดงโค้ด `BundleService.cs` → method `SellBundleAsync()`
- ชี้ส่วน `using var transaction = ...`

---

### 🎬 ส่วนที่ 4: Important Decisions (1.5 นาที)

**สิ่งที่ต้องพูด:**
```
"การตัดสินใจสำคัญๆ ที่ผมทำ:

🤖 0️⃣ ใช้ AI (GitHub Copilot) 100% ในการพัฒนา
   → ตามโจทย์ที่กำหนดให้ใช้ AI เต็มรูปแบบ
   → ใช้ GitHub Copilot Chat สำหรับ:
      • สร้าง Architecture และ Database Design
      • เขียนโค้ดทั้งหมด (Controllers, Services, Repositories)
      • Generate Unit Tests
      • เขียน Documentation ทั้ง 22 ไฟล์
   → ผลลัพธ์: ประหยัดเวลา และได้ Code Quality สูง

1️⃣ เลือก Clean Architecture 
   → เพราะต้องการ Testability และ Maintainability
   → AI แนะนำและช่วยสร้างโครงสร้างที่ถูกต้อง

2️⃣ ใช้ Repository + Unit of Work Pattern
   → แยก Data Access Logic ออกจาก Business Logic
   → ง่ายต่อการเปลี่ยน Database
   → AI ช่วย Generate Boilerplate Code

3️⃣ ใช้ Strategy Pattern สำหรับ Stock Calculation
   → อนาคตสามารถเพิ่ม Strategy ใหม่ได้
   → เช่น Reserved Stock, Pre-order Stock
   → AI แนะนำ Pattern ที่เหมาะสม

4️⃣ Comprehensive Validation & Error Handling
   → ใช้ FluentValidation สำหรับ Input Validation
   → Custom Exception สำหรับ Business Rules
   → Global Exception Handler สำหรับ Consistent Error Response
   → AI ช่วย Generate Validation Rules

5️⃣ เลือก SQL Server + EF Core
   → ใช้ Transaction Support
   → ACID Properties สำหรับความถูกต้อง
   → AI ช่วย Generate EF Core Configurations

6️⃣ Extensive Testing
   → Unit Tests: 16/16 passed (100%)
   → Integration Tests ครบทุก Use Case
   → Max Capacity Test: 250 Variants
   → AI ช่วยเขียน Test Cases ทั้งหมด
"
```

**แสดงบนหน้าจอ:**
- เปิด `PROJECT_COMPLETION_REPORT.md` ส่วน Important Decisions
- แสดง `AI_COPILOT_WORKFLOW.md` (ถ้ามี)
- แสดง Test Results
- **สำคัญ:** แสดง GitHub Copilot Icon ในหน้าจอ VS Code

---

### 🎬 ส่วนที่ 4.5: AI Development Showcase (30 วินาที - เพิ่มเติม)

**สิ่งที่ต้องพูดและแสดง:**
```
"ผมจะแสดงตัวอย่างว่า AI ช่วยพัฒนาอย่างไร:

[แสดงหน้าจอ VS Code]
- นี่คือ GitHub Copilot ที่ช่วยเขียนโค้ด
- ช่วย Generate Service Methods
- ช่วย Generate Unit Tests
- ช่วยเขียน Documentation

ตัวอย่าง AI Workflow:
1. ผมถาม Copilot: 'Create a ProductService with CRUD'
2. AI สร้าง Service Class พร้อม Methods
3. ผมถาม: 'Generate unit tests'
4. AI สร้าง Test Cases ทั้งหมด

ผลลัพธ์:
✅ โค้ด 100% จาก AI-Assisted Development
✅ ได้ Clean Code ตาม Best Practices
✅ Test Coverage สูง
✅ Documentation ครบถ้วน"
```

**แสดงบนหน้าจอ:**
- VS Code พร้อม GitHub Copilot Icon
- แสดงตัวอย่างโค้ดที่ AI Generate
- แสดง Chat History กับ Copilot (ถ้ามี)
- แสดง Documentation ที่ AI สร้าง

---

### 🎬 ส่วนที่ 5: Closing & Results (30 วินาที)

**สิ่งที่ต้องพูด:**
```
"สรุปผลงาน:
🤖 ใช้ AI (GitHub Copilot) 100% ตามโจทย์
✅ ครบทุก Requirements ที่โจทย์กำหนด
✅ Database Design สมบูรณ์
✅ 14 API Endpoints พร้อม Swagger
✅ Unit Tests 100% Pass (16/16 tests)
✅ Documentation ครบถ้วน 22 ไฟล์
✅ Production-ready code quality
✅ AI-Powered Development Workflow

พิสูจน์ให้เห็นว่า AI สามารถช่วยพัฒนา
Software ระดับ Production ได้จริง

ขอบคุณครับ/ค่ะ"
```

**แสดงบนหน้าจอ:**
- แสดง Swagger UI พร้อม Endpoints ทั้งหมด
- แสดง Test Results Summary
- **แสดง GitHub Copilot Logo/Icon**
- หรือ Test Results Summary

---

## 🎬 แนวทางการอัดวิดีโอ

### ✅ **Option 1: Screen Recording + Webcam (แนะนำ)**

**เครื่องมือฟรี:**
- **OBS Studio** (แนะนำที่สุด) - https://obsproject.com/
- **ShareX** - https://getsharex.com/
- **Windows Game Bar** (กด Win + G)

**Setup:**
1. เปิด Webcam มุมบนขวา (ขนาดเล็ก)
2. Screen Capture ส่วนใหญ่
3. Microphone เปิดรับเสียงพูด

**ข้อดี:**
- ดู Professional
- เห็นหน้าผู้พูด (สร้างความน่าเชื่อถือ)
- แสดงทั้งโค้ดและ Demo ได้

---

### ✅ **Option 2: Screen Recording Only**

**เครื่องมือ:**
- **OBS Studio**
- **Loom** (ฟรี 5 นาที/วิดีโอ) - https://www.loom.com/

**Setup:**
1. Record Full Screen หรือ Window Selection
2. พูดอธิบายไปด้วย
3. อาจเพิ่ม Text Overlay สำหรับ Key Points

**ข้อดี:**
- ทำง่าย
- Focus ที่เนื้อหา

---

### ✅ **Option 3: PowerPoint/Slides + Screen Recording**

**Setup:**
1. สร้าง Slides สรุป Key Points
2. สลับระหว่าง Slides กับ Live Demo
3. Record ทั้งหมดเป็นวิดีโอเดียว

**ข้อดี:**
- ดูเป็นระเบียบ
- ง่ายต่อการติดตาม

---

## 📋 Checklist ก่อนอัด

### เตรียม Environment

- [ ] API รันสำเร็จ (`dotnet run`)
- [ ] Swagger UI เปิดได้ (http://localhost:5159)
- [ ] Database มีข้อมูล Demo (`.\demo-reset.ps1`)
- [ ] ปิด Notification/แจ้งเตือนทั้งหมด
- [ ] ซ่อน Personal Info (ชื่อ, Email, etc.)

### เตรียมหน้าจอ

- [ ] ขนาดตัวอักษร Terminal/Code Editor ใหญ่พอ
- [ ] Theme ชัดเจน (แนะนำ Light Theme สำหรับวิดีโอ)
- [ ] เปิดเฉพาะ Windows ที่จำเป็น
- [ ] ปิด Browser Tabs ที่ไม่เกี่ยวข้อง

### เตรียมสคริปต์

- [ ] เขียนสคริปต์ทั้งหมด
- [ ] ซ้อมพูด 1-2 รอบ
- [ ] จับเวลาแต่ละส่วน (ไม่ควรเกิน 7 นาที)
- [ ] เตรียม Request Body JSON สำหรับ Demo

---

## 🤖 AI Development Workflow (สำคัญมาก!)

### เนื่องจากโจทย์กำหนดให้ใช้ AI 100%

ต้องแสดงให้เห็นว่าใช้ AI อย่างไรในวิดีโอ:

### ตัวอย่างที่ควรพูดและแสดง:

#### 1. **Database Design with AI**
```
"ผมเริ่มด้วยการถาม GitHub Copilot:
'Design database schema for Product Master, Variants, and Bundle system'

AI ช่วย:
✅ แนะนำ Entity Relationships
✅ สร้าง EF Core Configurations
✅ Generate Migration Files
"
```

#### 2. **Code Generation with AI**
```
"ตัวอย่างการใช้ AI เขียนโค้ด:

Prompt: 'Create ProductService with CRUD operations and generate variants using Cartesian product'

AI สร้างให้:
✅ Service Interface
✅ Service Implementation
✅ Validation Logic
✅ Error Handling
"
```

#### 3. **Test Generation with AI**
```
"ผมถาม Copilot:
'Generate unit tests for ProductService with Moq framework'

AI สร้าง:
✅ Mock Setup
✅ Test Cases ทั้งหมด
✅ Assertions
✅ 16/16 Tests ผ่านหมด
"
```

#### 4. **Documentation with AI**
```
"Documentation ทั้ง 22 ไฟล์:
✅ AI ช่วยเขียน README
✅ AI ช่วยสร้าง API Documentation
✅ AI ช่วยเขียน Testing Guides
✅ AI ช่วยสรุป Project Completion Report
"
```

### 💡 วิธีแสดงใน Video:

**Option 1: แสดง AI Copilot Chat History**
- เปิด GitHub Copilot Chat Panel
- แสดง Conversation ที่เคยถาม
- แสดงว่า AI ตอบอะไรบ้าง

**Option 2: แสดง Live AI Suggestion**
- เปิดไฟล์โค้ด
- แสดง Copilot Inline Suggestions (สีเทา)
- อธิบายว่า AI แนะนำโค้ดตรงนี้

**Option 3: แสดง AI Generated Comments**
- แสดงโค้ดที่มี Comments ดีๆ
- บอกว่า AI ช่วยเขียน Comments

**Option 4: แสดง File Structure**
- แสดง 22 Documentation Files
- บอกว่าทุกไฟล์สร้างด้วย AI

---

## 🎯 Tips สำหรับวิดีโอที่ดี

### ✅ DO:

1. **พูดชัดเจน ช้าพอสมควร**
   - แต่ละคำออกเสียงให้ชัด
   - ไม่เร็วเกินไป

2. **แสดงความมั่นใจ**
   - พูดเหมือนอธิบายให้เพื่อนฟัง
   - ไม่ต้องเป็นทางการมากเกินไป

3. **ใช้ภาษาง่ายๆ**
   - อธิบายเทคนิคด้วยคำที่เข้าใจง่าย
   - หลีกเลี่ยง Jargon มากเกินไป

4. **แสดงผลลัพธ์จริง**
   - Demo ใน Swagger
   - แสดง Response จริง
   - รันโค้ดจริง

5. **เน้น Key Points**
   - ใช้เมาส์ชี้ส่วนสำคัญ
   - Highlight โค้ด
   - Zoom In ถ้าจำเป็น

---

### ❌ DON'T:

1. **อย่าพูดเร็วเกินไป**
2. **อย่าใช้เวลานานเกิน 10 นาที**
3. **อย่าแสดงข้อมูลส่วนตัว** (API Keys, Passwords, etc.)
4. **อย่า Edit มากเกินไป** - จอใจ Natural ดีกว่า
5. **อย่ากลัวผิดพลาด** - ถ้าผิดไปก็พูดแก้ไขต่อได้

---

## 📦 Files ที่ควรแสดงในวิดีโอ

### ไฟล์สำคัญ (เตรียมเปิดไว้):

```
✅ ต้องแสดง:
1. c:\Users\Chalermphan\source\flowaccout\docs\PROJECT_COMPLETION_REPORT.md
2. Swagger UI (http://localhost:5159)
3. Solution Explorer (แสดง Architecture)

✅ อาจแสดง:
4. src\FlowAccount.Application\Services\BundleService.cs (Transaction)
5. src\FlowAccount.Application\Services\ProductService.cs (Batch Gen)
6. tests\FlowAccount.Tests\ (Test Results)
7. docs\TESTING_RESULTS_REPORT.md
```

---

## 🚀 Step-by-Step Recording Process

### Phase 1: Setup (5 นาที)

```powershell
# 1. รีเซ็ต Database
cd c:\Users\Chalermphan\source\flowaccout
.\demo-reset.ps1

# 2. รัน API
cd src\FlowAccount.API
dotnet run

# 3. เปิด Browser → http://localhost:5159
```

### Phase 2: Test Run (5 นาที)

1. เปิดไฟล์ทั้งหมดที่จะแสดง
2. ทดลองพูดตามสคริปต์
3. ทดลอง Navigate ระหว่างหน้าจอ
4. เช็คเวลารวม

### Phase 3: Record (15-30 นาที)

1. กด Record
2. พูดตามสคริปต์ที่เตรียมไว้
3. แสดง Demo แต่ละส่วน
4. ปิด Record

### Phase 4: Review (5 นาที)

- [ ] ดูวิดีโอทั้งหมด
- [ ] เช็คเสียงชัดไหม
- [ ] เช็คหน้าจอชัดไหม
- [ ] อัดใหม่ถ้าจำเป็น

---

## 🎬 OBS Studio Setup (แนะนำ)

### การติดตั้ง OBS Studio:

```powershell
# ดาวน์โหลดจาก https://obsproject.com/download
# หรือใช้ winget (ถ้ามี)
winget install OBSProject.OBSStudio
```

### การตั้งค่า OBS:

1. **Scene Setup:**
   - Scene 1: "Full Screen" → Display Capture
   - Scene 2: "Code + Webcam" → Window Capture + Webcam

2. **Audio:**
   - Desktop Audio: เปิด (รับเสียงจาก System)
   - Mic/Auxiliary Audio: เปิด (รับเสียงพูด)

3. **Video Settings:**
   - Resolution: 1920x1080 (Full HD)
   - FPS: 30
   - Encoder: x264

4. **Output Settings:**
   - Format: MP4
   - Quality: High Quality, Medium File Size

---

## 📤 Export & Submit

### ขนาดไฟล์ที่แนะนำ:
- **5-7 นาที** → ประมาณ 100-200 MB (MP4, 1080p)

### Platform สำหรับ Upload:
- **YouTube** (Unlisted Link)
- **Google Drive** (แชร์เป็น Link)
- **OneDrive**
- **Vimeo**

### ชื่อไฟล์แนะนำ:
```
FlowAccount_API_Project_Demo_[YourName].mp4
```

---

## 📝 Presentation Script Template

ใช้เทมเพลตนี้ร่างสคริปต์:

```markdown
## PART 1: INTRO (0:00-1:00)
"สวัสดีครับ/ค่ะ..."
[พูดอะไร]
[แสดงอะไรบนหน้าจอ]

## PART 2: ARCHITECTURE (1:00-2:30)
"สำหรับ Architecture..."
[พูดอะไร]
[แสดงอะไรบนหน้าจอ]

... etc
```

---

## ✅ Final Checklist

### ก่อนส่งวิดีโอ:

- [ ] **เวลารวม:** 5-7 นาที (ไม่เกิน 10 นาที)
- [ ] **ครบ 3 หัวข้อ:**
  - [ ] Project Overview
  - [ ] Your Approach
  - [ ] Important Decisions
- [ ] **แสดง Demo จริง** ใน Swagger
- [ ] **อธิบาย Architecture** ชัดเจน
- [ ] **เสียงชัด** ได้ยินทุกคำ
- [ ] **หน้าจอชัด** อ่านโค้ดได้
- [ ] **ไม่มีข้อมูลส่วนตัว**
- [ ] **ไฟล์ใช้งานได้** (ลองเปิดดู)

---

## 🎓 Summary

### สิ่งที่ต้องทำ:
1. ✅ เตรียม Environment
2. ✅ เขียนสคริปต์
3. ✅ ซ้อมพูด
4. ✅ อัดวิดีโอ
5. ✅ Review
6. ✅ Upload & ส่ง Link

### เวลาที่ใช้รวม:
- เตรียมการ: 30 นาที
- อัดวิดีโอ: 15-30 นาที (รวมอัดซ้ำ)
- **รวม: ประมาณ 1 ชั่วโมง**

---

## 📞 Need Help?

ถ้าติดปัญหาระหว่างอัดวิดีโอ:

1. **API ไม่ทำงาน:**
   ```powershell
   .\demo-reset.ps1
   cd src\FlowAccount.API
   dotnet run
   ```

2. **Database ผิดพลาด:**
   ```powershell
   .\clear-data.ps1
   .\demo-reset.ps1
   ```

3. **Swagger ไม่แสดง:**
   - เช็ค URL: http://localhost:5159
   - รีสตาร์ท API

---

## 🎬 Good Luck!

**Remember:** 
- Be confident! 
- Show your work with pride!
- Explain your reasoning!
- Demo real features!

**คุณทำโปรเจกต์นี้ได้ดีมาก - เชื่อมั่นในผลงานของคุณเอง!** 🚀
```
