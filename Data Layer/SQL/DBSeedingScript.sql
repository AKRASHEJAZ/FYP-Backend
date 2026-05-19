-- =============================================
-- Seed Data: Default Roles
-- =============================================

INSERT INTO Roles (Name)
VALUES 
('Admin'),
('Cashier');

-- =============================================
-- Seed Data: Default Customer
-- =============================================

INSERT INTO Customers (Name, IsWalkIn)
VALUES ('Walk-in Customer', 1);