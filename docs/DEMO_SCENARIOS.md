# 🎬 Demo Scenarios Guide
## คู่มือสำหรับอัดวีดีโอและ Demo

---

## 🎯 ภาพรวม Demo Data

หลังจากรัน `demo-reset.ps1` คุณจะได้:

### **สินค้าที่มีอยู่:**
```
📦 Basic T-Shirt (เสื้อยืดคอกลม)
├── 🔴 แดง-M  (SKU: TSHIRT-RED-M)   - 50 ชิ้น
├── 🔴 แดง-L  (SKU: TSHIRT-RED-L)   - 30 ชิ้น
├── 🔵 น้ำเงิน-M (SKU: TSHIRT-BLUE-M) - 40 ชิ้น
└── 🔵 น้ำเงิน-L (SKU: TSHIRT-BLUE-L) - 20 ชิ้น

📊 รวม: 140 ชิ้น
💰 ราคา: 299 บาท/ชิ้น
```

---

## 🎬 Demo Scenario 1: แสดงข้อมูลพื้นฐาน

### **1.1 ดู Categories**
```
GET /api/categories
```
**ผลลัพธ์:** 2 categories (Clothing, Electronics)

### **1.2 ดู Products**
```
GET /api/products
```
**ผลลัพธ์:** 1 product (Basic T-Shirt) พร้อม 4 variants

### **1.3 ดู Product Details**
```
GET /api/products/1
```
**ไฮไลท์:**
- แสดงข้อมูลสินค้า
- แสดง variants ทั้ง 4 แบบ
- แสดงสต็อกแต่ละ variant

### **1.4 ดู Stock**
```
GET /api/stock/product/1
```
**ผลลัพธ์:** Stock ทั้งหมดของ T-Shirt

---

## 🎬 Demo Scenario 2: สร้าง Product ใหม่

### **2.1 สร้าง Product Master**
```json
POST /api/products

{
  "name": "Premium Hoodie",
  "description": "เสื้อฮู้ดพรีเมียม ผ้าหนา อุ่นสบาย",
  "categoryId": 1,
  "basePrice": 890.00,
  "isActive": true
}
```

### **2.2 สร้าง Variants แบบง่าย (Manual)**
```json
POST /api/products/{productId}/variants

{
  "sku": "HOODIE-BLACK-M",
  "barcode": "HOOD001",
  "price": 890.00,
  "attributes": [
    {
      "variantOptionId": 1,
      "variantOptionValueId": 3
    },
    {
      "variantOptionId": 2,
      "variantOptionValueId": 6
    }
  ]
}
```

### **2.3 สร้าง Variants แบบ Auto Generate**
```json
POST /api/products/{productId}/generate-variants

{
  "basePrice": 890.00,
  "priceAdjustments": [
    {
      "variantOptionValueId": 8,
      "adjustment": 50.00
    }
  ],
  "skuPattern": "HOODIE-{color}-{size}",
  "combinations": [
    {
      "variantOptions": [
        { "variantOptionId": 1, "variantOptionValueId": 3 },
        { "variantOptionId": 2, "variantOptionValueId": 6 }
      ]
    },
    {
      "variantOptions": [
        { "variantOptionId": 1, "variantOptionValueId": 3 },
        { "variantOptionId": 2, "variantOptionValueId": 7 }
      ]
    },
    {
      "variantOptions": [
        { "variantOptionId": 1, "variantOptionValueId": 3 },
        { "variantOptionId": 2, "variantOptionValueId": 8 }
      ]
    }
  ]
}
```
**ไฮไลท์:** สร้าง 3 variants พร้อมกันในคำสั่งเดียว (ดำ-M, ดำ-L, ดำ-XL)

---

## 🎬 Demo Scenario 3: Bundle Operations

### **3.1 สร้าง Bundle - ชุดสุดคุ้ม**
```json
POST /api/bundles

{
  "name": "Summer Set",
  "description": "ชุดซัมเมอร์ เสื้อยืด 2 ตัว",
  "bundlePrice": 499.00,
  "isActive": true,
  "items": [
    {
      "productVariantId": 1,
      "quantity": 1
    },
    {
      "productVariantId": 3,
      "quantity": 1
    }
  ]
}
```
**ไฮไลท์:**
- เสื้อแดง-M (299) + เสื้อน้ำเงิน-M (299) = 598 บาท
- ราคา Bundle: 499 บาท → **ประหยัด 99 บาท!**

### **3.2 ดู Bundles ทั้งหมด**
```
GET /api/bundles
```

### **3.3 ดู Bundle Details**
```
GET /api/bundles/{bundleId}
```
**ไฮไลท์:** แสดงรายการสินค้าใน bundle + ราคาประหยัด

### **3.4 ตรวจสอบ Stock Availability**
```
GET /api/bundles/{bundleId}/stock-availability
```

---

## 🎬 Demo Scenario 4: Stock Management

### **4.1 เพิ่ม Stock**
```json
POST /api/stock/adjust

{
  "productVariantId": 1,
  "warehouseId": 1,
  "quantity": 100,
  "adjustmentType": "In",
  "reason": "เติมสต็อกใหม่"
}
```
**ก่อน:** 50 ชิ้น → **หลัง:** 150 ชิ้น

