-- ======================================
-- 🎬 DEMO Seed Data for FlowAccount
-- เหมาะสำหรับการ Demo และอัดวีดีโอ
-- ======================================

USE FlowAccountDb;
GO

PRINT '🎬 Starting DEMO Seed Data Installation...';

PRINT '';

-- ======================================
-- 1. Categories (หมวดหมู่สินค้า)
-- ======================================
PRINT '📁 Creating Categories...';

SET IDENTITY_INSERT Categories ON;

INSERT INTO
    Categories (
        Id,
        Name,
        Description,
        IsActive,
        CreatedAt
    )
VALUES (
        1,
        'Clothing',
        'เสื้อผ้า',
        1,
        GETUTCDATE ()
    ),
    (
        2,
        'Electronics',
        'อิเล็กทรอนิกส์',
        1,
        GETUTCDATE ()
    );

SET IDENTITY_INSERT Categories OFF;

PRINT ' ✅ 2 Categories created';
GO

-- ======================================
-- 2. Warehouses (คลังสินค้า)
-- ======================================
PRINT '📦 Creating Warehouses...';

SET IDENTITY_INSERT Warehouses ON;

INSERT INTO
    Warehouses (
        Id,
        Name,
        Location,
        IsActive,
        CreatedAt
    )
VALUES (
        1,
        'Main Warehouse',
        'กรุงเทพฯ',
        1,
        GETUTCDATE ()
    );

SET IDENTITY_INSERT Warehouses OFF;

PRINT ' ✅ 1 Warehouse created';
GO

-- ======================================
-- 3. Variant Options (ตัวเลือก Variant)
-- ======================================
PRINT '🎨 Creating Variant Options...';

SET IDENTITY_INSERT VariantOptions ON;

-- สี (Color)
INSERT INTO
    VariantOptions (
        Id,
        Name,
        DisplayName,
        CreatedAt
    )
VALUES (
        1,
        'color',
        'สี',
        GETUTCDATE ()
    );

-- ขนาด (Size)
INSERT INTO
    VariantOptions (
        Id,
        Name,
        DisplayName,
        CreatedAt
    )
VALUES (
        2,
        'size',
        'ขนาด',
        GETUTCDATE ()
    );

SET IDENTITY_INSERT VariantOptions OFF;

PRINT ' ✅ 2 Variant Options created';
GO

-- ======================================
-- 4. Variant Option Values (ค่า Variant)
-- ======================================
PRINT '🏷️ Creating Variant Values...';

SET IDENTITY_INSERT VariantOptionValues ON;

-- ค่าสี
INSERT INTO
    VariantOptionValues (
        Id,
        VariantOptionId,
        Value,
        DisplayValue,
        CreatedAt
    )
VALUES (
        1,
        1,
        'red',
        'แดง',
        GETUTCDATE ()
    ),
    (
        2,
        1,
        'blue',
        'น้ำเงิน',
        GETUTCDATE ()
    ),
    (
        3,
        1,
        'black',
        'ดำ',
        GETUTCDATE ()
    ),
    (
        4,
        1,
        'white',
        'ขาว',
        GETUTCDATE ()
    );

-- ค่าขนาด
INSERT INTO
    VariantOptionValues (
        Id,
        VariantOptionId,
        Value,
        DisplayValue,
        CreatedAt
    )
VALUES (5, 2, 'S', 'S', GETUTCDATE ()),
    (6, 2, 'M', 'M', GETUTCDATE ()),
    (7, 2, 'L', 'L', GETUTCDATE ()),
    (
        8,
        2,
        'XL',
        'XL',
        GETUTCDATE ()
    );

SET IDENTITY_INSERT VariantOptionValues OFF;

PRINT ' ✅ 8 Variant Values created';
GO

-- ======================================
-- 5. Product Master - เสื้อยืด (T-Shirt)
-- ======================================
PRINT '👕 Creating Product: T-Shirt...';

SET IDENTITY_INSERT ProductMasters ON;

INSERT INTO
    ProductMasters (
        Id,
        Name,
        Description,
        CategoryId,
        BasePrice,
        IsActive,
        CreatedAt
    )
VALUES (
        1,
        'Basic T-Shirt',
        'เสื้อยืดคอกลม ผ้าคอตตอน 100%',
        1, -- Clothing
        299.00,
        1,
        GETUTCDATE ()
    );

SET IDENTITY_INSERT ProductMasters OFF;

PRINT ' ✅ T-Shirt Master created';
GO

-- ======================================
-- 6. Product Variants - เสื้อยืด (4 SKUs)
-- ======================================
PRINT '🏷️ Creating T-Shirt Variants (4 SKUs)...';

SET IDENTITY_INSERT ProductVariants ON;

-- แดง-M
INSERT INTO
    ProductVariants (
        Id,
        ProductMasterId,
        SKU,
        Barcode,
        Price,
        StockQuantity,
        IsActive,
        CreatedAt
    )
VALUES (
        1,
        1,
        'TSHIRT-RED-M',
        'BAR001',
        299.00,
        0,
        1,
        GETUTCDATE ()
    );

-- แดง-L
INSERT INTO
    ProductVariants (
        Id,
        ProductMasterId,
        SKU,
        Barcode,
        Price,
        StockQuantity,
        IsActive,
        CreatedAt
    )
VALUES (
        2,
        1,
        'TSHIRT-RED-L',
        'BAR002',
        299.00,
        0,
        1,
        GETUTCDATE ()
    );

-- น้ำเงิน-M
INSERT INTO
    ProductVariants (
        Id,
        ProductMasterId,
        SKU,
        Barcode,
        Price,
        StockQuantity,
        IsActive,
        CreatedAt
    )
