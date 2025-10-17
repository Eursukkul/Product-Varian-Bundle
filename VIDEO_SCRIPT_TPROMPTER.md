# สคริปต์อ่านขณะอัดวิดีโอ - FlowAccount

---

## 🎬 ส่วนที่ 1: เปิดตัว (30 วินาที)

สวัสดีครับ

ผมชื่อ เฉลิมพันธ์

วันนี้ผมจะนำเสนอโปรเจค

ที่ผมพัฒนาสำหรับตำแหน่ง Software Engineer

ที่บริษัท FlowAccount ครับ

[หยุด 1 วินาที]

จุดเด่นของโปรเจคนี้คือ

ผมพัฒนาด้วย AI 100%

โดยใช้ GitHub Copilot

[หยุด 1 วินาที]

เทคโนโลยีที่ใช้คือ .NET 10

กับ Clean Architecture

แบ่งเป็น 4 ชั้น

[หยุด 1 วินาที]

👉 **[ชี้เมาส์ที่ VS Code Explorer]**

👉 **[ชี้โฟลเดอร์ทั้ง 4: Domain, Application, Infrastructure, API]**

---

## 🏗️ ส่วนที่ 2: สถาปัตยกรรม (2 นาที)

ผมเลือกใช้ Clean Architecture

หรือที่เรียกว่า Onion Architecture ครับ

[หยุด 1 วินาที]

แบ่งเป็น 4 ชั้น

[หยุด]

👉 **[ชี้โฟลเดอร์ Domain]**

ชั้นแรก Domain

เก็บ Entities หลักของระบบ

เช่น ProductMaster, ProductVariant, Bundle

[หยุด]

👉 **[ชี้โฟลเดอร์ Application]**

ชั้นที่สอง Application

เขียน Business Logic

และ Services ต่างๆ

[หยุด]

👉 **[ชี้โฟลเดอร์ Infrastructure]**

ชั้นที่สาม Infrastructure

จัดการ Database

ใช้ Repository Pattern

[หยุด]

👉 **[ชี้โฟลเดอร์ API]**

ชั้นสุดท้าย API

เป็น RESTful API endpoints

มี Swagger documentation

[หยุด 2 วินาที]

Design Patterns ที่ใช้มี

Repository Pattern

Unit of Work Pattern

Dependency Injection

Strategy Pattern

และ Specification Pattern

[หยุด 1 วินาที]

**Decision สำคัญข้อแรก:**

เลือก Clean Architecture

เพราะแยกความรับผิดชอบชัดเจน

ทำให้ง่ายต่อการ Test

และ Maintain ระบบ

[หยุด 2 วินาที]

---

## ⚡ ส่วนที่ 3: Feature แรก - สร้าง Variants แบบเยอะๆ (3.5 นาที)

Feature แรก

คือการสร้าง Product Variant

หลายร้อยตัวพร้อมกันในคำขอเดียว

[หยุด 1 วินาที]

ผมจะ Demo การสร้าง 250 variants

จากการผสม

10 ไซส์ คูณ 25 สี

เท่ากับ 250 ตัว

เพื่อแสดง Performance ที่ดีของ Bulk Insert

[หยุด 1 วินาที]

---

### � ขั้นตอนที่ 1: สร้าง Product Master พร้อม Options (1 นาที)

�👉 **[เปิด Browser → Swagger UI]**

👉 **[หา POST /api/products]**

👉 **[คลิก Try it out]**

ผมจะสร้าง Product ที่มี

10 ไซส์

25 สี

[หยุด 1 วินาที]

👉 **[Copy payload นี้ไป Paste:]**

```json
{
  "name": "T-Shirt Demo 250",
  "description": "Demo Product with 250 variants",
  "basePrice": 299.00,
  "baseCost": 150.00,
  "options": [
    {
      "name": "Size",
      "values": ["XXS", "XS", "S", "M", "L", "XL", "XXL", "3XL", "4XL", "5XL"]
    },
    {
      "name": "Color",
      "values": ["Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Pink", "Black", "White", "Gray", "Brown", "Navy", "Beige", "Cream", "Olive", "Maroon", "Teal", "Coral", "Mint", "Lavender", "Gold", "Silver", "Copper", "Bronze", "Charcoal"]
    }
  ]
}
```

