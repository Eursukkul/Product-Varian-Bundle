# 🎥 Quick Video Recording Guide (5 Minutes)

## สำหรับคนที่รีบ - อ่านเฉพาะนี้ก็พอ!

---

## 🎯 ขั้นตอนสั้นๆ (4 ขั้นตอน)

### 1️⃣ เตรียมก่อนกด Record (5 นาที)

```powershell
# เปิด Terminal และรัน:
cd C:\Users\Chalermphan\source\flowaccout
cd src/FlowAccount.API
dotnet run
```

เปิด Browser:
- Tab 1: https://github.com/Eursukkul/Product-Varian-Bundle
- Tab 2: http://localhost:5159/swagger

---

### 2️⃣ พูดตาม Script (5-7 นาที)

#### **Part 1: Intro (30 วินาที)**
> "สวัสดีครับ วันนี้จะนำเสนอ FlowAccount Product Variant & Bundle System ครับ
> 
> เป็นระบบ Backend API สำหรับจัดการ Product Variants และ Bundles
> 
> โค้ดอยู่ที่ GitHub: github.com/Eursukkul/Product-Varian-Bundle"

**แสดง:** หน้า GitHub

---

#### **Part 2: อธิบายโปรเจค (1 นาที)**
> "โปรเจคนี้ทำตามโจทย์ 2 ข้อ:
> 
> **ข้อ 1 - Database Schema:**
> ออกแบบ 10 ตาราง ได้แก่ ProductMaster, VariantOption, ProductVariant, Bundle, BundleItem และอื่นๆ
> ใช้ Entity Framework Core กับ SQL Server
> 
> **ข้อ 2 - API Endpoints:**
> สร้าง RESTful API สำหรับ CRUD operations
> มี Request/Response JSON payload ครบถ้วน
> 
> **Architecture:**
> ใช้ Clean Architecture แบ่ง 4 layers คือ API, Application, Domain, และ Infrastructure"

**แสดง:** โครงสร้าง folder ใน VS Code

---

#### **Part 3: แนวทางที่ใช้ (1 นาที)**
> "ผมใช้ Clean Architecture เพราะ:
> - แยก Business Logic ออกจาก Infrastructure
> - ง่ายต่อการ test และ maintain
> 
> **Technology Stack:**
> - .NET 8 Web API
> - Entity Framework Core
> - SQL Server
> - AutoMapper, Serilog
> - xUnit สำหรับ testing
> 
> มี Unit Tests 16 tests ผ่านหมด 100%"

**แสดง:** Test results

---

#### **Part 4: Demo หลัก (2 นาที) ⭐ สำคัญที่สุด!**
> "ตอนนี้จะ demo feature หลัก คือ Batch Operations - สร้าง 250 variants พร้อมกัน
> 
> ผมมีสินค้า 'Ultimate T-Shirt Collection' ที่มี:
> - 10 ไซส์ (XS ถึง 6XL)
> - 5 สี
> - 5 วัสดุ
> 
> รวม 10 × 5 × 5 = 250 variants"

**ใน Swagger UI:**
1. เปิด POST /api/Products/10/generate-variants
2. กด "Try it out"
3. ใส่ Request Body:

```json
{
  "productMasterId": 10,
  "selectedOptions": {
    "17": [82, 83, 84, 85, 86, 87, 88, 89, 90, 91],
    "18": [92, 93, 94, 95, 96],
    "19": [97, 98, 99, 100, 101]
  },
  "priceStrategy": 0,
  "fixedPrice": 299.00,
  "baseCost": 150.00
}
```

4. กด Execute

> "เห็นไหมครับ... ระบบสร้าง 250 variants สำเร็จใน 2 วินาที!
> 
> SKU ถูก generate อัตโนมัติ: ULTIMATE-XS-BLACK-COTTON ไปจนถึง ULTIMATE-6XL-GREEN-ECO
> 
> Performance: 8.2 milliseconds ต่อ variant"

**แสดง:** Response ใน Swagger

---

#### **Part 5: Important Decisions (1 นาที)**
> "ผมมี 3 decisions สำคัญ:
> 
> **1. ใช้ Clean Architecture**
> เพราะต้องการ code ที่ maintainable และ testable
> 
> **2. จำกัดที่ 250 variants**
> เพื่อป้องกัน performance issues และ timeout
> 
> **3. ใช้ Transaction Management**
> เพื่อให้การตัดสต็อกเป็น atomic - สำเร็จทั้งหมดหรือล้มเหลวทั้งหมด ไม่มีครึ่งๆ กลางๆ"

