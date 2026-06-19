# QuanLyHoaLan - Architecture & Context Notes

This file contains the core architectural decisions, patterns, and context of the `QuanLyHoaLan` project. 
**AI Assistants should read this file first** to understand the project structure without needing to scan the entire repository.

## 1. Project Overview
- **Domain**: Orchid Management System (Quản lý Hoa Lan).
- **Architecture**: Clean Architecture.
- **Framework**: ASP.NET Core (.NET).
- **Database**: PostgreSQL (Entity Framework Core).

## 2. Project Structure (Clean Architecture)
The solution is divided into 4 main layers:

### 2.1. Domain Layer (`QuanLyHoaLan.Domain`)
- **Entities**: `Orchid`, `Category`, `Article`, `User`, `Role`, `UploadedImage`, `JwtRefreshToken`.
- Contains core business rules, Enums, and custom Exceptions.
- Defines core Interfaces like `IBaseRepository<T>`, `IUnitOfWork`.

### 2.2. Application Layer (`QuanLyHoaLan.Application`)
- Implements **CQRS** pattern using **MediatR**.
- **Features Folder**: Grouped by use-case and implemented via Commands/Queries:
  - **Auth**: `Login`, `LoginWithGoogle`, `RefreshToken`
  - **Categories**: `CreateCategory`, `UpdateCategory`, `DeleteCategory`, `GetCategories`, `GetCategoryById`
  - **Orchids**: `CreateOrchid`, `UpdateOrchid`, `DeleteOrchid`, `GetOrchids`, `GetOrchidById`
  - **Articles**: `CreateArticle`, `UpdateArticle`, `DeleteArticle`, `GetArticles`, `GetArticleById`
  - **Images**: `UploadImage`, `DeleteImage`
- **MediatR Pipeline Behaviors**:
  - `AuthorizationBehaviour` / `CurrentUserBehaviour`
  - `ValidationBehaviour` (using FluentValidation)
  - `PerformanceBehaviour`
  - `UnitOfWorkBehavior` (auto commits transactions for commands)
  - `LoggingBehaviour` / `UnhandledExceptionBehaviour`
- Contains `DTOs`, `Models`, and interface definitions for external services (`IImageService`, `IJwtTokenGenerator`, `IGoogleAuthService`).

### 2.3. Infrastructure Layer (`QuanLyHoaLan.Infrastructure`)
- **Persistence**: `ApplicationDbContext` (PostgreSQL), `BaseRepository<T>`, `UnitOfWork`.
- **Authentication**: JWT token generation and validation. Google OAuth service.
- **External Services**: Cloudinary service for image uploading.
- Handles EF Core Migrations and Database Seeding.

### 2.4. Presentation Layer (`QuanLyHoaLan.API`)
- ASP.NET Core Web API project.
- **Middlewares**: `SecurityHeadersMiddleware`, `StructuredLoggingMiddleware`, `ErrorHandlingMiddleware` (Global Exception Handling).
- API Documentation using Swagger and ReDoc.
- Configured with Rate Limiting and CORS.

## 3. Key Technical Decisions & Patterns
- **CQRS**: Commands and Queries are strictly separated. Commands modify state and Queries return data.
- **Unit of Work**: Handled automatically via `UnitOfWorkBehavior` in MediatR pipeline. Developers usually don't need to manually call `_unitOfWork.CommitAsync()` in Command Handlers.
- **Validation**: FluentValidation is heavily used and automatically triggered via `ValidationBehaviour`.
- **Error Handling**: Exceptions thrown in the Application layer (e.g., `NotFoundException`, `ValidationException`) are caught and formatted consistently by `ErrorHandlingMiddleware` in the API.
- **Authentication**: JWT Bearer tokens with Refresh Token support.
- **PostgreSQL Array Pattern**: For certain Many-to-Many or One-to-Many relationships where only IDs are needed (e.g., An Article referencing multiple Orchids), avoid creating Join Tables. Instead, use a native PostgreSQL Array column (`List<Guid>` -> `uuid[]`) for massive performance gains on Inserts/Updates and simplified Queries.

## 4. Current Workflow / Guidelines for AI
- When creating a new feature, always follow the CQRS folder structure in `QuanLyHoaLan.Application/Features/[FeatureName]`.
- Place database interactions in the Infrastructure layer, but access them via `IUnitOfWork` or specific repository interfaces in the Application layer.
- Never write business logic in the API Controllers. Controllers should only dispatch MediatR requests and return results.
- For data validation, always create a `[CommandName]Validator` using `AbstractValidator<T>` instead of data annotations on models.
