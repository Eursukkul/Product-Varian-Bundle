-- ======================================
-- Seed Data for FlowAccount Database
-- ======================================

USE FlowAccountDb;
GO

-- ======================================
-- 1. Insert Categories
-- ======================================
SET IDENTITY_INSERT Categories ON;

IF NOT EXISTS (
    SELECT 1
    FROM Categories
    WHERE
        Id = 1
) BEGIN
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
        'Apparel',
        'เสื้อผ้าและเครื่องแต่งกาย',
        1,
        GETUTCDATE ()
    );

PRINT 'Category 1: Apparel created';

END ELSE BEGIN PRINT 'Category 1 already exists';

END IF NOT EXISTS (
    SELECT 1
    FROM Categories
    WHERE
        Id = 2
) BEGIN
INSERT INTO
    Categories (
        Id,
        Name,
        Description,
        IsActive,
        CreatedAt
    )
VALUES (
        2,
        'Accessories',
        'เครื่องประดับและอุปกรณ์เสริม',
        1,
        GETUTCDATE ()
    );

PRINT 'Category 2: Accessories created';

END IF NOT EXISTS (
    SELECT 1
    FROM Categories
    WHERE
        Id = 3
) BEGIN
INSERT INTO
    Categories (
        Id,
        Name,
        Description,
        IsActive,
        CreatedAt
    )
VALUES (
        3,
        'Electronics',
        'อุปกรณ์อิเล็กทรอนิกส์',
        1,
        GETUTCDATE ()
    );

PRINT 'Category 3: Electronics created';

END

SET IDENTITY_INSERT Categories OFF;
GO

-- ======================================
-- 2. Insert Warehouses
-- ======================================
SET IDENTITY_INSERT Warehouses ON;

IF NOT EXISTS (
    SELECT 1
    FROM Warehouses
    WHERE
        Id = 1
) BEGIN
INSERT INTO
    Warehouses (
        Id,
        Name,
        Description,
        IsActive,
        CreatedAt
    )
VALUES (
        1,
        'Main Warehouse',
        'คลังสินค้าหลัก กรุงเทพฯ',
        1,
        GETUTCDATE ()
    );

PRINT 'Warehouse 1: Main Warehouse created';

END ELSE BEGIN PRINT 'Warehouse 1 already exists';

END IF NOT EXISTS (
    SELECT 1
    FROM Warehouses
    WHERE
        Id = 2
) BEGIN
INSERT INTO
    Warehouses (
        Id,
        Name,
        Description,
        IsActive,
        CreatedAt
    )
VALUES (
        2,
        'Secondary Warehouse',
        'คลังสินค้าสำรอง เชียงใหม่',
        1,
        GETUTCDATE ()
    );

PRINT 'Warehouse 2: Secondary Warehouse created';

END

SET IDENTITY_INSERT Warehouses OFF;
GO

-- ======================================
-- 3. Verify Data
-- ======================================
PRINT '';

PRINT '====== Verification ======';

PRINT 'Categories:';

SELECT Id, Name, IsActive FROM Categories;

PRINT '';

PRINT 'Warehouses:';

SELECT Id, Name, IsActive FROM Warehouses;

PRINT '';

PRINT '====== Seed Data Complete ======';
GO