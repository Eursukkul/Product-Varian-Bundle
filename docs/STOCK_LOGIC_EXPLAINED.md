# 📊 Stock Logic - Complete Explanation with Examples

## 📋 Overview

**Stock Logic** คือกลไกในการคำนวณว่า Bundle (ชุดสินค้า) สามารถขายได้สูงสุดกี่ชุด โดยพิจารณาจากสต็อกของสินค้าองค์ประกอบทั้งหมด ภายในคลังสินค้าที่ระบุ

---

## 🎯 Requirement from Assignment

> **โจทย์ข้อสอบ:** เขียนสูตร/คำอธิบายสั้น ๆ ว่า "จำนวนขายได้สูงสุด = ค่าต่ำสุดของ (สต็อกจริง/ปริมาณที่ต้องใช้) ต่อสินค้าองค์ประกอบ ภายในคลังเดียวกัน" พร้อมตัวอย่างตัวเลข 1 ชุดเพื่อยืนยันความเข้าใจตรงกับโจทย์

---

## 📐 Formula - สูตรคำนวณ

### **สูตรหลัก (Mathematical Formula)**

```
จำนวน Bundle ที่ขายได้สูงสุด = MIN(สต็อกสินค้าแต่ละตัว / ปริมาณที่ต้องใช้ต่อ Bundle)

หรือเขียนเป็นสูตรคณิตศาสตร์:

MaxBundles = MIN(Q₁/R₁, Q₂/R₂, Q₃/R₃, ..., Qₙ/Rₙ)

โดยที่:
- Qᵢ = จำนวนสต็อกจริงของสินค้าชิ้นที่ i (Available Quantity)
- Rᵢ = จำนวนที่ต้องใช้ต่อ 1 Bundle ของสินค้าชิ้นที่ i (Required Quantity)
- n = จำนวนสินค้าทั้งหมดใน Bundle
```

### **Algorithm (Step-by-Step)**

```
1. สำหรับแต่ละสินค้าใน Bundle:
   a. ดึงสต็อกปัจจุบันในคลังที่ระบุ (Qᵢ)
   b. ดึงจำนวนที่ต้องใช้ต่อ Bundle (Rᵢ)
   c. คำนวณ: จำนวน Bundle ที่ทำได้จากสินค้านี้ = Qᵢ ÷ Rᵢ (ปัดทศนิยมทิ้ง)

2. หาค่าต่ำสุด (MIN) จากทุกสินค้า
   → นี่คือจำนวน Bundle สูงสุดที่ขายได้

3. สินค้าที่มีค่าเท่ากับ MIN = "Bottleneck" (คอขวด)
   → สินค้านี้จำกัดการผลิต Bundle
```

---

## 💡 Example 1: Basic Calculation

### **Scenario: Gaming PC Bundle**

**Bundle Configuration:**

| สินค้า | ปริมาณที่ต้องใช้ (Rᵢ) | หน่วย |
|--------|---------------------|-------|
| CPU | 1 | ชิ้น |
| RAM (16GB) | 2 | แท่ง |
| SSD (1TB) | 1 | ชิ้น |
| Graphics Card | 1 | ชิ้น |

**Current Stock (Warehouse ID: 1):**

| สินค้า | สต็อกปัจจุบัน (Qᵢ) |
|--------|------------------|
| CPU | 50 |
| RAM (16GB) | 150 |
| SSD (1TB) | 30 |
| Graphics Card | 100 |

### **Calculation:**

```
1. CPU:           50 ÷ 1 = 50 bundles
2. RAM:          150 ÷ 2 = 75 bundles
3. SSD:           30 ÷ 1 = 30 bundles  ⚠️ BOTTLENECK
4. Graphics Card: 100 ÷ 1 = 100 bundles

MaxBundles = MIN(50, 75, 30, 100) = 30 bundles
```

### **Result:**