👉 **[Paste และ Execute]**

[หยุด 3 วินาที รอผลลัพธ์]

เห็นไหมครับ Status 200 OK

Product สร้างสำเร็จ

[หยุด 1 วินาที]

👉 **[Scroll ดู Response]**

👉 **[ชี้ที่ "id": 1]**

นี่คือ productMasterId ที่เราจะใช้

จดไว้ว่าเป็น 1

[หยุด 1 วินาที]

👉 **[Scroll หา variantOptions array]**

👉 **[ชี้ option แรก: Size]**

Size Option มี id เป็นเลข 1

มี 10 values

แต่ละ value มี id ตั้งแต่ 1-10

[หยุด 1 วินาที]

👉 **[ชี้ option ที่สอง: Color]**

Color Option มี id เป็นเลข 2

มี 25 values

แต่ละ value มี id ตั้งแต่ 11-35

[หยุด 1 วินาที]

ตอนนี้เรามีข้อมูลครบแล้ว

productMasterId = 1

Size option = 1, values = [1-10]

Color option = 2, values = [11-35]

[หยุด 1 วินาที]

---

### 🔹 ขั้นตอนที่ 2: Generate 250 Variants ด้วย Bulk Insert (1.5 นาที)

👉 **[หา POST /api/products/1/generate-variants]**

👉 **[คลิก Try it out]**

ตอนนี้ผมจะสร้าง 250 variants พร้อมกัน

[หยุด 1 วินาที]

👉 **[Copy payload นี้ไป Paste:]**

```json
{
  "productMasterId": 1,
  "selectedOptions": {
    "1": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
    "2": [11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35]
  },
  "priceStrategy": 0,
  "basePrice": 299.00,
  "baseCost": 150.00,
  "skuPattern": "DEMO-{Size}-{Color}"
}
```

**อธิบาย payload:**

productMasterId: 1 ที่เพิ่งสร้าง

selectedOptions Key "1" = Size มี 10 values

selectedOptions Key "2" = Color มี 25 values

10 × 25 = 250 variants

priceStrategy: 0 = ราคาเท่ากันทุก variant

[หยุด 1 วินาที]

👉 **[Paste และ Execute]**

[หยุด 3 วินาที รอผลลัพธ์]

เห็นไหมครับ

Status 200 OK

ระบบสร้าง 250 variants สำเร็จ

ใช้เวลาแค่ 2 วินาที

นี่แสดงให้เห็นถึง Performance ที่ดีมาก

[หยุด 1 วินาที]

👉 **[Scroll ดู Response]**

👉 **[ชี้ที่ "totalVariantsGenerated": 250]**

นี่คือจำนวน variants ที่สร้าง

250 ตัว ในคำขอเดียว

[หยุด]

👉 **[ชี้ที่ "processingTime": "00:00:02.044"]**

ใช้เวลาแค่ 2 วินาทีเศษ

[หยุด]

👉 **[Scroll ดู variants array]**

👉 **[ชี้ที่ SKU]**

SKU สร้างอัตโนมัติ

เช่น DEMO-XXS-Red

DEMO-M-Blue

DEMO-XL-Green

[หยุด]

👉 **[ชี้ที่ price และ cost]**

ทุก variant มีราคา 299 บาท

เพราะใช้ Fixed strategy

[หยุด 2 วินาที]

**Decision สำคัญข้อสอง:**

ผมใช้ Bulk Insert

แทนที่จะ Insert ทีละตัว

[หยุด 1 วินาที]

ถ้า Insert ทีละตัว 250 ครั้ง

ต้องเชื่อมต่อ Database 250 ครั้ง

จะใช้เวลานานมาก

[หยุด 1 วินาที]

แต่ Bulk Insert

เชื่อมต่อแค่ครั้งเดียว

รวมข้อมูลทั้ง 250 รายการ

