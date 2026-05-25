-- =============================================
-- Table: Roles
-- Purpose: Stores system roles (Admin, Cashier, etc.)
-- =============================================

CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

-- =============================================
-- Table: Users
-- Purpose: Stores system users and authentication data
-- =============================================

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    
    Name NVARCHAR(100) NOT NULL,
    
    Email NVARCHAR(150) NOT NULL UNIQUE,
    
    PasswordHash NVARCHAR(MAX) NOT NULL,
    
    RoleId INT NOT NULL,
    
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    
    IsActive BIT NOT NULL DEFAULT 1,

    -- Foreign Key Constraint
    CONSTRAINT FK_Users_Roles
        FOREIGN KEY (RoleId)
        REFERENCES Roles(Id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE UserAuditLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,

    UserId INT NOT NULL,
    Action NVARCHAR(50) NOT NULL,

    PerformedBy INT NULL,
    PerformedAt DATETIME NOT NULL DEFAULT GETDATE(),

    Details NVARCHAR(MAX) NULL,

    CONSTRAINT FK_UserAuditLogs_User
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_UserAuditLogs_PerformedBy
        FOREIGN KEY (PerformedBy) REFERENCES Users(Id)
        -- NO ACTION (default)
);

-- =========================================
-- Categories Table
-- =========================================
CREATE TABLE Categories
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    Name NVARCHAR(100) NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- =========================================
-- Units Table
-- =========================================
CREATE TABLE Units
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    Name NVARCHAR(50) NOT NULL,

    Symbol NVARCHAR(20) NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- =========================================
-- Products Table
-- =========================================
CREATE TABLE Products
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    Name NVARCHAR(150) NOT NULL,

    CategoryId INT NOT NULL,

    UnitId INT NOT NULL,

    InternalCode NVARCHAR(50) NULL,

    IsActive BIT NOT NULL DEFAULT 1,

    IsSellable BIT NOT NULL DEFAULT 1,

    IsPurchasable BIT NOT NULL DEFAULT 1,

    DoesExpire BIT NOT NULL DEFAULT 0,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Products_Category
        FOREIGN KEY (CategoryId)
        REFERENCES Categories(Id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_Products_Unit
        FOREIGN KEY (UnitId)
        REFERENCES Units(Id)
        ON DELETE NO ACTION
);

-- =========================================
-- Useful Indexes
-- =========================================

CREATE INDEX IX_Products_CategoryId
ON Products(CategoryId);

CREATE INDEX IX_Products_UnitId
ON Products(UnitId);

-- =========================================
-- InventoryBatches Table
-- =========================================

CREATE TABLE InventoryBatches
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    ProductId INT NOT NULL,

    PurchasePrice DECIMAL(18,2) NOT NULL,

    SellingPrice DECIMAL(18,2) NOT NULL,

    Quantity DECIMAL(18,2) NOT NULL,

    BatchCode NVARCHAR(100) NULL,

    MFGDate DATE NULL,

    ExpiryDate DATE NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_InventoryBatches_Product
        FOREIGN KEY (ProductId)
        REFERENCES Products(Id)
);

-- ===================
-- Customer Table
-- ===================

CREATE TABLE Customers
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    Name NVARCHAR(150) NOT NULL,

    Phone NVARCHAR(30) NULL,
    Email NVARCHAR(150) NULL,

    Address NVARCHAR(250) NULL,

    IsWalkIn BIT NOT NULL DEFAULT 0,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- ============================
-- Sales, InventoryActions Tables
-- ============================

CREATE TABLE Sales
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    CustomerId INT NULL,

    TotalAmount DECIMAL(18,2) NOT NULL,

    SaleDate DATETIME2 NOT NULL DEFAULT GETDATE(),

    CreatedBy INT NOT NULL,

    FOREIGN KEY (CustomerId)
        REFERENCES Customers(Id),

    FOREIGN KEY (CreatedBy)
        REFERENCES Users(Id)
);

CREATE TABLE InventoryActions
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    InventoryBatchId INT NOT NULL,

    ActionType INT NOT NULL,

    Quantity DECIMAL(18,2) NOT NULL,

    ReferenceId INT NULL,

    ReferenceType INT NULL,

    Notes NVARCHAR(250) NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    CreatedBy INT NOT NULL,

    FOREIGN KEY (InventoryBatchId)
        REFERENCES InventoryBatches(Id),

    FOREIGN KEY (CreatedBy)
        REFERENCES Users(Id)
);