### **4.2 ลด Stock**
```json
POST /api/stock/adjust

{
  "productVariantId": 1,
  "warehouseId": 1,
  "quantity": 10,
  "adjustmentType": "Out",
  "reason": "ขายหน้าร้าน"
}
```
**ก่อน:** 150 ชิ้น → **หลัง:** 140 ชิ้น

### **4.3 ดู Stock Movement History**
```
GET /api/stock/product/1/movements
```
**ไฮไลท์:** แสดงประวัติการเคลื่อนไหวสต็อกทั้งหมด

---

## 🎬 Demo Scenario 5: Reserve & Release Stock

### **5.1 จอง Stock (Reserve)**
```json
POST /api/stock/reserve

{
  "productVariantId": 1,
  "warehouseId": 1,
  "quantity": 5,
  "referenceId": "ORDER-001",
  "referenceType": "Sales Order"
}
```
**ผลลัพธ์:**
- Available: 145 → 140
- Reserved: 0 → 5

### **5.2 ปล่อย Stock (Release)**
```json
POST /api/stock/release

{
  "productVariantId": 1,
  "warehouseId": 1,
  "quantity": 5,
  "referenceId": "ORDER-001",
  "referenceType": "Sales Order"
}
```
**ผลลัพธ์:**
- Available: 140 → 145
- Reserved: 5 → 0

---

## 🎬 Demo Scenario 6: Batch Operations

### **6.1 Batch Create Products**
```json
POST /api/products/batch

{
  "products": [
    {
      "name": "Casual Jeans",
      "description": "กางเกงยีนส์ขายาว",
      "categoryId": 1,
      "basePrice": 790.00,
      "isActive": true
    },
    {
      "name": "Sport Shorts",
      "description": "กางเกงขาสั้นกีฬา",
      "categoryId": 1,
      "basePrice": 390.00,
      "isActive": true
    }
  ]
}
```
**ไฮไลท์:** สร้าง 2 products พร้อมกัน

### **6.2 Batch Update Prices**
```json
PUT /api/products/batch/prices

{
  "updates": [
    {
      "productId": 1,
      "newPrice": 349.00
    },
    {
      "productId": 2,
      "newPrice": 949.00
    }
  ]
}
```

### **6.3 Batch Stock Adjustment**
```json
POST /api/stock/batch/adjust

{
  "warehouseId": 1,
  "adjustments": [
    {
      "productVariantId": 1,
      "quantity": 20,
      "adjustmentType": "In"
    },
    {
      "productVariantId": 2,
      "quantity": 15,
      "adjustmentType": "In"
    }
  ],
  "reason": "เติมสต็อกรายวัน"
}
```

---

## 🎬 Demo Scenario 7: Error Handling

### **7.1 ลอง Reserve เกินสต็อก**
```json
POST /api/stock/reserve

{
  "productVariantId": 1,
  "warehouseId": 1,
  "quantity": 999999,
  "referenceId": "ORDER-002",
  "referenceType": "Sales Order"
}
```
**ผลลัพธ์:** ❌ Error - Insufficient stock

### **7.2 ลองสร้าง Product ไม่ครบข้อมูล**
```json
POST /api/products

{
  "name": "",
  "basePrice": -100
}
```
**ผลลัพธ์:** ❌ Validation errors

---

## 🎯 เคล็ดลับสำหรับการ Demo

### **ลำดับที่แนะนำ:**
1. ✅ แสดงข้อมูลที่มีอยู่ (Scenario 1)
2. ✅ สร้าง Product ใหม่ (Scenario 2)
3. ✅ สร้าง Bundle (Scenario 3)
4. ✅ จัดการ Stock (Scenario 4)
5. ✅ Reserve/Release (Scenario 5)
6. ✅ Batch Operations (Scenario 6)
7. ✅ Error Handling (Scenario 7)

### **จุดเด่นที่ควรเน้น:**
- 🚀 **Auto Generate Variants** - สร้างหลาย variants พร้อมกัน
- 💰 **Bundle Pricing** - คำนวณส่วนลดอัตโนมัติ
- 📊 **Stock Management** - Reserve/Release ที่แม่นยำ
- ⚡ **Batch Operations** - ประมวลผลหลายรายการพร้อมกัน
- 🛡️ **Error Handling** - Validation ที่ชัดเจน

### **การรีเซ็ตระหว่างอัด:**
```powershell
# ถ้าต้องการเริ่มใหม่ตอนกลาง demo
.\demo-reset.ps1
```

---

## 📝 Checklist ก่อนอัด

- [ ] รัน `demo-reset.ps1` เรียบร้อย
- [ ] API กำลังรันอยู่
- [ ] เปิด Swagger: http://localhost:5000/swagger
- [ ] ทดสอบ API ทุก endpoint ก่อนอัด
- [ ] เตรียม JSON requests ไว้ใน clipboard
- [ ] ปิดแท็บและแอปพลิเคชันที่ไม่จำเป็น
- [ ] ตั้งค่า browser zoom ให้เหมาะกับการอัดวีดีโอ

---

## 🎬 Ready to Record!

**Quick Start:**
```powershell
# Reset database
.\demo-reset.ps1

# Start API
cd src\FlowAccount.API
dotnet run

# Open Swagger
start http://localhost:5000/swagger
```

🎉 **Happy Recording!**
