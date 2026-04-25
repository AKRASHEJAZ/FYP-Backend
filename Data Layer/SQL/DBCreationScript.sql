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