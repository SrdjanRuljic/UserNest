# Architecture Documentation

## Overview

UserNest follows **Clean Architecture** principles with clear separation of concerns across four main layers.

## Architecture Layers

### 1. Domain Layer (`src/Core/Domain/`)
**Purpose**: Contains business entities and domain logic
**Dependencies**: None (pure business logic)

**Key Components:**
- `Entities/`: Core business entities (AppUser, AppRole, Language, RefreshToken)
- `Enums/`: Domain enumerations (Roles, Policies, Languages)
- `Common/`: Base classes (AuditableEntity)

**Proof of Clean Architecture:**
- No external dependencies
- Pure C# classes with business rules
- Independent of infrastructure concerns

### 2. Application Layer (`src/Core/Application/`)
**Purpose**: Contains application logic, commands, queries, and behaviors
**Dependencies**: Domain layer only

**Key Components:**
- `Commands/`: CQRS command handlers
- `Queries/`: CQRS query handlers  
- `Common/`: Interfaces, behaviors, pagination
- `Mappings/`: AutoMapper profiles
- `Resources/`: Localization resources

**Proof of Clean Architecture:**
- References only Domain layer
- Uses MediatR for CQRS pattern
- Implements pipeline behaviors for cross-cutting concerns

### 3. Infrastructure Layer (`src/Infrastructure/Infrastructure/`)
**Purpose**: Implements external concerns (database, authentication, services)
**Dependencies**: Application and Domain layers

**Key Components:**
- `Persistence/`: Entity Framework DbContext and configurations
- `Auth/`: JWT token generation
- `Identity/`: ASP.NET Core Identity integration
- `Services/`: External service implementations

**Proof of Clean Architecture:**
- Implements interfaces from Application layer
- Contains all external dependencies
- Database configurations separated from domain

### 4. Presentation Layer (`src/Presentation/WebAPI/`)
**Purpose**: Web API controllers and configuration
**Dependencies**: All other layers

**Key Components:**
- `Controllers/`: REST API endpoints
- `Middlewares/`: Custom middleware
- `Services/`: Presentation-specific services
- `ViewModels/`: API request/response models

## Architectural Patterns

### 1. CQRS (Command Query Responsibility Segregation)
**Implementation**: MediatR with separate Commands and Queries
**Proof**: 
- Commands in `Application/Users/Commands/`
- Queries in `Application/Users/Queries/`
- Separate handlers for each operation

### 2. Repository Pattern
**Implementation**: `IApplicationDbContext` interface
**Proof**: 
- Interface in Application layer
- Implementation in Infrastructure layer
- Clean separation of data access

### 3. Dependency Injection
**Implementation**: Service registration in ConfigureServices
**Proof**:
- Application services registered in `Application/ConfigureServices.cs`
- Infrastructure services in `Infrastructure/ConfigureServices.cs`
- Presentation services in `Program.cs`

### 4. Pipeline Behaviors
**Implementation**: MediatR pipeline behaviors
**Proof**:
- `AuthorizationBehavior`: Handles authorization
- `LoggingBehavior`: Request/response logging
- `LoggingExceptionBehavior`: Exception handling

## Cross-Cutting Concerns

### 1. Authentication & Authorization
- JWT Bearer token authentication
- Role-based authorization policies
- Custom authorization behaviors

### 2. Logging
- Serilog structured logging
- Request/response logging via behaviors
- File and console output

### 3. Localization
- Multi-language support (EN, SR-Cyrl, SR-Latn, SL)
- Resource files in Application layer
- Request localization middleware

### 4. Audit Trail
- `AuditableEntity` base class
- Automatic Created/Modified tracking
- Soft delete implementation

## Data Flow

```
Request → Controller → MediatR → Command/Query Handler → Repository → Database
                ↓
Response ← ViewModel ← Handler ← Domain Entity ← Infrastructure
```

## Security Architecture

### 1. Authentication Flow
1. User provides credentials
2. JWT token generated with claims
3. Refresh token stored for token renewal
4. Bearer token used for subsequent requests

### 2. Authorization Policies
- `RequireAdminRole`: Admin-only access
- `RequireRegularUserRole`: Regular user access
- `RequireAuthorization`: Any authenticated user

### 3. Data Protection
- Password hashing via ASP.NET Core Identity
- JWT token validation
- Soft delete for data retention

## Testing Architecture

### 1. Unit Tests (`tests/Application.UnitTests/`)
- Test individual components in isolation
- Mock external dependencies
- Focus on business logic

### 2. Integration Tests (`tests/Application.IntegrationTests/`)
- Test complete workflows
- Use in-memory database
- Test API endpoints end-to-end

## Benefits of This Architecture

1. **Maintainability**: Clear separation of concerns
2. **Testability**: Each layer can be tested independently
3. **Scalability**: Easy to add new features without affecting existing code
4. **Flexibility**: Can swap implementations (e.g., different databases)
5. **SOLID Principles**: Follows all SOLID principles
6. **Clean Code**: Easy to understand and modify

## Technology Stack

- **Framework**: .NET 8.0
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity + JWT
- **CQRS**: MediatR
- **Mapping**: AutoMapper
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI
- **Testing**: xUnit, FluentAssertions