Insert ในคำสั่งเดียว

ทำให้เร็วมาก

[หยุด 2 วินาที]

นี่คือประสิทธิภาพของ Bulk Insert

250 variants ใช้เวลาแค่ 2 วินาที

[หยุด 2 วินาที]

---

### 🔹 ขั้นตอนที่ 3: รัน Complete Test เพื่อทดสอบระบบครบทุก Feature (1 นาที)

ตอนนี้ผมจะรัน Complete Test Script

เพื่อทดสอบระบบทั้งหมด

[หยุด 1 วินาที]

👉 **[เปิด Terminal 2]**

👉 **[พิมพ์คำสั่ง:]**

```powershell
.\complete-test.ps1
```

👉 **[กด Enter]**

[หยุด 3 วินาที รอ Script เริ่มรัน]

Script นี้จะทดสอบทุก Feature

สร้าง Product Master

Generate 500 Variants

สร้าง Bundle

คำนวณ Stock

Sell Bundle

และ Verify Transaction

[หยุด 2 วินาที รอผลลัพธ์]

👉 **[Scroll ดูผลลัพธ์]**

เห็นไหมครับ

ผ่านทุก Step

[หยุด 1 วินาที]

👉 **[ชี้ที่ STEP 3: Generate Variants]**

Generate 500 variants สำเร็จ

ใช้ Batch Operation

[หยุด]

👉 **[ชี้ที่ Processing Time]**

เห็นเวลาที่ใช้ไหมครับ

รวดเร็วมาก

[หยุด 1 วินาที]

👉 **[Scroll ดู STEP 7: Bundle Stock]**

คำนวณ Bundle Stock สำเร็จ

แสดง Bottleneck Item

[หยุด]

👉 **[Scroll ดู STEP 8: Sell Bundle]**

ทดสอบ Transaction Management

มี BEGIN TRANSACTION

หัก Stock พร้อมกัน

แล้ว COMMIT สำเร็จ

[หยุด 2 วินาที]

Script นี้พิสูจน์ว่า

ระบบทำงานครบทุก Feature

Batch Operations

Transaction Management

Stock Logic

ทั้งหมดทำงานถูกต้อง

[หยุด 2 วินาที]

---

## 📦 ส่วนที่ 4: Stock Management API (1 นาที)

ก่อนจะดู Bundle Stock

ผมต้องอธิบาย Stock Management ก่อน

[หยุด 1 วินาที]

👉 **[เปิด Browser → Swagger]**

👉 **[ชี้ที่ Stock tag/section]**

ผมสร้าง StockController

สำหรับจัดการสต็อกสินค้า

[หยุด 1 วินาที]

มี 2 API หลัก

[หยุด]

👉 **[ชี้ที่ POST /api/stock/adjust]**

อันแรก POST /api/stock/adjust

เพิ่มหรือลดสต็อก

[หยุด]

👉 **[ชี้ที่ GET /api/stock]**

อันที่สอง GET /api/stock

เช็คสต็อกปัจจุบัน

[หยุด 1 วินาที]

👉 **[คลิก POST /api/stock/adjust → Try it out]**

ลอง Demo การเพิ่มสต็อกนะครับ

[หยุด 1 วินาที]

👉 **[Copy payload นี้:]**

```json
{
  "warehouseId": 1,
  "itemType": "Variant",
  "itemId": 1,
  "quantity": 50,
  "adjustmentType": "StockIn",
  "reason": "เติมสต็อกสินค้า"
}
```

👉 **[Execute]**

[หยุด 2 วินาที]

เห็นไหมครับ

สต็อกเพิ่มขึ้นเป็น 50

[หยุด 1 วินาที]

AdjustmentType มี 3 แบบ

StockIn เพิ่มสต็อก

StockOut ลดสต็อก

Set กำหนดค่าตรงๆ

[หยุด 2 วินาที]

---

## 📦 ส่วนที่ 5: Feature ที่สอง - คำนวณสต็อก Bundle (1.5 นาที)

Feature ที่สอง

คือคำนวณสต็อก Bundle

[หยุด 1 วินาที]

