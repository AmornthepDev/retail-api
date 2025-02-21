using Dapper;

namespace Retail.Application.Database
{
    public class DbInitializer
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DbInitializer(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task InitializeAsync(CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

            var sql = """
                -- Create Tables
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Products') AND type = N'U')
                BEGIN
                    CREATE TABLE Products (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        Sku VARCHAR(20) NOT NULL UNIQUE,
                        Name NVARCHAR(255) NOT NULL,
                        Description NVARCHAR(MAX) NULL
                    );
                END;

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ProductVariants') AND type = N'U')
                BEGIN
                    CREATE TABLE ProductVariants (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        ProductId INT NOT NULL,
                        Size DECIMAL(10, 2) NOT NULL CHECK (Size > 0),
                        Unit NVARCHAR(10) NOT NULL,
                        Price DECIMAL(18, 2) NOT NULL CHECK (Price > 0),
                        Quantity INT NOT NULL CHECK (Quantity > 0),
                        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
                    );
                END;

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Categories') AND type = N'U')
                BEGIN
                    CREATE TABLE Categories (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        Name NVARCHAR(255) NOT NULL UNIQUE
                    );
                END;

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ProductCategories') AND type = N'U')
                BEGIN
                    CREATE TABLE ProductCategories (
                        CategoryId INT NOT NULL,
                        ProductId INT NOT NULL,
                        PRIMARY KEY (CategoryId, ProductId),
                        FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE,
                        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
                    );
                END;

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'SubCategories') AND type = N'U')
                BEGIN
                    CREATE TABLE SubCategories (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        Name NVARCHAR(255) NOT NULL UNIQUE,
                        CategoryId INT NOT NULL,
                        FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
                    );
                END;

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ProductSubCategories') AND type = N'U')
                BEGIN
                    CREATE TABLE ProductSubCategories (
                        ProductId INT NOT NULL,
                        SubCategoryId INT NOT NULL,
                        PRIMARY KEY (ProductId, SubCategoryId),
                        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
                        FOREIGN KEY (SubCategoryId) REFERENCES SubCategories(Id) ON DELETE CASCADE
                    );
                END;

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Orders') AND type = N'U')
                BEGIN
                    CREATE TABLE Orders (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        OrderNumber VARCHAR(50) NOT NULL UNIQUE,
                        OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
                        CustomerId INT NULL,
                        TotalAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
                        DiscountAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
                        NetAmount AS (TotalAmount - DiscountAmount) PERSISTED,
                        Status TINYINT NOT NULL DEFAULT 0
                    );
                END;

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'OrderItems') AND type = N'U')
                BEGIN
                    CREATE TABLE OrderItems (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        OrderId INT NOT NULL,
                        ProductVariantId INT NOT NULL,
                        Quantity INT NOT NULL CHECK (Quantity > 0),
                        UnitPrice DECIMAL(18, 2) NOT NULL CHECK (UnitPrice > 0), -- snap shot of unit price
                        DiscountAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
                        FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
                        FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id) ON DELETE CASCADE
                    );
                END;

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Discounts') AND type = N'U')
                BEGIN
                    CREATE TABLE Discounts (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        Name NVARCHAR(255) NOT NULL,
                        Description NVARCHAR(MAX) NULL,
                        IsActive BIT NOT NULL DEFAULT 1
                    );
                END;

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'OrderDiscounts') AND type = N'U')
                BEGIN
                    CREATE TABLE OrderDiscounts (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        OrderId INT NOT NULL,
                        DiscountId INT NOT NULL,
                        OrderItemId INT NULL, -- Null if it's order-level discount
                        DiscountAmount DECIMAL(18, 2) NOT NULL CHECK (DiscountAmount > 0), -- snap shot of discount amount
                        FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
                        FOREIGN KEY (DiscountId) REFERENCES Discounts(Id) ON DELETE NO ACTION,
                        FOREIGN KEY (OrderItemId) REFERENCES OrderItems(Id) ON DELETE NO ACTION
                    );
                END;

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'DiscountConditions') AND type = N'U')
                BEGIN
                    CREATE TABLE DiscountConditions (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        DiscountId INT NOT NULL,
                        ConditionConfig NVARCHAR(MAX) NOT NULL,
                        FOREIGN KEY (DiscountId) REFERENCES Discounts(Id) ON DELETE CASCADE
                    );
                END;

                -- Create Indexes
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_Name' AND object_id = OBJECT_ID(N'Products'))
                BEGIN
                    CREATE INDEX IX_Products_Name ON Products(Name);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Categories_Name' AND object_id = OBJECT_ID(N'Categories'))
                BEGIN
                    CREATE INDEX IX_Categories_Name ON Categories(Name);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SubCategories_Name' AND object_id = OBJECT_ID(N'SubCategories'))
                BEGIN
                    CREATE INDEX IX_SubCategories_Name ON SubCategories(Name);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductVariants_ProductId' AND object_id = OBJECT_ID(N'ProductVariants'))
                BEGIN
                    CREATE INDEX IX_ProductVariants_ProductId ON ProductVariants(ProductId);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductVariants_Price' AND object_id = OBJECT_ID(N'ProductVariants'))
                BEGIN
                    CREATE INDEX IX_ProductVariants_Price ON ProductVariants(Price);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductVariants_Quantity' AND object_id = OBJECT_ID(N'ProductVariants'))
                BEGIN
                    CREATE INDEX IX_ProductVariants_Quantity ON ProductVariants(Quantity);
                END;

                -- Insert Seed Data
                INSERT INTO Products (Sku, Name, Description)
                VALUES  ('HAPPY-00101', 'Coke Cola', NULL),
                        ('MILK-00101', 'A Milk', NULL),
                        ('MILK-00102', 'A Milk', NULL),
                        ('MILK-00103', 'B Milk', NULL),
                        ('MILK-00104', 'B Milk', NULL),
                        ('SPW-00102', 'A Singha Soda Water', NULL),
                        ('RB-00101', 'Cheese Rice Ball', NULL),
                        ('RB-00102', 'Tuna Rice Ball', NULL),
                        ('BD-00101', 'Red bean bread', NULL),
                        ('BD-00102', 'Croissant', NULL);

                INSERT INTO ProductVariants (ProductId, Size, Unit, Price, Quantity)
                VALUES 
                    ((SELECT Id FROM Products WHERE Sku = 'HAPPY-00101'), 225, 'mL', 29.00, 10),
                    ((SELECT Id FROM Products WHERE Sku = 'MILK-00101'), 225, 'mL', 35.00, 10),
                    ((SELECT Id FROM Products WHERE Sku = 'MILK-00102'), 450, 'mL', 45.00, 10),
                    ((SELECT Id FROM Products WHERE Sku = 'MILK-00103'), 225, 'mL', 35.00, 10),
                    ((SELECT Id FROM Products WHERE Sku = 'MILK-00104'), 450, 'mL', 45.00, 10),
                    ((SELECT Id FROM Products WHERE Sku = 'SPW-00102'), 250, 'mL', 29.00, 10),
                    ((SELECT Id FROM Products WHERE Sku = 'RB-00101'), 120, 'gram', 35.00, 10),
                    ((SELECT Id FROM Products WHERE Sku = 'RB-00102'), 150, 'gram', 45.00, 10),
                    ((SELECT Id FROM Products WHERE Sku = 'BD-00101'), 120, 'gram', 35.00, 10),
                    ((SELECT Id FROM Products WHERE Sku = 'BD-00102'), 150, 'gram', 45.00, 10);

                INSERT INTO Categories (Name)
                VALUES  ('Beverage'),
                        ('Food');

                INSERT INTO ProductCategories (CategoryId, ProductId)
                VALUES
                    -- Beverages
                    ((SELECT Id FROM Categories WHERE Name = 'Beverage'), (SELECT Id FROM Products WHERE Sku = 'HAPPY-00101')),
                    ((SELECT Id FROM Categories WHERE Name = 'Beverage'), (SELECT Id FROM Products WHERE Sku = 'MILK-00101')),
                    ((SELECT Id FROM Categories WHERE Name = 'Beverage'), (SELECT Id FROM Products WHERE Sku = 'MILK-00102')),
                    ((SELECT Id FROM Categories WHERE Name = 'Beverage'), (SELECT Id FROM Products WHERE Sku = 'MILK-00103')),
                    ((SELECT Id FROM Categories WHERE Name = 'Beverage'), (SELECT Id FROM Products WHERE Sku = 'MILK-00104')),
                    ((SELECT Id FROM Categories WHERE Name = 'Beverage'), (SELECT Id FROM Products WHERE Sku = 'SPW-00102')),

                    -- Food
                    ((SELECT Id FROM Categories WHERE Name = 'Food'), (SELECT Id FROM Products WHERE Sku = 'RB-00101')),
                    ((SELECT Id FROM Categories WHERE Name = 'Food'), (SELECT Id FROM Products WHERE Sku = 'RB-00102')),
                    ((SELECT Id FROM Categories WHERE Name = 'Food'), (SELECT Id FROM Products WHERE Sku = 'BD-00101')),
                    ((SELECT Id FROM Categories WHERE Name = 'Food'), (SELECT Id FROM Products WHERE Sku = 'BD-00102'));

                INSERT INTO SubCategories (Name, CategoryId)
                VALUES  ('Milk', (SELECT Id FROM Categories WHERE Name = 'Beverage')), 
                        ('Soft Drinks', (SELECT Id FROM Categories WHERE Name = 'Beverage')), 
                        ('Sparkling Water', (SELECT Id FROM Categories WHERE Name = 'Beverage')), 
                        ('Rice Ball', (SELECT Id FROM Categories WHERE Name = 'Food')), 
                        ('Bread', (SELECT Id FROM Categories WHERE Name = 'Food'));

                INSERT INTO ProductSubCategories (ProductId, SubCategoryId)
                VALUES
                    -- Soft Drinks
                    ((SELECT Id FROM Products WHERE Sku = 'HAPPY-00101'), (SELECT Id FROM SubCategories WHERE Name = 'Soft Drinks')),

                    -- Milk
                    ((SELECT Id FROM Products WHERE Sku = 'MILK-00101'), (SELECT Id FROM SubCategories WHERE Name = 'Milk')),
                    ((SELECT Id FROM Products WHERE Sku = 'MILK-00102'), (SELECT Id FROM SubCategories WHERE Name = 'Milk')),
                    ((SELECT Id FROM Products WHERE Sku = 'MILK-00103'), (SELECT Id FROM SubCategories WHERE Name = 'Milk')),
                    ((SELECT Id FROM Products WHERE Sku = 'MILK-00104'), (SELECT Id FROM SubCategories WHERE Name = 'Milk')),

                    -- Sparkling Water
                    ((SELECT Id FROM Products WHERE Sku = 'SPW-00102'), (SELECT Id FROM SubCategories WHERE Name = 'Sparkling Water')),

                    -- Rice Ball
                    ((SELECT Id FROM Products WHERE Sku = 'RB-00101'), (SELECT Id FROM SubCategories WHERE Name = 'Rice Ball')),
                    ((SELECT Id FROM Products WHERE Sku = 'RB-00102'), (SELECT Id FROM SubCategories WHERE Name = 'Rice Ball')),

                    -- Bread
                    ((SELECT Id FROM Products WHERE Sku = 'BD-00101'), (SELECT Id FROM SubCategories WHERE Name = 'Bread')),
                    ((SELECT Id FROM Products WHERE Sku = 'BD-00102'), (SELECT Id FROM SubCategories WHERE Name = 'Bread'));

                INSERT INTO Discounts (Name, Description, IsActive)
                VALUES  ('Same Beverage Discount', 'Discount on same drink: 2 get 10%, 3 get 20%', 1),
                        ('Bread Pair Discount', 'Buy 2 breads, cheaper one 50% off', 1),
                        ('Rice ball & Milk Bundle', 'Buy Rice Ball and Milk together, get $10 off', 1),
                        ('Order Total Discount', 'Get $50 off when order total amount is over $1000', 1);

                INSERT INTO DiscountConditions (DiscountId, ConditionConfig)
                VALUES  (1, '{"Conditions":[{"MinQuantity":2,"DiscountValue":10, "IsPercentage": true },{"MinQuantity":3,"DiscountValue":20, "IsPercentage": true}],"ExcludedSubCategoryIds":[1]}'),
                        (2, '{"DiscountValue":50, "IsPercentage": true,"ApplyToCheaperItem":true,"CategoryId":2}'),
                        (3, '{"BundleItemsSubCategoryIds": [1, 4], "DiscountValue": 10,"IsPercentage": false}'),
                        (4, '{"MinOrderAmount": 1000,"DiscountValue": 50,"IsPercentage": false}');
            """;

            await connection.ExecuteAsync(sql);
        }
    }
}
