USE HaCMS_2122110132;
GO


-- Categories
INSERT INTO Categories (Name, Description) VALUES 
(N'Apple', N'Danh mục điện thoại iPhone chính hãng'),
(N'Samsung', N'Danh mục điện thoại Samsung Galaxy'),
(N'Xiaomi', N'Danh mục điện thoại Xiaomi giá tốt');

-- Users
INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES 
('admin_ha', '123456', N'Nguyễn Văn Hà (Chủ shop)', 'Admin'),
('nhanvien_trinh', '123456', N'Phạm Thị Mai Trinh', 'Sales'),
('nhanvien_02', '123456', N'Nhân viên kho', 'Staff'),
('khach_hang_A', '123456', N'Khách hàng mua iPhone', 'Customer');

-- Products
INSERT INTO Products (Name, Price, Description, ImageUrl) VALUES 
(N'iPhone 15 Pro Max', 29990000, N'Điện thoại Apple cao cấp nhất hiện nay', '/images/anh2.jpg'),
(N'Samsung Galaxy S24 Ultra', 26990000, N'Ông vua Android màn hình lớn kèm bút S-Pen', '/images/anh1.jpg'),
(N'Xiaomi 14 Ultra', 22990000, N'Flagship chụp ảnh đỉnh cao hợp tác với Leica', '/images/anh3.jpg');

-- Posts
INSERT INTO Posts (Title, Content, ImageUrl, CreatedDate, CategoryId) VALUES 
(N'Galaxy S26 Ultra Galaxy AI', N'Công nghệ màn hình Dynamic AMOLED 2X 120Hz, bộ nhớ lớn, AI thông minh.', '/images/anh1.jpg', GETDATE(), 2),
(N'Iphone 17 Pro Max', N'iPhone 17 Pro Max là mẫu điện thoại cao cấp nhất thuộc dòng iPhone 17 Series của Apple.', '/images/anh2.jpg', GETDATE(), 1),
(N'Dòng Flagship cao cấp (Xiaomi Series)', N'Xiaomi nổi tiếng với các dòng điện thoại thông minh cấu hình cao và giá thành hợp lý.', '/images/anh3.jpg', GETDATE(), 3);

-- CategoriesProducts
INSERT INTO CategoriesProducts (CategoryId, ProductId) VALUES 
(1, 1),
(2, 2),
(3, 3);

-- Customers
INSERT INTO Customers (FullName, Email, Phone, Address, [Password]) VALUES 
(N'Phạm Thị Mai Trinh', 'maitrinh@gmail.com', '0909123456', N'123 Đường Ba Tháng Hai, Quận 10, TP.HCM', '123456'),
(N'Lê Hoàng Nam', 'hoangnam@gmail.com', '0987654321', N'456 Đường Nguyễn Trãi, Quận 5, TP.HCM', '123456'),
(N'Trần Minh Tâm', 'minhtam@gmail.com', '0911223344', N'789 Đường Lê Lợi, Quận 1, TP.HCM', '123456');

-- Orders
INSERT INTO Orders (CustomerId, OrderDate, Status, Notes) VALUES 
(1, GETDATE(), 1, N'Đơn hàng của Phạm Thị Mai Trinh'),
(2, GETDATE(), 1, N'Đơn hàng của Lê Hoàng Nam');

-- OrderDetails
INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice) VALUES 
(1, 1, 1, 29990000),
(1, 2, 1, 26990000),
(2, 3, 1, 22990000);

-- ============================================================================
-- BƯỚC 4: KIỂM TRA
-- ============================================================================
SELECT * FROM Categories;
SELECT * FROM Users;
SELECT * FROM Products;
SELECT * FROM Posts;
SELECT * FROM CategoriesProducts;
SELECT * FROM Customers;
SELECT * FROM Orders;
SELECT * FROM OrderDetails;