Bundle คืออะไร?

คือชุดสินค้าที่รวมหลายตัวเข้าด้วยกัน

[หยุด 1 วินาที]

👉 **[เปิดไฟล์ docs/STOCK_LOGIC_EXPLAINED.md]**

สูตรที่ใช้คือ

Bundle Stock เท่ากับ

ค่าต่ำสุดของ

สต็อกแต่ละชิ้น หารด้วย จำนวนที่ต้องใช้

[หยุด 1 วินาที]

ยกตัวอย่างนะครับ

สมมติ Bundle มี

Product A ต้องใช้ 2 ชิ้น มีสต็อก 100

Product B ต้องใช้ 1 ชิ้น มีสต็อก 30

[หยุด 1 วินาที]

Product A ทำได้ 50 Bundle

Product B ทำได้ 30 Bundle

ดังนั้น Bundle นี้มีสต็อก 30 ชุด

เพราะ Product B เป็นคอขวด

[หยุด 2 วินาที]

👉 **[เปิด Browser → Swagger]**

👉 **[หา GET /api/bundles/{id}/stock]**

👉 **[คลิก Try it out]**

👉 **[ใส่ id = 1 ในช่อง bundleId]**

👉 **[คลิก Execute]**

[หยุด 2 วินาที รอผลลัพธ์]

👉 **[ชี้ที่ Response]**

ดู Response ที่ได้นะครับ

[หยุด 1 วินาที]

👉 **[ชี้ที่ bundleId และ bundleName]**

นี่คือ Bundle "Starter Pack"

[หยุด]

👉 **[ชี้ที่ availableStock: 30]**

เห็นไหมครับ

Bundle มีสต็อก 30 ชุด

[หยุด 1 วินาที]

👉 **[ชี้ที่ bottleneckItem]**

ตรงนี้สำคัญมาก

bottleneckItem บอกว่า

Product ไหนเป็นคอขวด

มี currentStock 30

requiredQuantity 1

possibleBundles 30

[หยุด 1 วินาที]

👉 **[Scroll ดู items array]**

แต่ละ item จะแสดงว่า

สามารถทำ Bundle ได้กี่ชุด

[หยุด]

Item แรก: possibleBundles 50

Item ที่สอง: possibleBundles 30

ดังนั้น Bundle มีแค่ 30 ชุด

เพราะ Item ที่สองเป็นคอขวด

[หยุด 2 วินาที]

**Decision สำคัญข้อสาม:**

ผมคำนวณแบบ Real-time

ไม่ใช้ Cache

[หยุด 1 วินาที]

ทำไม?

เพราะถ้ามีคนซื้อพร้อมกัน

หรือมีการอัพเดทสต็อกบ่อย

การคำนวณ Real-time

จะแม่นยำกว่า

ป้องกันการ Oversell ได้

[หยุด 1 วินาที]

ถ้าใช้ Cache

อาจจะได้ข้อมูลเก่า

แล้วขายเกินสต็อกที่มีจริง

[หยุด 2 วินาที]

---

## 🔐 ส่วนที่ 5: Feature ที่สาม - Transaction Management (1.5 นาที)

Feature ที่สาม

คือ Transaction Management

[หยุด 1 วินาที]

ปัญหาคือ

ตอนขาย Bundle

ต้องหักสต็อกหลายๆ ชิ้นพร้อมกัน

ถ้ามี Error ระหว่างทาง

ต้อง Rollback ทั้งหมด

[หยุด 2 วินาที]

👉 **[เปิดไฟล์ docs/TRANSACTION_MANAGEMENT_DETAILS.md]**

👉 **[Scroll หาส่วน Transaction Code]**

ดูโค้ดตัวอย่างนะครับ

[หยุด 1 วินาที]

มี BeginTransaction

ถ้าสำเร็จ ก็ Commit

ถ้า Error ก็ Rollback

[หยุด 2 วินาที]

นอกจากนี้

ผมยังใช้ Optimistic Locking

[หยุด 1 วินาที]

👉 **[Scroll หา RowVersion Example]**

