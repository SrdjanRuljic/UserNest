# Class Diagram Documentation

## Domain Model Overview

The UserNest domain model follows a clean architecture approach with clear entity relationships and separation of concerns.

## Core Entities

### 1. AppUser (User Entity)
- **Inherits**: `IdentityUser` (ASP.NET Core Identity)
- **Purpose**: Represents application users with extended properties
- **Key Properties**:
  - `FirstName`, `LastName`: User's personal information
  - `IsDeleted`: Soft delete flag
  - `LanguageId`: Foreign key to Language entity
  - Audit properties: `Created`, `CreatedBy`, `LastModified`, `LastModifiedBy`

### 2. AppRole (Role Entity)
- **Inherits**: `IdentityRole` (ASP.NET Core Identity)
- **Purpose**: Defines user roles in the system
- **Roles**: Admin, RegularUser

### 3. AppUserRole (User-Role Junction)
- **Inherits**: `IdentityUserRole<string>`
- **Purpose**: Many-to-many relationship between users and roles

### 4. Language (Language Entity)
- **Purpose**: Supports multi-language functionality
- **Key Properties**:
  - `Name`: Language name
  - `Culture`: Culture code (e.g., "en", "sr-Cyrl-RS")
  - `Description`: Optional description
  - `FlagPath`: Path to flag image
  - `IsDeleted`: Soft delete flag

### 5. RefreshToken (Token Entity)
- **Purpose**: Manages JWT refresh tokens for authentication
- **Key Properties**:
  - `Token`: The refresh token string
  - `UserId`: Foreign key to AppUser

### 6. AuditableEntity (Base Class)
- **Purpose**: Provides audit trail functionality
- **Properties**:
  - `Created`, `CreatedBy`: Creation tracking
  - `LastModified`, `LastModifiedBy`: Modification tracking

## Entity Relationships

```mermaid
classDiagram
    class AuditableEntity {
        +DateTime Created
        +string CreatedBy
        +DateTime? LastModified
        +string? LastModifiedBy
    }

    class AppUser {
        +string FirstName
        +string LastName
        +bool IsDeleted
        +long? LanguageId
        +ICollection~RefreshToken~ RefreshTokens
        +ICollection~AppUserRole~ UserRoles
        +Language? Language
    }

    class AppRole {
        +ICollection~AppUserRole~ UserRoles
    }

    class AppUserRole {
        +AppUser User
        +AppRole Role
    }

    class Language {
        +long Id
        +string Name
        +string Culture
        +string? Description
        +string? FlagPath
        +bool IsDeleted
        +ICollection~AppUser~ UsersUsingLanguage
    }

    class RefreshToken {
        +string Token
        +string UserId
        +AppUser User
    }

    class Roles {
        <<enumeration>>
        Admin
        RegularUser
    }

    class Policies {
        <<enumeration>>
        RequireAuthorization
        RequireAdminRole
        RequireRegularUserRole
    }

    %% Relationships
    AppUser --|> AuditableEntity : inherits
    AppUser ||--o{ RefreshToken : "has many"
    AppUser ||--o{ AppUserRole : "has many"
    AppRole ||--o{ AppUserRole : "has many"
    Language ||--o{ AppUser : "used by many"
    
    %% ASP.NET Identity inheritance
    AppUser --|> IdentityUser : inherits
    AppRole --|> IdentityRole : inherits
    AppUserRole --|> IdentityUserRole : inherits
```

## Relationship Details

### 1. User-Language Relationship
- **Type**: Many-to-One (Optional)
- **Description**: Users can optionally select a preferred language
- **Navigation**: `AppUser.Language` ↔ `Language.UsersUsingLanguage`

### 2. User-Role Relationship
- **Type**: Many-to-Many
- **Description**: Users can have multiple roles, roles can be assigned to multiple users
- **Junction Entity**: `AppUserRole`
- **Navigation**: `AppUser.UserRoles` ↔ `AppRole.UserRoles`

### 3. User-RefreshToken Relationship
- **Type**: One-to-Many
- **Description**: Users can have multiple refresh tokens for different sessions
- **Navigation**: `AppUser.RefreshTokens` ↔ `RefreshToken.User`

## Domain Enums

### Roles Enum
```csharp
public enum Roles
{
    Admin,        // System administrator
    RegularUser   // Standard user
}
```

### Policies Enum
```csharp
public enum Policies
{
    RequireAuthorization,      // Any authenticated user
    RequireAdminRole,         // Admin users only
    RequireRegularUserRole    // Regular users only
}
```

## Key Design Patterns

### 1. Soft Delete Pattern
- All entities implement `IsDeleted` flag
- Data is never permanently removed
- Query filters automatically exclude deleted records

### 2. Audit Trail Pattern
- `AuditableEntity` base class provides audit functionality
- Tracks creation and modification timestamps
- Records who created/modified entities

### 3. Identity Integration Pattern
- Extends ASP.NET Core Identity entities
- Maintains compatibility with Identity framework
- Adds custom properties without breaking existing functionality

## Database Schema Implications

### Tables Created
- `AspNetUsers` (extended with custom properties)
- `AspNetRoles` (standard Identity table)
- `AspNetUserRoles` (many-to-many junction)
- `Languages` (custom entity table)
- `RefreshTokens` (custom entity table)

### Indexes
- `IsDeleted` indexes for soft delete queries
- Standard Identity indexes for authentication
- Foreign key indexes for relationships

## Benefits of This Design

1. **Extensibility**: Easy to add new properties to existing entities
2. **Auditability**: Complete audit trail for all changes
3. **Flexibility**: Soft delete allows data recovery
4. **Security**: Role-based access control with policies
5. **Internationalization**: Built-in multi-language support
6. **Compatibility**: Works seamlessly with ASP.NET Core Identity
