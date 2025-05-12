-- Deleting any previous data in the tables to avoid duplicates.
DELETE FROM OrderProducts;
DELETE FROM Orders;
DELETE FROM Products;
DELETE FROM Users;


-- Seeding the Users table with random data.
SET IDENTITY_INSERT Users ON;

WITH Numbers AS (
   SELECT TOP 10000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Number
   FROM master.dbo.spt_values a
   CROSS JOIN master.dbo.spt_values b
)
INSERT INTO Users (Id, Name)
SELECT
   Number AS Id,
   CONCAT(LEFT(NEWID(), 8), ' ', LEFT(NEWID(), 8)) AS Name
FROM Numbers;

SET IDENTITY_INSERT Users OFF;


-- Seeding the Products table with random data.
SET IDENTITY_INSERT Products ON;

WITH Numbers AS (
  SELECT TOP 10000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Number
  FROM master.dbo.spt_values a
  CROSS JOIN master.dbo.spt_values b
)
INSERT INTO Products (Id, Name, Price)
SELECT
  Number AS Id,
  CONCAT('Product ', Number) AS Name,
  ROUND(RAND(CHECKSUM(NEWID())) * (500 - 200) + 200, 2) AS Price
FROM Numbers;

SET IDENTITY_INSERT Products OFF;


-- Seeding the Orders table with random data.
SET IDENTITY_INSERT Orders ON;

WITH Numbers AS (
  SELECT TOP 1000000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Number
  FROM master.dbo.spt_values a
  CROSS JOIN master.dbo.spt_values b
)
INSERT INTO Orders (Id, UserId, DateAdded)
SELECT
  Number AS Id,
  (SELECT Id FROM Users ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 1 ROW ONLY) AS UserId,
  DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 490), '2024-01-01') AS DateAdded
FROM Numbers;

SET IDENTITY_INSERT Orders OFF;


-- Seeding the OrdersProducts table with random data.
-- Using temporary tables mostly for performance reasons, because they are stored in tempdb in SQL Server.
-- This makes operations on them faster compared to working directly with large permanent tables.
WITH Numbers AS (
    SELECT TOP 300000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Number
    FROM master.dbo.spt_values a
    CROSS JOIN master.dbo.spt_values b
)
SELECT
    ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) + (SELECT ISNULL(MAX(Id), 0) FROM OrderProducts) AS Id,
    ((Number - 1) % (SELECT COUNT(*) FROM Orders)) + 1 AS OrderId,
    ((Number - 1) % (SELECT COUNT(*) FROM Products)) + 1 AS ProductId,
    ABS(CHECKSUM(NEWID()) % 10) + 1 AS Quantity,
    ROUND(RAND(CHECKSUM(NEWID())) * (1000 - 1) + 1, 2) AS Price
INTO #TempOrderProducts
FROM Numbers;

DELETE FROM #TempOrderProducts
WHERE EXISTS (
    SELECT 1
    FROM OrderProducts
    WHERE OrderProducts.OrderId = #TempOrderProducts.OrderId
      AND OrderProducts.ProductId = #TempOrderProducts.ProductId
);

DELETE FROM #TempOrderProducts
WHERE EXISTS (
  SELECT 1
  FROM #TempOrderProducts t2
  WHERE #TempOrderProducts.OrderId = t2.OrderId  
    AND #TempOrderProducts.ProductId = t2.ProductId
    AND #TempOrderProducts.Id > t2.Id
);  

INSERT INTO OrderProducts (Id, OrderId, ProductId, Quantity, Price)
SELECT Id, OrderId, ProductId, Quantity, Price
FROM #TempOrderProducts;

DROP TABLE #TempOrderProducts; -- We need to clean up the temporary table that was used for seeding.


-- Optionally adding indexes to the tables for better performance.
CREATE INDEX IX_Orders_DateAdded ON Orders (DateAdded);
CREATE INDEX IX_Orders_UserId ON Orders (UserId);
CREATE INDEX IX_OrderProducts_OrderId ON OrderProducts (OrderId);


-- Checking the number of entries in each table.
--SELECT COUNT(*) AS TotalProductsEntries
--FROM Products;

--SELECT COUNT(*) AS TotalOrdersEntries
--FROM Orders;

--SELECT COUNT(*) AS TotalOrderProductsEntries
--FROM OrderProducts;

--SELECT COUNT(*) AS TotalUsersEntries
--FROM Users;
