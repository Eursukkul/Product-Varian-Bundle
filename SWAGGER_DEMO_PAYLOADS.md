# Swagger Demo Payloads

## สำหรับใช้ใน Video Demo

---

## 1. Create ProductMaster

**Endpoint:** `POST /api/products`

```json
{
  "name": "T-Shirt Premium",
  "description": "High quality cotton t-shirt",
  "sku": "TSH-2024-001",
  "categoryId": 1,
  "basePrice": 299.00
}
```

**Expected Response:**
```json
{
  "id": 1,
  "name": "T-Shirt Premium",
  "description": "High quality cotton t-shirt",
  "sku": "TSH-2024-001",
  "categoryId": 1,
  "basePrice": 299.00,
  "isActive": true,
  "createdDate": "2025-10-17T10:00:00Z"
}
```

---

## 2. Generate Variants (Batch Operation)

**Endpoint:** `POST /api/products/{productMasterId}/variants/generate`

**URL Parameter:** `productMasterId = 1`

```json
{
  "productMasterId": 1,
  "attributeCombinations": [
    {
      "attributeName": "Size",
      "values": ["S", "M", "L", "XL"]
    },
    {
      "attributeName": "Color",
      "values": ["Red", "Blue", "Black", "White"]
    }
  ]
}
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Successfully generated 16 variants",
  "variantsCreated": 16,
  "variants": [
    {
      "id": 1,
      "sku": "TSH-2024-001-S-Red",
      "productMasterId": 1,
      "attributes": [
        { "attributeName": "Size", "value": "S" },
        { "attributeName": "Color", "value": "Red" }
      ],
      "price": 299.00,
      "stockQuantity": 0
    }
    // ... 15 more variants
  ]
}
```

---

## 3. Update Variant Stock

**Endpoint:** `PUT /api/products/variants/{variantId}/stock`

**URL Parameter:** `variantId = 1`

```json
{
  "stockQuantity": 100
}
```

**Expected Response:**
```json
{
  "id": 1,
  "sku": "TSH-2024-001-S-Red",
  "stockQuantity": 100,
  "updatedDate": "2025-10-17T10:05:00Z"
}
```

---

## 4. Create Bundle

**Endpoint:** `POST /api/bundles`

```json
{
  "name": "Summer Starter Pack",
  "description": "Perfect combo for summer",
  "items": [
    {
      "variantId": 1,
      "quantity": 2
    },
    {
      "variantId": 5,
      "quantity": 1
    },
    {
      "variantId": 9,
      "quantity": 1
    }
  ],
  "bundlePrice": 999.00
}
```

**Expected Response:**
```json
{
  "id": 1,
  "name": "Summer Starter Pack",
  "description": "Perfect combo for summer",
  "bundlePrice": 999.00,
  "items": [
    {
      "variantId": 1,
      "variantSku": "TSH-2024-001-S-Red",
      "quantity": 2
    },
    {
      "variantId": 5,
      "variantSku": "TSH-2024-001-M-Red",
      "quantity": 1
    },
    {
      "variantId": 9,
      "variantSku": "TSH-2024-001-S-Blue",
      "quantity": 1
    }
  ],
  "availableStock": 50,
  "isActive": true
}
```

---

## 5. Get Bundle Stock

**Endpoint:** `GET /api/bundles/{bundleId}/stock`

**URL Parameter:** `bundleId = 1`

**Expected Response:**
```json
{
  "bundleId": 1,
  "bundleName": "Summer Starter Pack",
  "availableStock": 50,
  "calculation": {
    "components": [
      {
        "variantId": 1,
        "variantSku": "TSH-2024-001-S-Red",
        "requiredQuantity": 2,
        "availableStock": 100,
        "maxBundles": 50
      },
      {
        "variantId": 5,
        "variantSku": "TSH-2024-001-M-Red",
        "requiredQuantity": 1,
        "availableStock": 80,
        "maxBundles": 80
      },
      {
        "variantId": 9,
        "variantSku": "TSH-2024-001-S-Blue",
        "requiredQuantity": 1,
        "availableStock": 60,
        "maxBundles": 60
      }
    ],
    "bottleneck": {
      "variantId": 1,
      "variantSku": "TSH-2024-001-S-Red",
      "maxBundles": 50
    }
  }
}
```

---

## 6. Get All Products

**Endpoint:** `GET /api/products`

**Expected Response:**
```json
{
  "data": [
    {
      "id": 1,
      "name": "T-Shirt Premium",
      "sku": "TSH-2024-001",
      "basePrice": 299.00,
      "variantCount": 16,
      "isActive": true
    }
  ],
  "totalCount": 1,
  "pageSize": 10,
  "currentPage": 1
}
```

---

## 7. Get Product with Variants

**Endpoint:** `GET /api/products/{productId}/variants`