VALUES (
        3,
        1,
        'TSHIRT-BLUE-M',
        'BAR003',
        299.00,
        0,
        1,
        GETUTCDATE ()
    );

-- น้ำเงิน-L
INSERT INTO
    ProductVariants (
        Id,
        ProductMasterId,
        SKU,
        Barcode,
        Price,
        StockQuantity,
        IsActive,
        CreatedAt
    )
VALUES (
        4,
        1,
        'TSHIRT-BLUE-L',
        'BAR004',
        299.00,
        0,
        1,
        GETUTCDATE ()
    );

SET IDENTITY_INSERT ProductVariants OFF;

PRINT ' ✅ 4 T-Shirt Variants created';
GO

-- ======================================
-- 7. Product Variant Attributes
-- ======================================
PRINT '🎨 Mapping Variant Attributes...';

SET IDENTITY_INSERT ProductVariantAttributes ON;

-- Variant 1: แดง-M
INSERT INTO
    ProductVariantAttributes (
        Id,
        ProductVariantId,
        VariantOptionId,
        VariantOptionValueId,
        CreatedAt
    )
VALUES (1, 1, 1, 1, GETUTCDATE ()), -- สี = แดง
    (2, 1, 2, 6, GETUTCDATE ());
-- ขนาด = M

-- Variant 2: แดง-L
INSERT INTO
    ProductVariantAttributes (
        Id,
        ProductVariantId,
        VariantOptionId,
        VariantOptionValueId,
        CreatedAt
    )
VALUES (3, 2, 1, 1, GETUTCDATE ()), -- สี = แดง
    (4, 2, 2, 7, GETUTCDATE ());
-- ขนาด = L

-- Variant 3: น้ำเงิน-M
INSERT INTO
    ProductVariantAttributes (
        Id,
        ProductVariantId,
        VariantOptionId,
        VariantOptionValueId,
        CreatedAt
    )
VALUES (5, 3, 1, 2, GETUTCDATE ()), -- สี = น้ำเงิน
    (6, 3, 2, 6, GETUTCDATE ());
-- ขนาด = M

-- Variant 4: น้ำเงิน-L
INSERT INTO
    ProductVariantAttributes (
        Id,
        ProductVariantId,
        VariantOptionId,
        VariantOptionValueId,
        CreatedAt
    )
VALUES (7, 4, 1, 2, GETUTCDATE ()), -- สี = น้ำเงิน
    (8, 4, 2, 7, GETUTCDATE ());
-- ขนาด = L

SET IDENTITY_INSERT ProductVariantAttributes OFF;

PRINT ' ✅ 8 Attributes mapped';
GO

-- ======================================
-- 8. Stocks (สต็อกสินค้า)
-- ======================================
PRINT '📊 Creating Stock Records...';

SET IDENTITY_INSERT Stocks ON;

INSERT INTO
    Stocks (
        Id,
        ProductVariantId,
        WarehouseId,
        Quantity,
        ReservedQuantity,
        CreatedAt,
        UpdatedAt
    )
VALUES (
        1,
        1,
        1,
        50,
        0,
        GETUTCDATE (),
        GETUTCDATE ()
    ), -- แดง-M: 50 ชิ้น
    (
        2,
        2,
        1,
        30,
        0,
        GETUTCDATE (),
        GETUTCDATE ()
    ), -- แดง-L: 30 ชิ้น
    (
        3,
        3,
        1,
        40,
        0,
        GETUTCDATE (),
        GETUTCDATE ()
    ), -- น้ำเงิน-M: 40 ชิ้น
    (
        4,
        4,
        1,
        20,
        0,
        GETUTCDATE (),
        GETUTCDATE ()
    );
-- น้ำเงิน-L: 20 ชิ้น

SET IDENTITY_INSERT Stocks OFF;

PRINT ' ✅ 4 Stock records created';
GO

-- ======================================
-- 9. Verification (ตรวจสอบข้อมูล)
-- ======================================
PRINT '';

PRINT '📊 ========== DATA SUMMARY ==========';

PRINT '';

SELECT 
    'Categories' AS [Table],
    COUNT(*) AS [Records],
    STRING_AGG(Name, ', ') AS [Data]
FROM Categories;

SELECT 
    'Warehouses' AS [Table],
    COUNT(*) AS [Records],
    STRING_AGG(Name, ', ') AS [Data]
FROM Warehouses;

SELECT 
    'Variant Options' AS [Table],
    COUNT(*) AS [Records],
    STRING_AGG(DisplayName, ', ') AS [Data]
FROM VariantOptions;

SELECT 
    'Variant Values' AS [Table],
    COUNT(*) AS [Records],
    CONCAT(COUNT(*), ' values') AS [Data]
FROM VariantOptionValues;

SELECT 
    'Products' AS [Table],
    COUNT(*) AS [Records],
    STRING_AGG(Name, ', ') AS [Data]
FROM ProductMasters;

SELECT 
    'Product Variants' AS [Table],
    COUNT(*) AS [Records],
    STRING_AGG(SKU, ', ') AS [Data]
FROM ProductVariants;

SELECT 
    'Stock Records' AS [Table],
    COUNT(*) AS [Records],
    CONCAT('Total: ', SUM(Quantity), ' units') AS [Data]
FROM Stocks;

PRINT '';

PRINT '✅ ========== DEMO SEED DATA COMPLETE ==========';

PRINT '';

PRINT '🎬 Ready for Demo!';

PRINT '📝 You now have:';

PRINT ' - 1 Product (T-Shirt) with 4 Variants';

PRINT ' - 2 Colors x 2 Sizes = 4 SKUs';

PRINT ' - Stock: 140 units total';

PRINT '';

PRINT '🚀 Next: Start API and open Swagger';

PRINT ' URL: http://localhost:5000/swagger';
GO