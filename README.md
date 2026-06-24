# 🌸 Mai Trinh Studio — Fashion & Beauty E-Commerce

<div align="center">

![Mai Trinh Studio](https://images.unsplash.com/photo-1490481651871-ab68de25d43d?q=80&w=1200&h=300&fit=crop)

**Hệ thống thương mại điện tử thời trang & làm đẹp cao cấp**  
Xây dựng bằng React + ASP.NET Core + SQL Server

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-7038-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-61DAFB?style=flat&logo=react)](https://reactjs.org/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?style=flat&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![JWT](https://img.shields.io/badge/Auth-JWT%20%2B%20Cookie-000000?style=flat&logo=jsonwebtokens)](https://jwt.io/)

</div>

---

## 👨‍🎓 Thông tin sinh viên

| Thông tin | Chi tiết |
|-----------|----------|
| **Họ và tên** | Nguyễn Văn Hà |
| **MSSV** | 2122110132 |
| **Lớp** | CCQ2211D |
| **Môn học** | ASP.NET |
| **Giảng viên** | Nguyễn Cao Thái |
| **GitHub** | [NguyenVanHa-2122110132/ChuyenDe_ASP.NET1](https://github.com/NguyenVanHa-2122110132/ChuyenDe_ASP.NET1) |

---

## 📖 Giới thiệu dự án

**Mai Trinh Studio** là hệ thống thương mại điện tử chuyên về thời trang & làm đẹp cao cấp, được xây dựng theo mô hình **Full-Stack** với kiến trúc tách biệt Frontend (React) và Backend (ASP.NET Core Web API).

Dự án hướng đến phong cách **luxury minimal** với gam màu vàng gold (#b8975a) và typography Cormorant Garamond, mang lại trải nghiệm mua sắm tinh tế và sang trọng.

---

## ✨ Tính năng chính

### 🛍️ Phía Khách hàng (React Frontend)
- **Trang chủ** — Banner, sản phẩm Mới / Hot / Sale
- **Thời Trang** — Lọc theo danh mục, kích cỡ, khoảng giá, tìm kiếm
- **Nước Hoa** — Danh mục nước hoa Unisex / Nam / Nữ
- **Mỹ Phẩm & Phụ Kiện** — Đầy đủ danh mục sản phẩm
- **Giỏ hàng** — Thêm, sửa số lượng, xóa sản phẩm (localStorage)
- **Yêu thích (Wishlist)** — Lưu sản phẩm yêu thích với badge đếm
- **Đặt hàng (Checkout)** — Form thông tin, COD & QR VietQR tự động
- **Blog / Xu hướng** — Bài viết thời trang, lọc theo danh mục
- **Tìm kiếm AI** — Chatbot tích hợp Groq API (llama-3.1-8b-instant)
- **Đăng ký / Đăng nhập** — OTP qua email, JWT Authentication
- **Quên mật khẩu** — Reset qua link email

### 🔧 Phía Quản trị (ASP.NET Core MVC)
- **Dashboard** — Tổng quan hệ thống
- **Quản lý Sản phẩm** — CRUD đầy đủ, upload ảnh
- **Quản lý Danh mục** — Phân cấp danh mục
- **Quản lý Đơn hàng** — Xác nhận, hủy đơn, hoàn kho
- **Quản lý Bài viết** — CKEditor 4 soạn thảo nội dung
- **Quản lý Khách hàng** — Danh sách, thông tin chi tiết
- **Quản lý Kho hàng** — Theo dõi tồn kho
- **Phân quyền** — Administrator / Admin / Sales / Cashier / Warehouse

---

## 🏗️ Kiến trúc hệ thống

```
ChuyenDe_ASP.NET1/
├── CMS.Backend/                  # ASP.NET Core Web API + MVC
│   ├── Controllers/
│   │   ├── API Controllers/      # REST API cho React (JWT Auth)
│   │   │   ├── AuthApiController.cs
│   │   │   ├── OrdersController.cs
│   │   │   ├── ProductsController.cs
│   │   │   └── ...
│   │   └── MVC Controllers/      # Trang Admin (Cookie Auth)
│   │       ├── BaseAdminController.cs
│   │       ├── PostController.cs
│   │       ├── ProductController.cs
│   │       └── ...
│   ├── Services/
│   │   └── EmailService.cs       # Gmail SMTP via MailKit
│   └── Program.cs                # Cấu hình JWT + Cookie Auth
│
├── CMS.Data/                     # Entity Framework Core
│   ├── Entities/                 # Model: Product, Order, Customer...
│   ├── ApplicationDbContext.cs
│   └── HaCMS_FullData.sql        # Script khởi tạo database
│
└── cms.frontend/                 # React 18
    └── src/
        ├── pages/
        │   ├── FashionPage.jsx   # Trang thời trang + sidebar lọc
        │   ├── Shop.jsx          # Trang shop chung
        │   ├── Checkout.jsx      # Đặt hàng + QR VietQR
        │   ├── WishlistPage.jsx  # Trang yêu thích
        │   ├── BlogPage.jsx      # Blog thời trang
        │   └── ...
        └── api/
            └── productService.js
```

---

## 🔐 Bảo mật

| Tính năng | Mô tả |
|-----------|-------|
| **JWT Bearer** | Xác thực API React, hết hạn 7 ngày |
| **Cookie Auth** | Xác thực trang Admin MVC, hết hạn 5 phút |
| **Password Hash** | ASP.NET Identity PasswordHasher |
| **OTP Email** | Xác minh email khi đăng ký, hết hạn 5 phút |
| **Rate Limiting** | Giới hạn 5 lần đăng nhập / 5 phút theo IP |
| **CORS** | Chỉ cho phép `localhost:3000` |
| **Security Headers** | X-Frame-Options, X-XSS-Protection, MIME Sniffing |

---

## 🛠️ Công nghệ sử dụng

### Backend
| Công nghệ | Mục đích |
|-----------|----------|
| ASP.NET Core.NET 10 | Web API + MVC Framework |
| Entity Framework Core | ORM, Code-First |
| SQL Server | Cơ sở dữ liệu chính |
| JWT Bearer | Xác thực API |
| MailKit | Gửi email SMTP Gmail |
| Newtonsoft.Json | Xử lý JSON |
| Swagger / OpenAPI | Tài liệu API |

### Frontend
| Công nghệ | Mục đích |
|-----------|----------|
| React 18 | UI Framework |
| React Router v6 | Điều hướng trang |
| Groq API | Chatbot AI tìm kiếm sản phẩm |
| VietQR | Tạo mã QR thanh toán động |
| CKEditor 4 | Soạn thảo bài viết |
| localStorage | Giỏ hàng, Wishlist, Auth token |

---

## 🚀 Hướng dẫn cài đặt

### Yêu cầu hệ thống
- .NET 8 SDK
- Node.js 18+
- SQL Server 2019+
- Visual Studio 2022

### 1. Clone dự án
```bash
git clone https://github.com/NguyenVanHa-2122110132/ChuyenDe_ASP.NET1.git
cd ChuyenDe_ASP.NET1
```

### 2. Cài đặt Database
```sql
-- Chạy file script trong SQL Server Management Studio
-- File: HaCMS_FullData.sql
```

### 3. Cấu hình Backend
Mở file `CMS.Backend/appsettings.json`, cập nhật:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HaCMS_2122110132;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "MaiTrinhSecretKey2026MaiTrinhSecretKey2026"
  },
  "EmailSettings": {
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password"
  }
}
```

### 4. Chạy Backend
```bash
cd CMS.Backend
dotnet run
# Backend chạy tại: https://localhost:7038
```

### 5. Cài đặt & chạy Frontend
```bash
cd cms.frontend
npm install
npm start
# Frontend chạy tại: http://localhost:3000
```

---

## 📊 Cơ sở dữ liệu

- **Database:** `HaCMS_2122110132`
- **~318 sản phẩm** thuộc 31 danh mục (Thời trang Nam / Nữ / Trẻ Em, Nước Hoa, Mỹ Phẩm, Phụ Kiện)
- **~25 thương hiệu** nước hoa cao cấp (Chanel, Dior, Gucci, YSL, Kilian...)

### Các bảng chính
```
Customers     — Tài khoản khách hàng
Users         — Tài khoản quản trị
Products      — Sản phẩm (~318 records)
Categories    — Danh mục (31 danh mục)
Orders        — Đơn hàng
OrderDetails  — Chi tiết đơn hàng
Carts         — Giỏ hàng
Posts         — Bài viết blog
OtpCodes      — Mã OTP xác thực
```

---

## 📱 Giao diện

### Phong cách thiết kế
- **Màu chủ đạo:** Gold `#b8975a` — Cream `#f7f3ee`
- **Typography:** Cormorant Garamond + Jost
- **Style:** Luxury Minimal — Sang trọng, tinh tế

### Các trang chính
| Trang | URL |
|-------|-----|
| Trang chủ | `/` |
| Thời trang | `/thoi-trang` |
| Nước hoa | `/nuoc-hoa` |
| Mỹ phẩm | `/my-pham` |
| Phụ kiện | `/phu-kien` |
| Giỏ hàng | `/cart` |
| Đặt hàng | `/checkout` |
| Yêu thích | `/yeu-thich` |
| Blog | `/blog` |
| Đăng nhập | `/login` |
| Admin | `https://localhost:7038` |

---

## 📧 Liên hệ

**Nguyễn Văn Hà** — MSSV: 2122110132  
📧 hambr2802@gmail.com  
🔗 [GitHub](https://github.com/NguyenVanHa-2122110132/ChuyenDe_ASP.NET1)

---

<div align="center">
  <sub>© 2026 Mai Trinh Studio — Môn ASP.NET | Giảng viên: Nguyễn Cao Thái</sub>
</div>