**แสดง:** Documentation หรือ Code

---

#### **Part 6: สรุป (30 วินาที)**
> "สรุปครับ:
> 
> ✅ Database Schema ครบ 10 ตาราง
> ✅ API Endpoints ครบทุก requirements
> ✅ Batch Operations ทดสอบจริง 250 variants
> ✅ Transaction Management มี
> ✅ Stock Logic มี bottleneck detection
> 
> โค้ดครบอยู่ที่ GitHub แล้ว
> 
> ขอบคุณครับ!"

**แสดง:** GitHub repository

---

### 3️⃣ เครื่องมือบันทึก (เลือก 1 อย่าง)

**แบบง่ายสุด - Windows Game Bar:**
1. กด `Win + G`
2. กด Record button (วงกลมสีแดง)
3. พูดตาม script ข้างบน
4. เสร็จแล้วกด `Win + Alt + R` เพื่อหยุด
5. ไฟล์จะอยู่ที่ `C:\Users\[Username]\Videos\Captures`

**แบบโปร - OBS Studio (ฟรี):**
1. ดาวน์โหลด: https://obsproject.com/
2. เปิดโปรแกรม → Start Recording
3. พูดตาม script
4. Stop Recording
5. ไฟล์ใน Videos folder

---

### 4️⃣ อัปโหลดและส่ง

**Upload ที่ YouTube (แนะนำ):**
1. ไปที่ youtube.com
2. กด Upload → Select file
3. Title: "FlowAccount Product Variant & Bundle System - [Your Name]"
4. Visibility: **Unlisted** (ไม่ใช่ Public)
5. รอ process เสร็จ
6. คัดลอก link

**หรือ Google Drive:**
1. ไปที่ drive.google.com
2. Upload file
3. Right-click → Share → Anyone with the link can view
4. คัดลอก link

---

## ✅ Final Submission Email Template

```
Subject: FlowAccount Project Submission - [Your Name]

เรียน [ชื่อผู้รับ]

ส่งงานโปรเจค FlowAccount Product Variant & Bundle System ดังนี้ครับ:

1. GitHub Repository:
   https://github.com/Eursukkul/Product-Varian-Bundle

2. Video Presentation (ระยะเวลา X นาที):
   [ใส่ link YouTube หรือ Google Drive]

โปรเจคนี้ครบทุก requirements:
✅ Database Schema (10 tables)
✅ API Endpoints (CRUD operations)
✅ Batch Operations (250 variants tested)
✅ Transaction Management
✅ Stock Logic with bottleneck detection

ขอบคุณครับ

[Your Name]
```

---

## 💡 เคล็ดลับสำคัญ

### ✅ ทำ
- พูดช้าๆ ชัดๆ
- แสดง demo จริงๆ (ไม่ต้องพูดอย่างเดียว)
- มั่นใจ ยิ้ม
- ถ้าพูดผิด หยุดแล้วพูดใหม่ (ตัดทิ้งได้)

### ❌ ไม่ควรทำ
- อย่ารีบ (มีเวลา 5-7 นาที พอ)
- อย่าอ่าน script แบบไม่มีอารมณ์
- อย่าข้ามส่วน demo (สำคัญมาก!)
- อย่าลืมแสดงหน้าจอ (อย่าแค่พูด)

---

## ⏱️ Timeline สำหรับคนรีบมาก

**มีเวลา 1 ชั่วโมง:**
- 0:00-0:10 → เตรียม (start API, เปิด browser)
- 0:10-0:20 → อ่าน script ซักรอบ
- 0:20-0:30 → ทดสอบพูด 1 รอบ
- 0:30-0:45 → บันทึกจริง (ใช้ Windows Game Bar)
- 0:45-0:55 → อัปโหลด YouTube/Google Drive
- 0:55-1:00 → ส่ง email

---

## 🎬 You Can Do This! 

อย่ากลัวครับ! คุณมีของพร้อมแล้ว:
- ✅ Code ใช้งานได้
- ✅ อยู่บน GitHub แล้ว
- ✅ Documentation ครบ
- ✅ ทุกอย่างทดสอบแล้ว

แค่บันทึกวิดีโออธิบาย 5-7 นาที ก็เสร็จแล้ว! 💪

**Good luck! 🚀**
