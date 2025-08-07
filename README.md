# Enterprise WebAPI Starter Kit
+ ASP.NET Core 10.0 (Web API)
+ Entity Framework Core (SQL Server, real DB)
+ .bat script chạy migrations, init DB
+ JWT Authentication
+ Serilog (ghi log ra file + console)
+ Role-Based Access Control (RBAC)
+ Tự động seed Roles + Admin user
+ Swagger UI bảo vệ bằng JWT
+ FluentValidation cho DTOs
+ Export CSV / Excel / PDF / Word / ZIP
+ BackgroundService định kỳ (ví dụ: cleanup job mỗi 1 phút)
+ xUnit tests cho Controller và Service Layer

# Solution Architect
```
/MyApp.sln
│
├── src/
│   ├── MyApp.BlazorUI/                --> Blazor Server App (Fluent UI, Pages)
│   ├── MyApp.Application/             --> Use Cases, Interfaces
│   ├── MyApp.Domain/                  --> Entities, Enums, Logic
│   ├── MyApp.Infrastructure/          --> EF Core, Dapper, Repository, Migrations
│   └── MyApp.Persistence/             --> EF DbContext + Repository Implementation (optional split)
│
├── tests/
│   ├── MyApp.UnitTests/
│   └── MyApp.IntegrationTests/
│
└── README.md
```

## MyApp.Domain/ (Core Domain)
+ Entities/: Entity classes (POCOs), không phụ thuộc EF
+ Enums/: Domain enums
+ ValueObjects/: Optional
+ Interfaces/: (optional) Nếu có domain service
+ Specifications/: (optional) nếu theo pattern Specification
+ Ví dụ:
```
// Entities/User.cs
public class User {
    public Guid Id { get; set; }
    public string Name { get; set; }

}
```

## MyApp.Application/ (Business Use Cases)
+ Interfaces/Repositories/: IGenericRepository<T>, IUserRepository
+ Interfaces/Services/: IEmailService, ITransactionManager
+ DTOs/: Input/Output models (application-level)
+ UseCases/: Application services or handlers (use case per folder)
+ Common/: Utilities, constants, etc.
+ Ví dụ:
```
UseCases/
└── Users/
    ├── CreateUserCommand.cs
    ├── CreateUserHandler.cs
    ├── GetUserQuery.cs
    └── GetUserHandler.cs
```
+ UseCase có thể dùng Mediator (ex: MediatR) hoặc thuần C# theo CQRS.

## MyApp.Infrastructure/ (Cross-Cutting, EF Core, Services)
+ Data/: EF DbContext, Migrations
+ Repositories/: EF/Dapper implementation of repository interfaces
+ Services/: EmailSender, Logging, Background jobs
+ Transaction/: TransactionScope/DbTransaction implementation
+ Configurations/: Fluent API config (if not using annotations)
+ Ví dụ:
```
// Data/ApplicationDbContext.cs
public class ApplicationDbContext : DbContext {
    public DbSet<User> Users { get; set; }
}

// Repositories/UserRepository.cs
public class UserRepository : IUserRepository {
    private readonly ApplicationDbContext _db;
}
```
+ Tuỳ cách chia, bạn có thể split thành 2 projects: Infrastructure (for common services) và Persistence (chứa EF Core cụ thể).

## MyApp.BlazorUI/ (Presentation Layer)
+ Pages/: Razor pages (Index.razor, Users.razor)
+ Components/: Reusable components (UserCard.razor)
+ Layouts/: Layouts (MainLayout, AdminLayout)
+ Services/: Application service wrapper, ViewModel, AuthService
+ DI/: Dependency injection startup
+ Themes/: Fluent UI customization
+ wwwroot/: Static assets (CSS, JS)
+ Ví dụ:
```
Pages/
└── Users/
    ├── UsersPage.razor
    ├── UserForm.razor
    └── UserDetail.razor
```

## Clean Architecture Dependency Flow
```
BlazorUI → Application → Domain
               ↑
         Infrastructure → (implements Application)
```
+ Blazor UI chỉ gọi Interface từ Application, không gọi trực tiếp Infra.

## Hướng dẫn khởi tạo solution
+ Tạo solution và 4 projects:
```
D:
md MyApp
dotnet new sln -n MyApp
md src
cd src
dotnet new blazorserver -n MyApp.BlazorUI
dotnet new classlib -n MyApp.Application
dotnet new classlib -n MyApp.Domain
dotnet new classlib -n MyApp.Infrastructure
cd ..
dotnet sln add ./src/MyApp.*
```
+ Thêm reference:
```
cd D:/MyApp/src/MyApp.Application
dotnet add MyApp.Application reference MyApp.Domain
cd D:/MyApp/src/MyApp.Infrastructure
dotnet add MyApp.Infrastructure reference MyApp.Application
dotnet add MyApp.Infrastructure reference MyApp.Domain
cd D:/MyApp/src/MyApp.BlazorUI
dotnet add MyApp.BlazorUI reference MyApp.Application
```
+ Cài các packages cần thiết:
```
# EF Core
dotnet add MyApp.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add MyApp.Infrastructure package Microsoft.EntityFrameworkCore.Design

# Blazor Fluent UI
dotnet add MyApp.BlazorUI package Microsoft.FluentUI.AspNetCore.Components

# Transaction
dotnet add MyApp.Infrastructure package System.Transactions
```

