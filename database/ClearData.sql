-- ======================================
-- Clear All Data from FlowAccount Database
-- ใช้สำหรับลบข้อมูลทดสอบทั้งหมด
-- ======================================

USE FlowAccountDb;
GO

PRINT '🗑️ Starting data cleanup...';
GO

-- ปิด Foreign Key Constraints ชั่วคระว
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all";
GO

-- ======================================
-- ลบข้อมูลตามลำดับ (เพื่อหลีกเลี่ยง FK Constraints)
-- ======================================

-- 1. ลบข้อมูล Stocks (ไม่มี FK ไปที่ตารางอื่น)
PRINT '🔹 Deleting Stocks...';

DELETE FROM Stocks;

PRINT '✅ Stocks cleared';
GO

-- 2. ลบข้อมูล BundleItems (FK -> Bundles, ProductVariants)
PRINT '🔹 Deleting BundleItems...';

DELETE FROM BundleItems;

PRINT '✅ BundleItems cleared';
GO

-- 3. ลบข้อมูล Bundles
PRINT '🔹 Deleting Bundles...';

DELETE FROM Bundles;

PRINT '✅ Bundles cleared';
GO

-- 4. ลบข้อมูล ProductVariantAttributes (FK -> ProductVariants)
PRINT '🔹 Deleting ProductVariantAttributes...';

DELETE FROM ProductVariantAttributes;

PRINT '✅ ProductVariantAttributes cleared';
GO

-- 5. ลบข้อมูล ProductVariants (FK -> ProductMasters)
PRINT '🔹 Deleting ProductVariants...';

DELETE FROM ProductVariants;

PRINT '✅ ProductVariants cleared';
GO

-- 6. ลบข้อมูล ProductMasters
PRINT '🔹 Deleting ProductMasters...';

DELETE FROM ProductMasters;

PRINT '✅ ProductMasters cleared';
GO

-- 7. ลบข้อมูล VariantOptionValues (FK -> VariantOptions)
PRINT '🔹 Deleting VariantOptionValues...';

DELETE FROM VariantOptionValues;

PRINT '✅ VariantOptionValues cleared';
GO

-- 8. ลบข้อมูล VariantOptions
PRINT '🔹 Deleting VariantOptions...';

DELETE FROM VariantOptions;

PRINT '✅ VariantOptions cleared';
GO

-- 9. ลบข้อมูล Categories
PRINT '🔹 Deleting Categories...';

DELETE FROM Categories;

PRINT '✅ Categories cleared';
GO

-- 10. ลบข้อมูล Warehouses
PRINT '🔹 Deleting Warehouses...';

DELETE FROM Warehouses;

PRINT '✅ Warehouses cleared';
GO

-- เปิด Foreign Key Constraints กลับมา
EXEC sp_MSforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all";
GO

-- ======================================
-- Reset Identity Columns (เริ่มนับใหม่จาก 1)
-- ======================================
PRINT '🔄 Resetting Identity columns...';

DBCC CHECKIDENT ('Categories', RESEED, 0);

DBCC CHECKIDENT ('VariantOptions', RESEED, 0);

DBCC CHECKIDENT ( 'VariantOptionValues', RESEED, 0 );

DBCC CHECKIDENT ('ProductMasters', RESEED, 0);

DBCC CHECKIDENT ('ProductVariants', RESEED, 0);

DBCC CHECKIDENT ( 'ProductVariantAttributes', RESEED, 0 );

DBCC CHECKIDENT ('Bundles', RESEED, 0);

DBCC CHECKIDENT ('BundleItems', RESEED, 0);

DBCC CHECKIDENT ('Warehouses', RESEED, 0);

DBCC CHECKIDENT ('Stocks', RESEED, 0);
GO

PRINT '✅ Identity columns reset';

PRINT '🎉 Data cleanup completed successfully!';
GO

-- ======================================
-- ตรวจสอบจำนวนข้อมูลที่เหลือ
-- ======================================
PRINT '📊 Verifying data counts...';
GO

SELECT 'Categories' AS TableName, COUNT(*) AS RecordCount
FROM Categories
UNION ALL
SELECT 'VariantOptions', COUNT(*)
FROM VariantOptions
UNION ALL
SELECT 'VariantOptionValues', COUNT(*)
FROM VariantOptionValues
UNION ALL
SELECT 'ProductMasters', COUNT(*)
FROM ProductMasters
UNION ALL
SELECT 'ProductVariants', COUNT(*)
FROM ProductVariants
UNION ALL
SELECT 'ProductVariantAttributes', COUNT(*)
FROM ProductVariantAttributes
UNION ALL
SELECT 'Bundles', COUNT(*)
FROM Bundles
UNION ALL
SELECT 'BundleItems', COUNT(*)
FROM BundleItems
UNION ALL
SELECT 'Warehouses', COUNT(*)
FROM Warehouses
UNION ALL
SELECT 'Stocks', COUNT(*)
FROM Stocks;
GO