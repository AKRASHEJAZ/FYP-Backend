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