## Testing
+ MyApp.UnitTests/: test use cases và domain logic
+ MyApp.IntegrationTests/: test EF repository, database

## Notes khi dùng Transaction truyền thống
+ Tạo interface ITransactionManager trong Application
+ Triển khai TransactionScope hoặc DbTransaction trong Infrastructure
+ Dùng using var tx = transactionManager.Begin(); trong UseCase

## ITransactionManager
```
public interface ITransactionManager {
    Task BeginAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
```

## Hoặc dùng Func<Task>:
```
public interface IUnitOfWork {
    Task ExecuteAsync(Func<Task> action);
}
```

# Blazor Server App + Fluent UI + EF Core + Clean Architecture

## 1. Tách biệt concern rõ ràng (Separation of Concerns)

"Không trộn lẫn logic business, logic trình bày (UI), và logic truy cập dữ liệu trong cùng một nơi."

### 🎯 Mục tiêu:
+ Mỗi layer/tầng chỉ chịu trách nhiệm 1 loại logic duy nhất
+ Giảm phụ thuộc lẫn nhau → dễ thay đổi, dễ unit test

|Tầng | Trách nhiệm | Không được làm |
|----|----|----|
| Domain | Định nghĩa nghiệp vụ cốt lõi (Entities, Enums) | Không dùng EF, không chứa validation UI |
| Application | Chứa use-case cụ thể (handlers, service) | Không biết gì về EF, DB, UI |
| Infrastructure | Cài đặt kỹ thuật (EF Core, Dapper, File, SMTP, Redis, Logging) | Không được dùng Razor component |
| BlazorUI | Trình bày dữ liệu, nhận input từ người dùng | Không chứa business logic, không gọi DbContext trực tiếp |

## 2. Dễ bảo trì, dễ test

"Khi một phần thay đổi, các phần khác không bị ảnh hưởng hoặc dễ dàng được kiểm thử riêng biệt."

### 🎯 Mục tiêu:
+ Không cần DB hoặc UI để test logic
+ Có thể mock dễ dàng các dependency
+ Đảm bảo ứng dụng hoạt động đúng logic mà không cần chạy thật

### Áp dụng:

#### Unit test dễ dàng
```
public class CreateUserHandlerTests {
    [Fact]
    public async Task CreateUser_ShouldAddNewUser_WhenValidInput() {
        var userRepoMock = new Mock<IUserRepository>();
        var handler = new CreateUserHandler(userRepoMock.Object);        
        var result = await handler.Handle(new CreateUserCommand { Name = "Test" });
        userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }
}
```

#### Refactor dễ:
+ Bạn thay EF bằng Dapper → chỉ sửa trong Infrastructure
+ Bạn thay Blazor bằng MAUI → không đụng gì Application/Domain
+ Bạn cần thêm audit log → chỉ sửa cross-cutting concern

## 3. Sẵn sàng scale sang Microservices hoặc Multi-layer APIs

"Nếu ngày mai phải chia hệ thống thành các microservice nhỏ, bạn không cần viết lại từ đầu."

### Mục tiêu:
+ Tăng tính mở rộng theo chiều ngang (scalability)
+ Dễ tách ra thành API riêng, background service riêng, mobile backend riêng

### Áp dụng:

#### Việc tách Application + Domain thành core libraries:
+ Bạn có thể dùng lại Application layer trong:
    + Blazor UI
    + Web API (RESTful)
    + gRPC Service
    + Background Job Worker

#### Tách DB access:
+ Bạn có thể tạo 1 microservice chỉ làm nhiệm vụ Read (CQRS ReadModel)
+ Một service khác chỉ quản lý User

## Ví dụ mở rộng:
| Dự án hiện tại | Mở rộng thành |
|----|----|
| Blazor + Clean Architecture | Thêm ASP.NET Core Web API (MyApp.API) |
| EF Core + SQL Server | Thêm MongoDB Read Model cho performance |
| EmailService nội bộ | Tách riêng thành NotificationService |
| Application logic | Dùng lại trong BackgroundWorker gửi báo cáo |

## Tóm tắt ngắn gọn:
| Lợi ích | Ý nghĩa |
|----|----|
| Tách biệt concern | Dễ hiểu, dễ quản lý, không rối logic |
| Dễ test/bảo trì | Thay đổi 1 phần không ảnh hưởng phần khác |
| Dễ scale | Chuẩn bị sẵn cho microservices, worker, API |

## Markdown Tables
+ https://www.codecademy.com/resources/docs/markdown/tables
+ https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/organizing-information-with-tables
