## FlowAccount Presentation - Teleprompter Script

Notes:
- Short lines for easy reading.
- [PAUSE 2s] means pause two seconds.
- [SHOW:], [OPEN:], [EXECUTE:], [CLICK:] are on-screen actions.
- Timestamps are suggested section durations.

---

[TIME 00:00 - 00:30] INTRO (30s)

สวัสดีครับ
[PAUSE 1s]

ผมชื่อ เฉลิมพันธ์ ครับ
[PAUSE 1s]

วันนี้ผมจะนำเสนอโปรเจคที่ผมพัฒนาสำหรับตำแหน่ง Software Engineer ที่ FlowAccount
[PAUSE 1s]

โปรเจคนี้พัฒนาด้วย AI 100% โดยใช้ GitHub Copilot Chat และ VS Code
[PAUSE 1s]

เทคโนโลยีหลัก: .NET 10.0 และ Clean Architecture (4 layers)
[PAUSE 1s]

[SHOW: VS Code Explorer]
[SHOW: ชี้โฟลเดอร์ Domain / Application / Infrastructure / API]

---

[TIME 00:30 - 02:30] ARCHITECTURE & DESIGN (2:00)

ผมเลือกใช้ Clean Architecture (Onion Architecture)
[PAUSE 0.5s]

มี 4 ชั้นหลัก:
- Domain: Entities และ Core Logic
- Application: Services, DTOs, Validators
- Infrastructure: Repositories, DbContext
- API: Controllers และ Swagger
[PAUSE 0.5s]

[SHOW: src/FlowAccount.Domain]
[SHOW: src/FlowAccount.Application]
[SHOW: src/FlowAccount.Infrastructure]
[SHOW: src/FlowAccount.API]

Design Patterns ที่ใช้ (สรุป):
- Repository
- Unit of Work
- Dependency Injection
- Strategy (variant generation)
- Specification
[PAUSE 1s]

เหตุผล: Separation of Concerns → Testable, Maintainable
[PAUSE 1s]

Important Decision #1: เลือก Clean Architecture เพื่อแยกความรับผิดชอบชัดเจน
[PAUSE 1s]

---

[TIME 02:30 - 04:00] FEATURE 1 — BATCH VARIANT GENERATION (1:30)

Feature: สร้างหลายๆ Product Variant ในคำขอเดียว
[PAUSE 0.5s]

[OPEN: Swagger UI -> POST /api/products/variants/generate]
[SWITCH TO BROWSER]

ตัวอย่าง Payload ที่จะใช้:
{ "productMasterId": 1, "selectedOptions": { "1": [1,2,3,4,5], "2": [6,7,8,9,10] }, "priceStrategy":0, "basePrice":299.00, "skuPattern":"TSHIRT-{Size}-{Color}" }
[PAUSE 0.5s]

[CLICK: Try it out → Paste payload → Execute]

ผลลัพธ์ที่คาดหวัง:
- totalCount: 25
- executionTimeMs: ~1500
- variants: array with SKUs เช่น PM001-XS-Red
[PAUSE 1s]

Important Decision #2: ใช้ Bulk Insert (AddRange) เพื่อลด DB round-trips
[PAUSE 0.5s]

หมายเหตุ: เช็ก IDs จาก GET /api/products ก่อนถ้าจำเป็น
[PAUSE 0.5s]

---

[TIME 04:00 - 05:30] FEATURE 2 — BUNDLE STOCK CALCULATION (1:30)

Feature: คำนวณสต็อกของ Bundle โดยคำนึงถึง component ทุกชิ้น
[PAUSE 0.5s]

[OPEN: docs/STOCK_LOGIC_EXPLAINED.md]
[SHOW: สูตร Bottleneck Approach]

สูตรสั้นๆ:
Bundle Stock = MIN( component.Stock / component.RequiredQuantity )
[PAUSE 0.5s]

[SWITCH TO BROWSER -> Swagger -> GET /api/bundles/{id}/stock]
[CLICK: Try it out → id=1 → Execute]

ดู response:
- availableStock: 30
- bottleneckItem: shows which variant limited the bundle
[PAUSE 1s]

Important Decision #3: คำนวณแบบ Real-time เพื่อความถูกต้องในการขายพร้อมกัน
[PAUSE 0.5s]

---

[TIME 05:30 - 07:00] FEATURE 3 — TRANSACTION MANAGEMENT (1:30)

Feature: Deduct stock แบบ atomic สำหรับการขาย bundle
[PAUSE 0.5s]

[OPEN: docs/TRANSACTION_MANAGEMENT_DETAILS.md]
[SHOW: BeginTransaction, Commit, Rollback example]

