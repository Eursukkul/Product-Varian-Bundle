# 🎯 Payload สำหรับ Demo วิดีโอ

> เตรียม Payload เหล่านี้ไว้ก่อนอัดวิดีโอ - Copy & Paste ได้เลย!

---

## 📦 Demo 1: Batch Operations - สร้าง Product + Generate Variants

### Step 1: สร้าง Product Master

**Endpoint:** `POST /api/products`

**Payload:**
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

**ผลลัพธ์:** จะได้ Product ID (เช่น `1`)

---

### Step 2: Generate Variants (Cartesian Product)

**Endpoint:** `POST /api/products/{id}/generate-variants`

**URL Example:** `POST /api/products/1/generate-variants`

**ไม่ต้องส่ง Body** - เอา ID จาก Step 1 ใส่ใน URL

**ผลลัพธ์:** ระบบจะสร้าง 6 Variants:
1. Red - S
2. Red - M
3. Blue - S
4. Blue - M
5. Green - S
6. Green - M

---

## 🎁 Demo 2: Stock Logic - คำนวณ Bundle Stock

### Step 1: สร้าง Bundle (ถ้ายังไม่มี)

**Endpoint:** `POST /api/bundles`

**Payload:**
```json
{
  "name": "Summer Combo",
  "description": "T-Shirt and Hat Bundle",
  "bundlePrice": 499,
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

**คำอธิบาย:**
- Variant ID 1 = T-Shirt (Red, M) จำนวน 2 ตัว
- Variant ID 2 = Hat (Blue) จำนวน 1 ใบ

---

### Step 2: คำนวณว่าขายได้กี่ชุด

**Endpoint:** `POST /api/bundles/calculate-stock`

**Payload:**
```json
{
  "bundleId": 1,
  "warehouseId": 1,
  "quantity": 10
}
```

**คำอธิบาย:**
- ถามว่า Bundle ID 1 สามารถขายได้ 10 ชุดไหม?
- เช็คจาก Warehouse ID 1

**Response ตัวอย่าง:**
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
  ],
  "message": "Cannot fulfill 10 bundles. Maximum available: 5"
}
```

**อธิบายใน Video:**
- ถ้า T-Shirt มี 10 ตัว, Hat มี 5 ใบ
- จะขายได้แค่ 5 ชุด (จำกัดด้วย Hat)

---

## 💰 Demo 3: Transaction Management - ขาย Bundle

### Endpoint: ขาย Bundle

**Endpoint:** `POST /api/bundles/sell`

**Payload:**
```json
{
  "bundleId": 1,
  "quantity": 2,
  "warehouseId": 1
}
```

**คำอธิบาย:**
- ขาย Bundle ID 1 จำนวน 2 ชุด
- จาก Warehouse ID 1

**ผลลัพธ์:**
```json
{
  "success": true,
  "message": "Bundle sold successfully",
  "transactionId": "TXN-20251017-001",
  "bundleId": 1,
  "bundleName": "Summer Combo",
  "quantitySold": 2,
  "totalPrice": 998,
  "stockDeductions": [
    {
      "variantId": 1,
      "variantName": "T-Shirt (Red, M)",
      "quantityDeducted": 4,
      "remainingStock": 6
    },
    {
      "variantId": 2,
      "variantName": "Hat (Blue)",
      "quantityDeducted": 2,
      "remainingStock": 3
    }
  ]
}
```

**จุดเด่นที่ต้องอธิบาย:**
1. ✅ ตรวจสอบสต็อคทั้งหมดก่อน
2. ✅ ลดสต็อคทุกตัวใน 1 Transaction
3. ✅ ถ้าสต็อคไม่พอ → Rollback ทั้งหมด
4. ✅ รับประกัน ACID Properties

---

## 🧪 Demo 4: แสดงว่าสต็อคไม่พอ (Error Case)

**Endpoint:** `POST /api/bundles/sell`

**Payload:** (ขายมากเกินไป)
```json
{
  "bundleId": 1,
  "quantity": 100,
  "warehouseId": 1
}
```

**Response (Error):**
```json
{
  "success": false,
  "message": "Insufficient stock for bundle items",
  "errors": [
    {
      "variantId": 2,
      "variantName": "Hat (Blue)",
      "required": 100,
      "available": 5,
      "shortage": 95
    }
  ]
}
```

**อธิบายใน Video:**
- เมื่อสต็อคไม่พอ → ไม่ลดเลย
- Transaction Rollback
- ข้อมูลยังคงถูกต้อง

---

## 📊 Demo 5: Adjust Stock (เพิ่มสต็อค)

