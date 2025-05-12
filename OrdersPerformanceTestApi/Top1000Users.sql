SELECT TOP 1000
    u.Id AS UserId,
    u.Name AS UserName,
    SUM(op.Quantity * op.Price) AS TotalOrderValue
FROM Users u
INNER JOIN Orders o ON u.Id = o.UserId
INNER JOIN OrderProducts op ON o.Id = op.OrderId
WHERE o.DateAdded >= DATEADD(MONTH, -6, GETDATE())
GROUP BY u.Id, u.Name
HAVING SUM(op.Quantity * op.Price) > 1000.00
ORDER BY TotalOrderValue DESC;