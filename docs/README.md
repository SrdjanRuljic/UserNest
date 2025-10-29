# UserNest API

A comprehensive user management API with authentication and authorization features built using Clean Architecture principles.

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd UserNest
   ```

2. **Configure Database Connection**
   - Update `src/Presentation/WebAPI/appsettings.json` or `appsettings.Development.json`
   - Modify the `ConnectionStrings:DefaultConnection` to match your SQL Server instance
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=UserNest;User Id=sa;Password=YourPassword;TrustServerCertificate=true;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Run Migrations**
   ```bash
   dotnet ef database update --project src/Infrastructure/Infrastructure --startup-project src/Presentation/WebAPI
   ```

4. **Run the Application**
   ```bash
   dotnet run --project src/Presentation/WebAPI
   ```

5. **Access Swagger UI**
   - Navigate to `https://localhost:7000/swagger` (or the port shown in console)

## Project Structure

```
UserNest/
├── src/
│   ├── Core/
│   │   ├── Domain/           # Entities, Enums, Domain Logic
│   │   └── Application/      # Commands, Queries, Behaviors
│   ├── Infrastructure/       # Data Access, External Services
│   └── Presentation/
│       └── WebAPI/           # Controllers, Middleware, Configuration
├── tests/                    # Unit & Integration Tests
└── docs/                     # Documentation
```

## Key Features

- **Clean Architecture**: Separation of concerns with Domain, Application, Infrastructure, and Presentation layers
- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **Role-based Authorization**: Admin and Regular User roles with policy-based authorization
- **Multi-language Support**: Localization for English, Serbian (Cyrillic/Latin), and Slovenian
- **Audit Trail**: Automatic tracking of entity changes with created/modified timestamps
- **Soft Delete**: Entities are soft-deleted instead of hard-deleted
- **Comprehensive Logging**: Structured logging with Serilog
- **API Documentation**: Swagger/OpenAPI documentation with examples

## Environment Configuration

### Development
- Database seeding runs automatically
- Swagger UI enabled
- Detailed logging enabled

### Production
- Update JWT secrets in configuration
- Configure proper database connection
- Set appropriate logging levels
- Disable Swagger in production

## API Endpoints

- **Authentication**: `/api/auth/login`, `/api/auth/register`, `/api/auth/refresh`
- **Users**: `/api/users` (CRUD operations)
- **Authorization**: Bearer token required for protected endpoints

## Testing

```bash
# Run unit tests
dotnet test tests/Application.UnitTests

# Run integration tests
dotnet test tests/Application.IntegrationTests
```

## Documentation

- [Migration Guide](MIGRATION-GUIDE.md) - Database migration instructions
- [Architecture](ARCHITECTURE.md) - Detailed architecture documentation
- [Class Diagram](CLASS-DIAGRAM.md) - Domain model visualization