- ✅ **จำนวนขายได้สูงสุด: 30 Bundle**
- ⚠️ **Bottleneck: SSD (1TB)** - มีเพียง 30 ชิ้น จึงจำกัดการขายไว้ที่ 30 Bundle
- 💡 **คำอธิบาย:** แม้ว่า CPU จะมี 50 ชิ้น และ Graphics Card มี 100 ชิ้น แต่ SSD มีเพียง 30 ชิ้น จึงทำ Bundle ได้เพียง 30 ชุดเท่านั้น

### **JSON Response Example:**

```json
{
  "success": true,
  "message": "Bundle can produce 30 units",
  "data": {
    "bundleId": 1,
    "bundleName": "Gaming PC Bundle",
    "warehouseId": 1,
    "warehouseName": "Warehouse Bangkok",
    "maxAvailableBundles": 30,
    "itemsStockBreakdown": [
      {
        "itemName": "CPU",
        "itemSku": "CPU-INTEL-I9",
        "requiredQuantity": 1,
        "availableQuantity": 50,
        "possibleBundles": 50,
        "isBottleneck": false
      },
      {
        "itemName": "RAM (16GB)",
        "itemSku": "RAM-16GB-DDR4",
        "requiredQuantity": 2,
        "availableQuantity": 150,
        "possibleBundles": 75,
        "isBottleneck": false
      },
      {
        "itemName": "SSD (1TB)",
        "itemSku": "SSD-1TB-NVME",
        "requiredQuantity": 1,
        "availableQuantity": 30,
        "possibleBundles": 30,
        "isBottleneck": true   // ⚠️ This limits production
      },
      {
        "itemName": "Graphics Card",
        "itemSku": "GPU-RTX-4090",
        "requiredQuantity": 1,
        "availableQuantity": 100,
        "possibleBundles": 100,
        "isBottleneck": false
      }
    ],
    "explanation": "Bundle availability limited by SSD (1TB) - only 30 units available"
  }
}
```

---

## 💡 Example 2: T-Shirt Bundle with Different Quantities

### **Scenario: Premium Clothing Bundle**

**Bundle Configuration:**

| สินค้า | ปริมาณที่ต้องใช้ (Rᵢ) | หมายเหตุ |
|--------|---------------------|---------|
| T-Shirt (Size M, Red) | 1 | Variant ID: 101 |
| T-Shirt (Size L, Blue) | 1 | Variant ID: 102 |
| Shorts (Size M) | 2 | ต้องการ 2 ตัวต่อ Bundle |
| Cap | 1 | Product ID: 10 |

**Current Stock (Warehouse ID: 1):**

| สินค้า | สต็อกปัจจุบัน (Qᵢ) |
|--------|------------------|
| T-Shirt (M, Red) | 100 |
| T-Shirt (L, Blue) | 45 |
| Shorts (M) | 120 |
| Cap | 200 |

### **Calculation:**

```
1. T-Shirt (M, Red):  100 ÷ 1 = 100 bundles
2. T-Shirt (L, Blue):  45 ÷ 1 = 45 bundles  ⚠️ BOTTLENECK
3. Shorts (M):        120 ÷ 2 = 60 bundles
4. Cap:               200 ÷ 1 = 200 bundles

MaxBundles = MIN(100, 45, 60, 200) = 45 bundles
```

### **Result:**

- ✅ **จำนวนขายได้สูงสุด: 45 Bundle**
- ⚠️ **Bottleneck: T-Shirt (L, Blue)** - มีเพียง 45 ตัว
- 💡 **สังเกต:** Shorts ต้องใช้ 2 ตัวต่อ Bundle แต่ก็ยังไม่ใช่คอขวด เพราะมี 120 ตัว (ทำได้ 60 Bundle)

### **After Selling 10 Bundles:**

**Remaining Stock:**

| สินค้า | สต็อกก่อนขาย | ใช้ไป | สต็อกหลังขาย | Bundle ที่ทำได้ |
|--------|------------|------|-------------|--------------|
| T-Shirt (M, Red) | 100 | 10 | 90 | 90 |
| T-Shirt (L, Blue) | 45 | 10 | **35** | **35** ⚠️ |
| Shorts (M) | 120 | 20 | 100 | 50 |
| Cap | 200 | 10 | 190 | 190 |