**Endpoint:** `POST /api/stock/adjust`

**Payload:**
```json
{
  "variantId": 1,
  "warehouseId": 1,
  "quantity": 50,
  "adjustmentType": "Add",
  "reason": "New stock arrived"
}
```

**คำอธิบาย:**
- เพิ่มสต็อค Variant ID 1 จำนวน 50 ตัว
- ที่ Warehouse ID 1

---

## 🔍 Demo 6: Query Stock

**Endpoint:** `POST /api/stock/query`

**Payload:**
```json
{
  "variantId": 1,
  "warehouseId": 1
}
```

**Response:**
```json
{
  "variantId": 1,
  "variantName": "T-Shirt (Red, M)",
  "sku": "TSHIRT-001-RED-M",
  "warehouseId": 1,
  "warehouseName": "Main Warehouse",
  "quantity": 50,
  "lastUpdated": "2025-10-17T10:30:00Z"
}
```

---

## 📋 เตรียมข้อมูลก่อน Demo

### ก่อนอัดวิดีโอ ให้รัน:

```powershell
# รีเซ็ตและสร้างข้อมูล Demo
cd c:\Users\Chalermphan\source\flowaccout
.\demo-reset.ps1
```

Script นี้จะสร้าง:
- ✅ Categories
- ✅ Warehouses
- ✅ Products พื้นฐาน
- ✅ Variants
- ✅ Stock
- ✅ Bundles

---

## 💡 Tips สำหรับ Demo

### 1. เตรียม Payloads ไว้ในไฟล์

สร้างไฟล์ `demo-payloads.txt` หรือ `demo-payloads.json`

### 2. Copy & Paste ใน Swagger

- เปิด Swagger UI
- คลิก "Try it out"
- Paste Payload
- คลิก "Execute"

### 3. ลำดับ Demo ที่แนะนำ

```
1. สร้าง Product → Generate Variants (แสดง Batch)
2. Calculate Bundle Stock (แสดง Stock Logic)
3. Sell Bundle (แสดง Transaction)
4. Sell Bundle เกิน (แสดง Error Handling)
```

### 4. สิ่งที่ต้องชี้ให้เห็น

✅ **Request Payload** - ชี้ให้เห็นโครงสร้าง  
✅ **Response** - ชี้ผลลัพธ์ที่สำคัญ  
✅ **HTTP Status Code** - 200, 201, 400, etc.  
✅ **Execution Time** - แสดงความเร็ว

---

## 🎬 ตัวอย่างคำพูด

### Demo Batch Operations:
```
"ผมจะสร้างสินค้า T-Shirt ที่มี 3 สี และ 2 ขนาด
[Paste Payload]
กด Execute...
เห็นไหมครับว่าได้ Product ID = 1

ตอนนี้จะให้ระบบสร้าง Variants อัตโนมัติ
[เรียก Generate Variants API]
เห็นไหมครับ ระบบสร้าง 6 Variants ให้
3 สี × 2 ขนาด = 6 Variants
ใช้ Cartesian Product Algorithm"
```

### Demo Stock Logic:
```
"ตอนนี้มี Bundle ชื่อ Summer Combo
ประกอบด้วย T-Shirt 2 ตัว + Hat 1 ใบ

ถ้าเรามีสต็อค T-Shirt 10 ตัว, Hat 5 ใบ
คำนวณได้ว่าขายได้กี่ชุด?

[เรียก Calculate Stock API]
เห็นไหมครับ จากสต็อคที่มี
ระบบบอกว่าขายได้แค่ 5 ชุด
เพราะจำกัดด้วย Hat ที่มีแค่ 5 ใบ"
```

### Demo Transaction:
```
"สุดท้าย เมื่อขาย Bundle จริง
ระบบจะใช้ Transaction Management

1. ตรวจสอบสต็อคทั้งหมดก่อน
2. ถ้าพอ → ลดสต็อคทุกตัวพร้อมกัน
3. ถ้าไม่พอ → Rollback ไม่ลดเลย

รับประกัน ACID Properties
ข้อมูลถูกต้องเสมอ"
```

---

## 🎯 สรุป

### Payloads ที่ต้องเตรียม:

1. ✅ Create Product (with Variant Options)
2. ✅ Generate Variants (ไม่ต้องส่ง body)
3. ✅ Calculate Bundle Stock
4. ✅ Sell Bundle (Success case)
5. ✅ Sell Bundle (Error case - เพื่อแสดง Error Handling)

### Copy ไฟล์นี้ไปเปิดข้างๆ ตอนอัดวิดีโอ!

**พร้อม Demo แล้ว!** 🎥✨
