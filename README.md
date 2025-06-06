# 🛒 E-Commerce Backend API

A robust and scalable e-commerce backend built with **ASP.NET Core**. It handles user authentication, product management, order processing, and more — designed with modularity and real-world features in mind.

---

## 🚀 Features

- 👤 User roles: Admin, Seller, Buyer
- 🔐 JWT-based authentication & role-based authorization
- 📦 Product CRUD with image upload (Cloudinary support)
- 🛍️ Shopping cart & orders
- 📂 Category management
- 📸 Cloudinary integration for product image hosting
- 📨 Email notification (optional)
- 📈 Clean architecture & scalable structure
- 🌐 CORS enabled for frontend integration

---

## 🛠️ Tech Stack

- **ASP.NET Core** (.NET 7+)
- **Entity Framework Core** (Code-First + Migrations)
- **SQL Server / SQLite**
- **JWT Authentication**
- **Cloudinary** (Image hosting)
- **AutoMapper** (DTO Mapping)
- **Swagger / Swashbuckle** (API Docs)

---


---

## ⚙️ Getting Started

### 1. Clone the repository

```bash

git clone https://github.com/yourusername/e-commerce-backend.git
cd e-commerce-backend
```
### 2. Update appsettings.json

```json
"ConnectionStrings": {
  "DefaultConnection": "your_connection_string_here"
},
"JwtSettings": {
  "Key": "your_jwt_secret",
  "Issuer": "your_app",
  "Audience": "your_users"
},
"CloudinarySettings": {
  "CloudName": "your-cloud-name",
  "ApiKey": "your-api-key",
  "ApiSecret": "your-api-secret"
}
```
### 3. Run EF Core Migrations

```bash
dotnet ef database update
```
### 4. Run the application

```bash
dotnet run
```