ทุก Entity มี RowVersion field

ใช้ตรวจสอบว่า

มีคนแก้ไขข้อมูลพร้อมกันหรือไม่

[หยุด 1 วินาที]

ถ้า RowVersion ไม่ตรง

แปลว่ามีคนแก้ก่อนแล้ว

ระบบจะคืน 409 Conflict

แล้วให้ Client ลองใหม่

[หยุด 2 วินาที]

**Decision สำคัญข้อสี่:**

เลือกใช้ Optimistic Locking

แทน Pessimistic Locking

เพราะระบบเรา

อ่านข้อมูลมากกว่าเขียน

Optimistic เหมาะกว่า

[หยุด 2 วินาที]

---

## ✅ ส่วนที่ 6: Testing (1 นาที)

ต่อมาเรื่อง Testing ครับ

ผมทำ Unit Tests ครบถ้วน

[หยุด 1 วินาที]

👉 **[เปิด Terminal]**

👉 **[พิมพ์คำสั่ง:]**

```powershell
dotnet test
```

👉 **[กด Enter]**

[หยุด 3 วินาที รอผลลัพธ์]

เห็นไหมครับ

[หยุด]

👉 **[ชี้ที่ผลลัพธ์]**

Passed: 16 tests

Failed: 0 tests

Skipped: 1 test

Total: 17 tests

[หยุด 1 วินาที]

Test Coverage ครอบคลุม

Happy Path

Edge Cases

Error Handling

[หยุด 1 วินาที]

👉 **[เปิดไฟล์ tests/FlowAccount.Tests/BundleServiceTests.cs]**

👉 **[Scroll ดูโค้ด Test]**

ผมใช้ xUnit เป็น Testing Framework

Moq สำหรับ Mock dependencies

FluentAssertions เพื่อให้ Assert อ่านง่าย

[หยุด 1 วินาที]

ตัวอย่างเช่น

Test การคำนวณ Bundle Stock

Test การ Generate Variants

Test Error Handling

[หยุด 2 วินาที]

---

## 📚 ส่วนที่ 7: Documentation (30 วินาที)

เรื่อง Documentation ครับ

[หยุด 1 วินาที]

👉 **[เปิดไฟล์ docs/DOCUMENTATION_INDEX.md]**

ผมทำเอกสารครบ 22 ไฟล์

มี DESIGN_DECISIONS

DATABASE_DESIGN

BATCH_OPERATIONS

และอื่นๆ อีกเยอะ

[หยุด 1 วินาที]

ทุกเอกสารมี

ตัวอย่างโค้ด

Diagrams

และผลการทดสอบ

[หยุด 2 วินาที]

---

## 🎯 ส่วนที่ 8: Demo API จริง (2 นาที)

ตอนนี้ผมจะ Demo ระบบจริงนะครับ

[หยุด 1 วินาที]

👉 **[เปิด Terminal 1]**

รันคำสั่ง

```
dotnet run --project src/FlowAccount.API
```

[หยุด 2 วินาที รอ API start]

เห็นไหมครับ

API รันแล้ว

[หยุด 1 วินาที]

👉 **[เปิด Browser → Swagger UI]**

นี่คือ Swagger UI

แสดง API ทั้งหมด

[หยุด 1 วินาที]

ผมจะลองเรียก API สัก 2-3 ตัว

[หยุด 1 วินาที]

---

**API #1: GET /api/products**

👉 **[หา GET /api/products]**

👉 **[คลิก Try it out → Execute]**

เห็นไหมครับ

แสดงรายการ Products ทั้งหมด

[หยุด 2 วินาที]

---

**API #2: POST /api/products (สร้าง Product ใหม่)**

👉 **[หา POST /api/products]**

👉 **[คลิก Try it out]**

👉 **[Copy payload นี้ไป Paste:]**

```json
{
  "name": "T-Shirt",
  "description": "Cotton T-Shirt",
  "basePrice": 299.00,
  "baseCost": 150.00,
  "options": [
    {
      "name": "Size",
      "values": ["XS", "S", "M", "L", "XL"]
    },
    {
      "name": "Color",
      "values": ["Red", "Blue", "Green", "Yellow", "Black"]
    }
  ]
}
```