**New MaxBundles = MIN(90, 35, 50, 190) = 35 bundles**

---

## 💡 Example 3: Zero Stock Scenario

### **Scenario: Out of Stock Item**

**Bundle Configuration:**

| สินค้า | Required | Available | Possible Bundles |
|--------|----------|-----------|------------------|
| Item A | 1 | 50 | 50 |
| Item B | 2 | **0** | **0** ⚠️ |
| Item C | 1 | 100 | 100 |

### **Calculation:**

```
MaxBundles = MIN(50, 0, 100) = 0 bundles
```

### **Result:**

- ❌ **จำนวนขายได้สูงสุด: 0 Bundle**
- ⚠️ **Bottleneck: Item B** - สต็อกหมด (Out of Stock)
- 💡 **ไม่สามารถขาย Bundle ได้เลย** แม้ว่าสินค้าอื่นจะมีเหลือ

---

## 💡 Example 4: High Required Quantity

### **Scenario: Promotional Bundle (Buy 10 Get Discount)**

**Bundle Configuration:**

| สินค้า | Required (Rᵢ) | Available (Qᵢ) | Calculation |
|--------|--------------|----------------|-------------|
| Pen | 10 | 250 | 250 ÷ 10 = **25** ⚠️ |
| Notebook | 5 | 200 | 200 ÷ 5 = 40 |
| Eraser | 3 | 150 | 150 ÷ 3 = 50 |

### **Calculation:**

```
MaxBundles = MIN(25, 40, 50) = 25 bundles
```

### **Result:**

- ✅ **จำนวนขายได้สูงสุด: 25 Bundle**
- ⚠️ **Bottleneck: Pen** - ต้องการ 10 ด้ามต่อ Bundle
- 💡 **การขาย 1 Bundle จะใช้:**
  - Pen: 10 ด้าม
  - Notebook: 5 เล่ม
  - Eraser: 3 ก้อน

---

## 🔍 Bottleneck Detection Algorithm

### **การระบุ "คอขวด" (Bottleneck)**

```csharp
public class StockCalculationService
{
    public BundleStockDto CalculateBundleStock(int bundleId, int warehouseId)
    {
        var bundle = GetBundleWithItems(bundleId);
        var stockBreakdown = new List<ItemStockBreakdown>();
        
        int minPossibleBundles = int.MaxValue;
        
        // Step 1: Calculate possible bundles for each item
        foreach (var item in bundle.Items)
        {
            var stock = GetStock(warehouseId, item.ItemType, item.ItemId);
            var possibleBundles = stock.Quantity / item.RequiredQuantity;
            
            stockBreakdown.Add(new ItemStockBreakdown
            {
                ItemName = GetItemName(item),
                RequiredQuantity = item.RequiredQuantity,
                AvailableQuantity = stock.Quantity,
                PossibleBundles = possibleBundles
            });
            
            // Track minimum
            if (possibleBundles < minPossibleBundles)
                minPossibleBundles = possibleBundles;
        }
        
        // Step 2: Mark bottlenecks (items with possibleBundles == min)
        foreach (var item in stockBreakdown)
        {
            item.IsBottleneck = (item.PossibleBundles == minPossibleBundles);
        }
        
        return new BundleStockDto
        {
            MaxAvailableBundles = minPossibleBundles,
            ItemsStockBreakdown = stockBreakdown
        };
    }
}
```

---

## 📊 SQL Query Implementation

### **Optimized SQL for Stock Calculation**