โค้ดตัวอย่างสั้น:
using var transaction = await _unitOfWork.BeginTransactionAsync();
try { await _unitOfWork.SaveChangesAsync(); await transaction.CommitAsync(); }
catch { await transaction.RollbackAsync(); }
[PAUSE 0.5s]

ใช้ Optimistic Locking ด้วย [Timestamp] RowVersion
- ตรวจจับ concurrent updates → คืน 409 Conflict ให้ client retry

Important Decision #4: เลือก Optimistic Locking เพราะระบบอ่านเยอะกว่าเขียน
[PAUSE 0.5s]

---

[TIME 07:00 - 08:00] TESTING (1:00)

รัน Unit Tests: dotnet test
[PAUSE 0.5s]

ผลลัพธ์ที่ต้องแสดง:
Passed: 16  Failed: 0  Skipped: 1  Total: 17
[PAUSE 0.5s]

[SHOW: tests/FlowAccount.Tests/BundleServiceTests.cs]
[OPEN: ไฟล์ตัวอย่าง test]

อธิบายสั้นๆ: xUnit + Moq + FluentAssertions
[PAUSE 0.5s]

---

[TIME 08:00 - 08:30] DOCUMENTATION (0:30)

[OPEN: docs/DOCUMENTATION_INDEX.md]

Documentation: 22 ไฟล์
- DESIGN_DECISIONS.md
- DATABASE_DESIGN_DETAILED.md
- BATCH_OPERATIONS_DETAILS.md
[PAUSE 0.5s]

เน้นว่าเอกสารมีตัวอย่างโค้ดและผลการทดสอบครบ
[PAUSE 0.5s]

---

[TIME 08:30 - 10:30] API DEMO & COMPLETE TEST (2:00)

เปิด Terminal 1: รัน API
dotnet run --project src/FlowAccount.API
[PAUSE 1s]

รอข้อความ: Now listening on: https://localhost:7XXX
[PAUSE 1s]

[SWITCH TO BROWSER -> Swagger UI]

ทดลอง API:
1) GET /api/products
2) POST /api/products/variants/generate (demo ต้นฉบับ)
3) GET /api/bundles/{id}/stock
[PAUSE 1s]

รัน Complete Test Script (Terminal 2):
.\complete-test.ps1

แสดงผลลัพธ์ของสคริปต์ทีละขั้นตอน (Create ProductMaster, Generate Variants, Create Bundle, Calculate Stock)
[PAUSE 1s]

---

[TIME 10:30 - 11:30] AI-First WORKFLOW (1:00)

สรุปการใช้ AI 100% ในโปรเจคนี้
- วิเคราะห์โจทย์
- สร้างโครงสร้างโปรเจค
- Implement Features
- Review & Refactor
- Generate Tests และ Docs
[PAUSE 0.5s]

อธิบายบทบาท Developer: เป็น Reviewer และ Decision Maker
[PAUSE 0.5s]

---

[TIME 11:30 - 12:00] SUMMARY & CLOSE (0:30)

สรุปสั้นๆ:
- Clean Architecture + 10 Design Patterns
- Performance: 250 variants ใน ~2s
- Testing: 17 Unit Tests
- Documentation: 22 files
[PAUSE 1s]

ระบบพร้อมใช้งานจริง และแสดงว่าการใช้ AI เพิ่มประสิทธิภาพการพัฒนา
[PAUSE 1s]

ขอบคุณครับ
[PAUSE 1s]

---

## Pre-record Checklist (สั้น)

- ปิดโปรแกรมที่ไม่จำเป็น
- เปิด VS Code, Terminal 2 windows, Browser (Swagger)
- ทดสอบไมค์
- รัน API และตรวจ Swagger
- เตรียมไฟล์ markdown ที่จะโชว์

Commands (Powershell):
```powershell
cd c:\Users\Chalermphan\source\flowaccout
dotnet run --project src/FlowAccount.API    # Terminal 1
.\complete-test.ps1                          # Terminal 2
```

Tips สำหรับอ่าน:
- อ่านช้าๆ ชัดเจน (ประมาณ 140-160 คำ/นาที)
- ใช้ [PAUSE Xs] ตาม cue
- ถ้ต้องการหยุด ให้หยุด 2-3 วินาที แล้วเริ่มใหม่
- ใช้ Fullscreen editor หรือ Teleprompter app เพื่อไม่ต้องเลื่อน

---

## How to use this file

1. เปิด `VIDEO_SCRIPT_TPROMPTER.md` ใน VS Code
2. ขยายหน้าต่างตัวหนังสือให้ใหญ่ (Font 22-28)
3. ใช้ extension Teleprompter หรือ Fullscreen Editor
4. อ่านตามบรรทัด เลื่อนทีละบรรทัดเมื่อพร้อม

Good luck!