👉 **[Paste และ Execute]**

เห็นไหมครับ

สร้าง Product Master สำเร็จ

พร้อม Options ครบ

[หยุด 2 วินาที]

---

**API #3: GET /api/bundles/{id}/stock**

👉 **[หา GET /api/bundles/{id}/stock]**

👉 **[คลิก Try it out → ใส่ id = 1 → Execute]**

เห็นไหมครับ

แสดงสต็อก Bundle ได้ถูกต้อง

[หยุด 2 วินาที]

ตอนนี้ผมจะรัน Complete Test

[หยุด 1 วินาที]

👉 **[เปิด Terminal 2]**

👉 **[พิมพ์คำสั่ง:]**

```powershell
.\complete-test.ps1
```

👉 **[กด Enter]**

[หยุด 3 วินาที รอผลลัพธ์]

Script นี้จะทดสอบ

สร้าง ProductMaster

Generate Variants

สร้าง Bundle

คำนวณ Stock

ครบทุก Feature

[หยุด 2 วินาที]

เห็นไหมครับ

ผ่านทุก Test

[หยุด 2 วินาที]

---

## 🤖 ส่วนที่ 9: การใช้ AI 100% (1 นาที)

ผมพัฒนาโปรเจคนี้

ด้วย AI 100% ครับ

[หยุด 1 วินาที]

ใช้ GitHub Copilot

ตั้งแต่ต้นจนจบ

[หยุด 1 วินาที]

ขั้นตอนคือ

วิเคราะห์โจทย์

สร้างโครงสร้าง

เขียน Features

Review และ Refactor

สร้าง Tests

สร้าง Documentation

[หยุด 2 วินาที]

บทบาทของผมคือ

เป็น Reviewer

เป็น Decision Maker

ตรวจสอบทุกบรรทัด

และตัดสินใจ Design

[หยุด 2 วินาที]

ผลลัพธ์คือ

เสร็จภายใน 2 วัน

มี Quality สูง

Documentation ครบ

[หยุด 2 วินาที]

---

## 🎉 ส่วนที่ 10: สรุป (30 วินาที)

สรุปครับ

โปรเจคนี้มี

Clean Architecture

10 Design Patterns

Performance ดี

17 Unit Tests

22 ไฟล์ Documentation

[หยุด 2 วินาที]

ระบบพร้อมใช้งานจริง

และแสดงให้เห็นว่า

AI ช่วยเพิ่มประสิทธิภาพ

การพัฒนาได้มาก

[หยุด 2 วินาที]

ขอบคุณที่รับชมครับ

[หยุด 1 วินาที]

---

---

# 📋 วิธีใช้สคริปต์นี้

## 🎯 เตรียมตัวก่อนอัด (5 นาที)

**ขั้นตอนที่ 1: เปิดโปรแกรม**
- เปิด VS Code (แสดงโฟลเดอร์โปรเจค)
- เปิด Browser (Swagger UI)
- เปิด Terminal 2 หน้าต่าง
- ปิดโปรแกรมอื่นๆ ที่ไม่จำเป็น

**ขั้นตอนที่ 2: รัน API**

เปิด Terminal 1 แล้วพิมพ์:
```powershell
cd c:\Users\Chalermphan\source\flowaccout
dotnet run --project src/FlowAccount.API
```

รอจนเห็น "Now listening on" แล้วเปิด Browser ไปที่ Swagger

**ขั้นตอนที่ 3: เตรียม Terminal 2 (สำหรับรัน dotnet test)**

เปิด Terminal 2 ไว้พร้อม

ไม่ต้องรัน complete-test.ps1 ก่อน

เพราะเราจะสร้างข้อมูลใน Swagger โดยตรง

---

**💡 สำคัญ: วิดีโอนี้ทดสอบผ่าน Swagger เท่านั้น!**