```sql
-- Calculate maximum available bundles for a specific bundle and warehouse
WITH BundleStockCalculation AS (
    SELECT 
        bi.BundleId,
        bi.ItemType,
        bi.ItemId,
        bi.Quantity AS RequiredQuantity,
        s.Quantity AS AvailableQuantity,
        s.WarehouseId,
        FLOOR(s.Quantity / bi.Quantity) AS PossibleBundles,
        CASE 
            WHEN bi.ItemType = 'Variant' THEN pv.SKU
            WHEN bi.ItemType = 'Product' THEN pm.Name
        END AS ItemName
    FROM BundleItems bi
    INNER JOIN Stocks s 
        ON bi.ItemType = s.ItemType 
        AND bi.ItemId = s.ItemId
    LEFT JOIN ProductVariants pv 
        ON bi.ItemType = 'Variant' 
        AND bi.ItemId = pv.Id
    LEFT JOIN ProductMasters pm 
        ON bi.ItemType = 'Product' 
        AND bi.ItemId = pm.Id
    WHERE bi.BundleId = @BundleId
        AND s.WarehouseId = @WarehouseId
),
MinBundles AS (
    SELECT 
        BundleId,
        WarehouseId,
        MIN(PossibleBundles) AS MaxAvailableBundles
    FROM BundleStockCalculation
    GROUP BY BundleId, WarehouseId
)
SELECT 
    bsc.*,
    mb.MaxAvailableBundles,
    CASE 
        WHEN bsc.PossibleBundles = mb.MaxAvailableBundles 
        THEN 1 
        ELSE 0 
    END AS IsBottleneck
FROM BundleStockCalculation bsc
CROSS JOIN MinBundles mb
ORDER BY IsBottleneck DESC, bsc.ItemName;
```

---

## 🧪 Test Cases

### **Test Case 1: Normal Scenario**

```csharp
[Fact]
public async Task CalculateBundleStock_WithSufficientStock_ReturnsCorrectMaxBundles()
{
    // Arrange
    var bundleId = 1;
    var warehouseId = 1;
    
    // Bundle items:
    // - Item A: requires 1, available 50 → 50 bundles
    // - Item B: requires 2, available 80 → 40 bundles (bottleneck)
    // - Item C: requires 1, available 100 → 100 bundles
    
    // Act
    var result = await _service.CalculateBundleStockAsync(bundleId, warehouseId);
    
    // Assert
    Assert.Equal(40, result.MaxAvailableBundles);
    Assert.True(result.ItemsStockBreakdown
        .Single(x => x.ItemName == "Item B")
        .IsBottleneck);
}
```

### **Test Case 2: Out of Stock**

```csharp
[Fact]
public async Task CalculateBundleStock_WithZeroStock_ReturnsZeroBundles()
{
    // Arrange
    var bundleId = 2;
    var warehouseId = 1;
    
    // Bundle items:
    // - Item A: requires 1, available 50 → 50 bundles
    // - Item B: requires 1, available 0 → 0 bundles (bottleneck)
    
    // Act
    var result = await _service.CalculateBundleStockAsync(bundleId, warehouseId);
    
    // Assert
    Assert.Equal(0, result.MaxAvailableBundles);
}
```

---

## 🎯 Summary

### **Key Points:**

1. **สูตรหลัก:** 
   ```
   MaxBundles = MIN(Q₁/R₁, Q₂/R₂, ..., Qₙ/Rₙ)
   ```

2. **Bottleneck:** สินค้าที่มีค่า `Qᵢ/Rᵢ` ต่ำที่สุด

3. **Constraint:** ต้องคำนวณภายในคลังเดียวกัน (Same Warehouse)

4. **Algorithm:** O(n) time complexity where n = number of items in bundle

5. **Business Rule:** 
   - ไม่สามารถขายเกิน MaxBundles
   - ต้อง restock bottleneck item เพื่อเพิ่มจำนวนขาย

### **Visual Formula:**

```
Bundle Stock = MIN of all (Available ÷ Required)
               ↓
         ┌─────┴─────┐
         │           │
    Item A: 50/1=50  Item B: 80/2=40  Item C: 100/1=100
                          ↓
                     🎯 Bottleneck
                     (Limits to 40)
```

---

**Last Updated:** October 17, 2025  
**Status:** ✅ Production Ready  
**Version:** 1.0