**URL Parameter:** `productId = 1`

**Expected Response:**
```json
{
  "productMaster": {
    "id": 1,
    "name": "T-Shirt Premium",
    "sku": "TSH-2024-001",
    "basePrice": 299.00
  },
  "variants": [
    {
      "id": 1,
      "sku": "TSH-2024-001-S-Red",
      "attributes": [
        { "attributeName": "Size", "value": "S" },
        { "attributeName": "Color", "value": "Red" }
      ],
      "price": 299.00,
      "stockQuantity": 100
    },
    {
      "id": 2,
      "sku": "TSH-2024-001-M-Red",
      "attributes": [
        { "attributeName": "Size", "value": "M" },
        { "attributeName": "Color", "value": "Red" }
      ],
      "price": 299.00,
      "stockQuantity": 80
    }
    // ... more variants
  ],
  "totalVariants": 16
}
```

---

## 8. Search Products

**Endpoint:** `GET /api/products/search?keyword=shirt&minPrice=200&maxPrice=500`

**Query Parameters:**
- `keyword`: "shirt"
- `minPrice`: 200
- `maxPrice`: 500

**Expected Response:**
```json
{
  "data": [
    {
      "id": 1,
      "name": "T-Shirt Premium",
      "sku": "TSH-2024-001",
      "basePrice": 299.00,
      "categoryName": "Apparel",
      "variantCount": 16
    }
  ],
  "totalCount": 1
}
```

---

## การใช้งานใน Swagger UI

### ขั้นตอนการ Demo:

1. **เปิด Swagger UI**: `https://localhost:7xxx/swagger`

2. **ทดสอบ GET Products** (ง่ายที่สุด):
   - คลิก `GET /api/products`
   - คลิก "Try it out"
   - คลิก "Execute"
   - ดูผลลัพธ์

3. **สร้าง ProductMaster**:
   - คลิก `POST /api/products`
   - คลิก "Try it out"
   - Copy JSON จากด้านบน
   - คลิก "Execute"
   - จดจำ `id` ที่ได้

4. **Generate Variants**:
   - คลิก `POST /api/products/{productMasterId}/variants/generate`
   - ใส่ `productMasterId` ที่ได้จากขั้นตอนที่ 3
   - Copy JSON payload
   - คลิก "Execute"
   - ดูว่าสร้าง 16 variants

5. **อัพเดท Stock**:
   - คลิก `PUT /api/products/variants/{variantId}/stock`
   - ใส่ `variantId = 1`
   - ใส่ `stockQuantity: 100`
   - Execute

6. **สร้าง Bundle**:
   - คลิก `POST /api/bundles`
   - Copy JSON payload
   - Execute

7. **ดู Bundle Stock**:
   - คลิก `GET /api/bundles/{bundleId}/stock`
   - ใส่ `bundleId = 1`
   - Execute
   - **ดูการคำนวณ Bottleneck!**

---

## Tips สำหรับการ Demo

✅ **ลำดับที่แนะนำ:**
1. GET Products (แสดงว่าระบบว่าง)
2. POST Create Product (สร้างสินค้า)
3. POST Generate Variants (Batch operation - 16 variants!)
4. PUT Update Stock (เพิ่มสต็อก)
5. GET Bundle Stock (แสดง Bottleneck calculation)

✅ **จุดที่ต้องเน้น:**
- Swagger UI มี Documentation ครบถ้วน
- API Response ชัดเจน
- Batch operation สร้าง 16 variants พร้อมกัน
- Bundle Stock Calculation แสดง Bottleneck

✅ **หากมีข้อผิดพลาด:**
- อธิบายว่า API มี Error Handling ที่ดี
- แสดง Error Message ที่ชัดเจน
- สามารถ Debug ได้ง่าย

---

## คำพูดสำหรับ Demo

"นี่คือ Swagger UI ครับ จะเห็นว่ามี API endpoints ทั้งหมดแสดงอย่างชัดเจน

ผมจะลอง Execute API จริงให้ดูนะครับ

**(คลิก POST /api/products)**

ตรงนี้ผมจะสร้าง ProductMaster ชื่อ 'T-Shirt Premium' ครับ

**(Paste JSON และ Execute)**

เห็นไหมครับ API ตอบกลับมาพร้อม id และข้อมูลครบถ้วน

**(คลิก POST Generate Variants)**

ตอนนี้ผมจะสร้าง 16 variants พร้อมกันด้วย Batch Operation ครับ 
มี 4 sizes และ 4 colors คูณกันได้ 16 variants

**(Execute)**

เห็นไหมครับ ระบบสร้าง 16 variants ได้ในคำขอเดียว และใช้เวลาเพียงไม่กี่วินาทีครับ"

---

พร้อมสำหรับ Demo แล้วครับ! 🚀