เราจะสร้างและทดสอบทุกอย่างใน Swagger:
1. สร้าง Product Master (ได้ ID)
2. ใช้ ID นั้นสร้าง 250 Variants
3. ไม่ต้องรัน complete-test.ps1 - Demo แบบ Real-time ใน Swagger
4. โชว์แบบ Continuous Flow ไม่มีการหยุด

---

## 🎬 วิธีอ่านสคริปต์

**แบบง่าย (แนะนำ):**
1. เปิดไฟล์นี้ใน VS Code
2. กด `Ctrl + K` แล้วกด `Z` (Zen Mode เต็มจอ)
3. กด `Ctrl + Mouse Wheel` ขยายตัวอักษร
4. อ่านไปทีละบรรทัด
5. เลื่อนด้วย Mouse หรือ Arrow Down

**แบบมืออาชีพ:**
1. Copy เนื้อหาไปวางใน https://cueprompter.com
2. ตั้งค่า Font ให้ใหญ่
3. ตั้งค่า Speed ให้เหมาะกับการอ่าน
4. กด Play แล้วอ่านตาม

---

## 💡 Tips สำคัญ

**การพูด:**
- พูดช้าๆ ชัดเจน (ไม่เร็วเกินไป)
- เมื่อเห็น `[หยุด X วินาที]` = หยุดพูดตามจำนวนวินาที
- พูดผิดนิดหน่อย ไม่เป็นไร พูดต่อได้เลย
- ถ้าผิดมาก หยุด 2-3 วินาที แล้วเริ่มประโยคใหม่

**การแสดงหน้าจอ:**
- เมื่อเห็น `👉 [...]` = ทำ action นั้นบนหน้าจอ
- เคลื่อนเมาส์ช้าๆ ชี้ชัดเจน
- ถ้ามีคำว่า "Execute" หรือ "คลิก" = กดจริง รอผลลัพธ์

**เวลา:**
- เป้าหมาย: 12-13 นาที
- ถ้าเร็วเกินไป = หยุดนานขึ้น
- ถ้าช้าเกินไป = พูดเร็วขึ้นนิดหน่อย

---

## 🎥 โปรแกรมอัดหน้าจอแนะนำ

**1. OBS Studio (ฟรี + ดีที่สุด)**
- ดาวน์โหลด: https://obsproject.com
- ตั้งค่า: 1920x1080, 30fps, MP4

**2. Windows Game Bar (มีในเครื่อง)**
- กด Win + G
- คลิกปุ่มอัด
- กด Win + Alt + R เพื่อหยุด

**3. Loom (ง่ายที่สุด)**
- เว็บ: https://www.loom.com
- ใช้ผ่าน Browser ได้เลย

---

## ✅ Checklist สุดท้าย

