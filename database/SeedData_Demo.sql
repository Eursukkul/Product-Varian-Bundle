-- DEMO Seed Data for FlowAccount
USE FlowAccountDb;
GO

PRINT 'Starting DEMO Seed Data...';

-- Categories
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
        N'เสอผา',
        1,
        GETUTCDATE ()
    ),
    (
        2,
        'Electronics',
        N'อเลกทรอนกส',
        1,
        GETUTCDATE ()
    );

SET IDENTITY_INSERT Categories OFF;

PRINT ' - Categories created';
GO

-- Warehouses
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
        N'กรงเทพ',
        1,
        GETUTCDATE ()
    );

SET IDENTITY_INSERT Warehouses OFF;

PRINT ' - Warehouse created';
GO

-- Product Master
SET IDENTITY_INSERT ProductMasters ON;

INSERT INTO
    ProductMasters (
        Id,
        Name,
        Description,
        CategoryId,
        IsActive,
        CreatedAt,
        UpdatedAt
    )
VALUES (
        1,
        'Basic T-Shirt',
        N'เสอยด',
        1,
        1,
        GETUTCDATE (),
        GETUTCDATE ()
    );

SET IDENTITY_INSERT ProductMasters OFF;

PRINT ' - Product created';
GO

-- Variant Options
SET IDENTITY_INSERT VariantOptions ON;

INSERT INTO
    VariantOptions (
        Id,
        ProductMasterId,
        Name,
        DisplayOrder
    )
VALUES (1, 1, N'ส', 1),
    (2, 1, N'ขนาด', 2);

SET IDENTITY_INSERT VariantOptions OFF;

PRINT ' - Variant Options created';
GO

-- Variant Values
SET IDENTITY_INSERT VariantOptionValues ON;

INSERT INTO
    VariantOptionValues (
        Id,
        VariantOptionId,
        Value,
        DisplayOrder
    )
VALUES (1, 1, N'แดง', 1),
    (2, 1, N'นำเงน', 2),
    (3, 2, 'M', 1),
    (4, 2, 'L', 2);

SET IDENTITY_INSERT VariantOptionValues OFF;

PRINT ' - Variant Values created';
GO

-- Product Variants
SET IDENTITY_INSERT ProductVariants ON;

INSERT INTO
    ProductVariants (
        Id,
        ProductMasterId,
        SKU,
        Price,
        Cost,
        IsActive,
        CreatedAt
    )
VALUES (
        1,
        1,
        'TSHIRT-RED-M',
        299.00,
        150.00,
        1,
        GETUTCDATE ()
    ),
    (
        2,
        1,
        'TSHIRT-RED-L',
        299.00,
        150.00,
        1,
        GETUTCDATE ()
    ),
    (
        3,
        1,
        'TSHIRT-BLUE-M',
        299.00,
        150.00,
        1,
        GETUTCDATE ()
    ),
    (
        4,
        1,
        'TSHIRT-BLUE-L',
        299.00,
        150.00,
        1,
        GETUTCDATE ()
    );

SET IDENTITY_INSERT ProductVariants OFF;

PRINT ' - Variants created';
GO

-- Product Variant Attributes
SET IDENTITY_INSERT ProductVariantAttributes ON;

INSERT INTO
    ProductVariantAttributes (
        Id,
        ProductVariantId,
        VariantOptionValueId
    )
VALUES (1, 1, 1),
    (2, 1, 3),
    (3, 2, 1),
    (4, 2, 4),
    (5, 3, 2),
    (6, 3, 3),
    (7, 4, 2),
    (8, 4, 4);

SET IDENTITY_INSERT ProductVariantAttributes OFF;

PRINT ' - Attributes mapped';
GO

-- Stocks
SET IDENTITY_INSERT Stocks ON;

INSERT INTO
    Stocks (
        Id,
        WarehouseId,
        ItemType,
        ItemId,
        Quantity,
        LastUpdated,
        ProductVariantId
    )
VALUES (
        1,
        1,
        'ProductVariant',
        1,
        50,
        GETUTCDATE (),
        1
    ),
    (
        2,
        1,
        'ProductVariant',
        2,
        30,
        GETUTCDATE (),
        2
    ),
    (
        3,
        1,
        'ProductVariant',
        3,
        40,
        GETUTCDATE (),
        3
    ),
    (
        4,
        1,
        'ProductVariant',
        4,
        20,
        GETUTCDATE (),
        4
    );

SET IDENTITY_INSERT Stocks OFF;

PRINT ' - Stocks created (140 units total)';

PRINT '';

PRINT '========== DEMO DATA READY ==========';

PRINT ' - 1 Product with 4 Variants';

PRINT ' - 140 units in stock';

PRINT ' - Ready for Demo!';
GO