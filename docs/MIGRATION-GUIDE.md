# Migration Guide

This guide covers database migration operations for the UserNest API.

## Prerequisites

- .NET 8.0 SDK
- SQL Server instance (LocalDB, Express, or Full)
- Entity Framework Core Tools installed globally:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Creating Migrations

### 1. Add New Migration
```bash
dotnet ef migrations add <MigrationName> --project src/Infrastructure/Infrastructure --startup-project src/Presentation/WebAPI
```

**Example:**
```bash
dotnet ef migrations add AddUserProfileTable --project src/Infrastructure/Infrastructure --startup-project src/Presentation/WebAPI
```

### 2. Update Database
```bash
dotnet ef database update --project src/Infrastructure/Infrastructure --startup-project src/Presentation/WebAPI
```

### 3. Remove Last Migration (if needed)
```bash
dotnet ef migrations remove --project src/Infrastructure/Infrastructure --startup-project src/Presentation/WebAPI
```

## Migration Commands Reference

| Command | Description |
|---------|-------------|
| `migrations add <name>` | Create a new migration |
| `database update` | Apply pending migrations to database |
| `database update <migration>` | Update to specific migration |
| `migrations remove` | Remove the last migration |
| `migrations list` | List all migrations |
| `database drop` | Drop the database |

## Common Scenarios

### Adding New Entity
1. Create entity in `src/Core/Domain/Entities/`
2. Add DbSet to `ApplicationDbContext`
3. Create configuration in `src/Infrastructure/Infrastructure/Persistence/Configurations/`
4. Run migration command

### Modifying Existing Entity
1. Update entity properties
2. Update configuration if needed
3. Create migration
4. Update database

### Removing Entity
1. Remove DbSet from context
2. Remove configuration file
3. Create migration
4. Update database

## Migration Files Location

Migrations are stored in:
```
src/Infrastructure/Infrastructure/Persistence/Migrations/
├── 20241029000000_InitialCreate.cs
├── 20241029000001_AddRefreshTokens.cs
└── ApplicationDbContextModelSnapshot.cs
```

## Database Connection Configuration

### Development
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=UserNest;User Id=sa;Password=SysAdmin1234!;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### Production
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=UserNest;User Id=prod-user;Password=secure-password;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

## Troubleshooting

### Migration Conflicts
- Check for pending migrations: `dotnet ef migrations list`
- Resolve conflicts manually in migration files
- Consider removing and recreating migrations for complex changes

### Connection Issues
- Verify SQL Server is running
- Check connection string format
- Ensure database exists or can be created
- Verify user permissions

### Migration Rollback
```bash
# Rollback to specific migration
dotnet ef database update <PreviousMigrationName> --project src/Infrastructure/Infrastructure --startup-project src/Presentation/WebAPI
```

## Best Practices

1. **Always backup database** before applying migrations in production
2. **Test migrations** in development environment first
3. **Use descriptive names** for migrations
4. **Review generated SQL** before applying to production
5. **Keep migrations small** and focused on single changes
6. **Never edit applied migrations** - create new ones instead
