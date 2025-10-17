# 🧪 คู่มือทดสอบใน Swagger ก่อนอัดวิดีโอ

## 🎯 URL Swagger UI
**http://localhost:5159/swagger** หรือ **http://localhost:5159**

---

## 📝 ขั้นตอนการทดสอบ (ตามสคริปต์วิดีโอ)

### ✅ STEP 1: สร้าง Product Master พร้อม 10 Sizes + 25 Colors

**Endpoint:** `POST /api/products`

**JSON Payload:**
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

**คาดหวัง Response:**
- ✅ Status: 200 OK
- ✅ ได้ `productMasterId` (เช่น `"id": 1`)
- ✅ Size Option มี `id` และ 10 values (id: 1-10)
- ✅ Color Option มี `id` และ 25 values (id: 11-35)

**สิ่งที่ต้องจด:**
- 📝 `productMasterId` = _______
- 📝 Size Option `id` = _______
- 📝 Color Option `id` = _______

---

### ✅ STEP 2: Generate 250 Variants

**Endpoint:** `POST /api/products/{productMasterId}/generate-variants`

⚠️ **แทนที่ `{productMasterId}` ด้วย ID จาก Step 1**

**JSON Payload:**
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

⚠️ **อย่าลืม:** เปลี่ยน `productMasterId`, key `"1"`, และ key `"2"` ให้ตรงกับ Option IDs จาก Step 1

**คาดหวัง Response:**
- ✅ Status: 200 OK
- ✅ `"totalVariantsGenerated": 250`
- ✅ `"processingTime": "00:00:02.xxx"` (ประมาณ 2 วินาที)
- ✅ เห็น SKU เช่น `"DEMO-XXS-Red"`, `"DEMO-M-Blue"`

**สิ่งที่ต้องสังเกต:**
- ✅ เวลาที่ใช้ (Performance)
- ✅ จำนวน variants = 250 ตัว
- ✅ SKU Pattern ถูกต้อง

---

### ✅ STEP 3: ตรวจสอบ Variants ที่สร้าง

**Endpoint:** `GET /api/products/{productMasterId}`

**คาดหวัง Response:**
- ✅ Status: 200 OK
- ✅ Product มี `productVariants` array
- ✅ จำนวน variants = 250

---

### ✅ STEP 4 (Optional): สร้าง Bundle

**Endpoint:** `POST /api/bundles`

**JSON Payload:**
```json
{
  "name": "Starter Pack",
  "description": "Bundle for beginners",
  "price": 499.00,
  "isActive": true,
  "items": [
    {
      "itemType": "Variant",
      "itemId": 1,
      "quantity": 2
    },
    {
      "itemType": "Variant",
      "itemId": 2,
      "quantity": 1
    }
  ]
}
```

⚠️ **อย่าลืม:** เปลี่ยน `itemId` ให้ตรงกับ Variant IDs ที่มีอยู่จริง

**คาดหวัง Response:**
- ✅ Status: 200 OK
- ✅ ได้ `bundleId`

---

### ✅ STEP 5: คำนวณ Bundle Stock

**Endpoint:** `POST /api/bundles/{bundleId}/calculate-stock`

**JSON Payload:**
```json
{
  "bundleId": 1,
  "warehouseId": 1
}
```

**คาดหวัง Response:**
- ✅ Status: 200 OK
- ✅ `maxAvailableBundles` = จำนวนที่สามารถทำได้
- ✅ แสดง `bundleItemStockInfo` พร้อม bottleneck

---

## 🎬 เตรียมพร้อมอัดวิดีโอ

### ✅ Checklist ก่อนอัด:
- [ ] API รันแล้ว (http://localhost:5159)
- [ ] เปิด Swagger UI ใน Browser
- [ ] เปิด VIDEO_SCRIPT_TPROMPTER.md ใน VS Code (Zen Mode)
- [ ] เปิดไฟล์นี้ (SWAGGER_TEST_GUIDE.md) ใน Tab แยก
- [ ] ทดสอบ STEP 1-2 สำเร็จแล้ว
- [ ] ปิด Notifications ทั้งหมด
- [ ] เตรียมโปรแกรมอัดหน้าจอ (OBS/Game Bar/Loom)

### 📊 ผลลัพธ์ที่ต้องได้:
- ✅ Product สร้างสำเร็จ
- ✅ 250 Variants สร้างเสร็จใน ~2 วินาที
- ✅ SKU ถูกต้องตาม Pattern
- ✅ แสดง Performance ที่ดี

---

## 🔧 Troubleshooting

### ❌ ถ้า API ไม่ทำงาน:
```powershell
# ตรวจสอบ API running
dotnet run --project src/FlowAccount.API
```

### ❌ ถ้า Swagger ไม่เปิด:
- ลองเข้า: http://localhost:5159
- หรือ: http://localhost:5159/swagger/index.html

### ❌ ถ้า Product/Variant IDs ไม่ตรง:
- ดู Response จาก Step 1 อีกครั้ง
- Copy IDs ที่ถูกต้อง
- แก้ไข JSON Payload

---

**พร้อมแล้ว เริ่มทดสอบได้เลย! 🚀**