**ก่อนกด Record:**
- [ ] API รันแล้ว (เห็น Swagger UI: http://localhost:5159/swagger)
- [ ] เปิดสคริปต์ไฟล์นี้พร้อมอ่าน (แนะนำ Zen Mode)
- [ ] เปิดส่วน JSON Payloads ไว้ใน Tab แยก (สำหรับ Copy-Paste)
- [ ] ไมค์ใช้งานได้ (ทดสอบแล้ว)
- [ ] ปิด Notifications ทั้งหมด (Windows, Browser, VS Code)
- [ ] ปิด chat apps อื่นๆ (Line, Teams, Discord)
- [ ] เตรียมใจพร้อมพูด

**หลังอัดเสร็จ:**
- [ ] เช็คเสียงชัดไหม
- [ ] เช็คภาพชัดไหม (รายละเอียด Swagger อ่านได้)
- [ ] เช็คความยาว 12-15 นาที (250 variants ใช้เวลานานกว่า)
- [ ] Export เป็น MP4
- [ ] ขนาดไฟล์ < 500MB

---

**พร้อมแล้ว ขอให้โชคดีครับ! 🎬🚀**

---

# 📦 JSON Payloads สำหรับ Copy-Paste

## 1️⃣ สร้าง Product Master พร้อม 10 Sizes + 25 Colors (POST /api/products)

```json
{
  "name": "T-Shirt Demo 250",
  "description": "Demo Product with 250 variants",
  "basePrice": 299.00,
  "baseCost": 150.00,
  "options": [
    {
      "name": "Size",
      "values": ["XXS", "XS", "S", "M", "L", "XL", "XXL", "3XL", "4XL", "5XL"]
    },
    {
      "name": "Color",
      "values": ["Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Pink", "Black", "White", "Gray", "Brown", "Navy", "Beige", "Cream", "Olive", "Maroon", "Teal", "Coral", "Mint", "Lavender", "Gold", "Silver", "Copper", "Bronze", "Charcoal"]
    }
  ]
}
```

**หมายเหตุ:**
- สร้าง Product พร้อม 10 ไซส์ และ 25 สี
- หลัง Execute ดู Response เพื่อเอา productMasterId และ optionIds
- Size option จะได้ id = 1, values = [1-10]
- Color option จะได้ id = 2, values = [11-35]

---

## 2️⃣ Generate 250 Variants ด้วย Bulk Insert (POST /api/products/{id}/generate-variants)

```json
{
  "productMasterId": 1,
  "selectedOptions": {
    "1": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
    "2": [11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35]
  },
  "priceStrategy": 0,
  "basePrice": 299.00,
  "baseCost": 150.00,
  "skuPattern": "DEMO-{Size}-{Color}"
}
```

**หมายเหตุ:**
- ใช้ productMasterId จาก Response ของ Step 1
- Key "1" = Size Option (values 1-10)
- Key "2" = Color Option (values 11-35)
- สร้าง 10×25 = 250 variants พร้อมกัน

---

## 3️⃣ สร้าง Bundle (POST /api/bundles)

```json
{
  "name": "Starter Pack",
  "description": "Bundle for beginners",
  "items": [
    {
      "variantId": 1,
      "quantity": 2
    },
    {
      "variantId": 2,
      "quantity": 1
    }
  ]
}
```

**หมายเหตุ:**
- variantId ต้องมีอยู่ในระบบ (ดูจาก GET /api/products/1/variants)
- quantity คือจำนวนที่ต้องใช้ใน Bundle

---

## 4️⃣ ดูสต็อก Bundle (GET /api/bundles/{id}/stock)

**ไม่ต้องใช้ JSON payload**

แค่ใส่ `id = 1` ในช่อง bundleId แล้วกด Execute

---

## 5️⃣ ดูรายการ Products (GET /api/products)

**ไม่ต้องใช้ JSON payload**

แค่กด Execute เลย

---

## 🎯 Quick Commands สำหรับ Terminal

```powershell
# Terminal 1: รัน API (ต้องรันก่อนเสมอ)
cd c:\Users\Chalermphan\source\flowaccout
dotnet run --project src/FlowAccount.API

# Terminal 2: รัน Unit Tests (ส่วนที่ 6 ของวิดีโอ)
dotnet test

# ========================================
# คำสั่งเสริม (ไม่ใช้ในวิดีโอหลัก)
# ========================================

# รัน Complete E2E Test (ทดสอบอัตโนมัติ 500 variants + Bundle)
.\complete-test.ps1

# ดู Test Status & Requirements
.\test-status.ps1
```

---

**💡 Tip:** เปิดไฟล์นี้ไว้ใน Tab แยก เพื่อ Copy-Paste JSON Payloads ได้ง่ายขณะอัดวิดีโอ!

---

**⚠️ หมายเหตุสำคัญ:**
- **วิดีโอนี้ Demo 250 variants ใน Swagger เท่านั้น** - ไม่ใช้ complete-test.ps1
- ทุกอย่างทดสอบแบบ Real-time ใน Swagger UI
- ไม่ต้อง Pre-seed ข้อมูล - สร้างใหม่ทั้งหมดระหว่างอัด
- **complete-test.ps1** สามารถใช้ทดสอบแบบอัตโนมัติได้ (ไม่บังคับ)
  - สร้าง Product "T-Shirt Premium" + 500 variants
  - ทดสอบ Bundle Stock + Transaction Management
  - มี cleanup อัตโนมัติ
