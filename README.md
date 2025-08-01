 <img align="left" width="116" height="116" src="https://raw.githubusercontent.com/jasontaylordev/CleanArchitecture/main/.github/icon.png" />
 
# E-COMMERCE-API BY Clean Architecture
 
<br/>

[![.NET](https://img.shields.io/badge/.NET-6.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/6.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Đây là một API E-Commerce được xây dựng theo kiến trúc Clean Architecture, cung cấp các chức năng quản lý sản phẩm, đơn hàng, người dùng và thanh toán.

## 🚀 Công nghệ sử dụng

* [ASP.NET Core 6](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0) - Web framework
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/) - ORM
* [AutoMapper](https://automapper.org/) - Object mapping
* [MediatR](https://github.com/jbogard/MediatR) - CQRS pattern
* [FluentValidation](https://fluentvalidation.net/) - Validation
* [JWT](https://jwt.io/) - Authentication
* [Swagger/OpenAPI](https://swagger.io/) - API documentation
* SQL Server - Database

## 📋 Yêu cầu hệ thống

* [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
* [SQL Server](https://www.microsoft.com/en-us/sql-server) hoặc SQL Server LocalDB
* [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) hoặc [Visual Studio Code](https://code.visualstudio.com/)

## 🛠️ Cài đặt và Chạy dự án

### 1. Clone repository

```bash
git clone [repository-url]
cd E-COMMERCE-API
```

### 2. Khôi phục packages

```bash
dotnet restore
```

### 3. Cập nhật Connection String

Cập nhật connection string trong `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=true;MultipleActiveResultSets=true;"
  }
}
```

### 4. Chạy Migration

```bash
dotnet ef database update --project src/EfCore.Persistence --startup-project src/WebApi
```

### 5. Chạy ứng dụng

```bash
dotnet run --project src/WebApi
```

API sẽ chạy tại: `https://localhost:5001` hoặc `http://localhost:5000`

Swagger UI: `https://localhost:5001/swagger`

## 🏗️ Cấu trúc dự án

## 🏗️ Cấu trúc dự án

```
src/
├── Domain/                      # Domain Layer - Entities, Value Objects, Domain Services
│   ├── Entities/               # Domain entities
│   ├── Enums/                  # Domain enumerations
│   └── Interfaces/             # Domain interfaces
│
├── Application/                # Application Layer - Use Cases, DTOs, Interfaces
│   ├── Dtos/                   # Data Transfer Objects
│   ├── Interfaces/             # Application interfaces
│   ├── Constants/              # Application constants
│   ├── Enums/                  # Application enumerations
│   ├── Exceptions/             # Custom exceptions
│   ├── Extensions/             # Extension methods
│   ├── Localization/           # Localization resources
│   └── Utility/                # Utility classes
│
├── EfCore.Persistence/         # Infrastructure Layer - Data Access
│   ├── Contexts/               # DbContext classes
│   ├── Repositories/           # Repository implementations
│   ├── Migrations/             # EF Core migrations
│   └── UnitOfWork/             # Unit of Work pattern
│
├── Cloud.Service/              # Cloud Services Layer
│   ├── AWS/                    # AWS services
│   ├── Email/                  # Email services
│   ├── Google/                 # Google services
│   └── Invoice/                # Invoice services
│
├── Shared/                     # Shared Layer - Common services
│   ├── Mappings/               # AutoMapper profiles
│   └── Services/               # Shared services
│
├── Common/                     # Common utilities
│   └── Serilog/                # Logging configuration
│
└── WebApi/                     # Presentation Layer - Controllers, Middleware
    ├── Controllers/            # API controllers
    ├── Authentication/         # Authentication configuration
    ├── Filter/                 # Action filters
    ├── Helpers/                # Helper classes
    └── Models/                 # API models
```

## 📚 Kiến trúc Clean Architecture

### Domain Layer
Chứa tất cả các entities, enums, exceptions, interfaces và logic đặc thù của domain. Đây là lớp trung tâm và không phụ thuộc vào bất kỳ lớp nào khác.

### Application Layer
Chứa toàn bộ application logic. Lớp này phụ thuộc vào domain layer nhưng không phụ thuộc vào bất kỳ lớp hoặc project nào khác. Lớp này định nghĩa các interfaces được implement bởi các lớp bên ngoài.

### Infrastructure Layer (EfCore.Persistence)
Chứa các classes để truy cập các tài nguyên bên ngoài như file systems, web services, database, v.v. Các classes này dựa trên interfaces được định nghĩa trong application layer.

### Presentation Layer (WebApi)
Chứa các API controllers, middleware, và cấu hình authentication. Đây là điểm entry cho ứng dụng.

## 🔧 Các tính năng chính

- ✅ **Clean Architecture** - Kiến trúc rõ ràng, dễ bảo trì
- ✅ **CQRS Pattern** - Tách biệt Command và Query
- ✅ **Repository Pattern** - Trừu tượng hóa data access
- ✅ **Unit of Work** - Quản lý transaction
- ✅ **AutoMapper** - Object-to-object mapping
- ✅ **FluentValidation** - Validation mạnh mẽ
- ✅ **JWT Authentication** - Bảo mật API
- ✅ **Swagger/OpenAPI** - API documentation
- ✅ **Logging với Serilog** - Ghi log chuyên nghiệp
- ✅ **Docker Support** - Containerization
- ✅ **Cloud Services** - Tích hợp AWS, Google Cloud

## 🐳 Chạy với Docker

### Sử dụng Docker Compose

```bash
# Chạy với MVC
cd docker/mvc
./up.ps1

# Chạy với Angular
cd docker/ng
./up.ps1
```

### Build riêng biệt

```bash
# Build MVC
./build/build-mvc.ps1

# Build với Angular
./build/build-with-ng.ps1
```

## 📝 API Documentation

Sau khi chạy ứng dụng, truy cập Swagger UI tại:
- **Development**: `https://localhost:5001/swagger`
- **Production**: `https://your-domain.com/swagger`

## 🧪 Testing

```bash
# Chạy tất cả tests
dotnet test

# Chạy tests với coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🗃️ Database

### Migration Commands

```bash
# Tạo migration mới
dotnet ef migrations add <MigrationName> --project src/EfCore.Persistence --startup-project src/WebApi

# Cập nhật database
dotnet ef database update --project src/EfCore.Persistence --startup-project src/WebApi

# Xóa migration cuối
dotnet ef migrations remove --project src/EfCore.Persistence --startup-project src/WebApi
```

### Seed Data

Database sẽ được seed với dữ liệu mẫu khi khởi chạy lần đầu. Xem file `Database/seed-data.txt` để biết thêm chi tiết.

## 🤝 Đóng góp

1. Fork dự án
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

## 📞 Hỗ trợ

Nếu bạn gặp vấn đề, vui lòng tạo issue trên GitHub hoặc liên hệ qua email.

## License

This project is licensed with the [MIT license](LICENSE).
