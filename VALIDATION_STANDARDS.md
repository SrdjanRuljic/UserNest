# UserNest API - Validation Standards & Swagger Documentation

## 📋 Validation Standards Established

### Field Length Constraints

| Field | Min Length | Max Length | Notes |
|-------|------------|------------|-------|
| **UserName** | 3 | 256 | Matches ASP.NET Core Identity default |
| **Email** | - | 256 | Matches ASP.NET Core Identity default |
| **Password** | 6 | 100 | Includes complexity requirements |
| **FirstName** | - | 100 | **NO minimum length** - matches entity configuration |
| **LastName** | - | 100 | **NO minimum length** - matches entity configuration |
| **PhoneNumber** | - | 20 | International format support |
| **RefreshToken** | 10 | 500 | JWT token length |

### Password Complexity Requirements
- Minimum 6 characters
- Maximum 100 characters
- Must contain at least:
  - One lowercase letter (a-z)
  - One uppercase letter (A-Z)
  - One digit (0-9)
  - One special character (#$^+=!*()@%&)

### Username Format Requirements
- Must contain only:
  - Letters (a-z, A-Z)
  - Digits (0-9)
  - Special characters: `-`, `.`, `_`, `@`, `+`

### Email Format Requirements
- RFC-compliant email validation
- Case-insensitive matching
- Prevents consecutive dots and leading/trailing dots

### Phone Number Format Requirements
- International format support
- Allows: `+`, digits, spaces, hyphens, parentheses
- Maximum 20 characters

## 🔧 Files Updated

### Data Annotations Updated
1. **InsertUserCommand.cs** - Aligned with custom validator and entity config
2. **UpdateUserCommand.cs** - Standardized validation lengths
3. **LoginCommand.cs** - Updated username length to match Identity standards

### Custom Validators
1. **InsertUserCommandValidator.cs** - Added password max length validation
2. **UpdateUserCommandValidator.cs** - Created new validator for consistency

## 🔧 Files Updated

### Data Annotations Updated
1. **InsertUserCommand.cs** - Aligned with custom validator and entity config
2. **UpdateUserCommand.cs** - Standardized validation lengths
3. **LoginCommand.cs** - Updated username length to match Identity standards

### Custom Validators
1. **InsertUserCommandValidator.cs** - Added password max length validation
2. **UpdateUserCommandValidator.cs** - Created new validator for consistency

## 🔧 Files Updated

### Data Annotations Updated
1. **InsertUserCommand.cs** - Aligned with custom validator and entity config
2. **UpdateUserCommand.cs** - Standardized validation lengths
3. **LoginCommand.cs** - Updated username length to match Identity standards

### Custom Validators
1. **InsertUserCommandValidator.cs** - Added password max length validation
2. **UpdateUserCommandValidator.cs** - Created new validator for consistency

### Swagger Documentation
1. **ExampleOperationFilter.cs** - Updated examples to reflect actual validation constraints

### Entity Configuration Fixed
1. **AppUserConfiguration.cs** - Added missing `.HasMaxLength(256)` for Email and UserName fields

### Translation Resources
1. **Translation.resx** - Added missing validation message keys
2. **Translation.en.resx** - Added English translations
3. **Translation.sl.resx** - Added Slovenian translations
4. **Translation.sr-Cyrl-RS.resx** - Added Serbian Cyrillic translations
5. **Translation.sr-Latn-RS.resx** - Added Serbian Latin translations
6. **Translation.Designer.cs** - Updated Designer file with new translation properties

### New Translation Keys Added
- `UserNameTooShort` - "Username must be at least 3 characters long"
- `PasswordTooLong` - "Password cannot exceed {0} characters"

**Note**: Removed `FirstNameTooShort` and `LastNameTooShort` as the entity configuration does not specify minimum length requirements for these fields.

## 🎯 Consistency Achieved

### ✅ Data Annotations ↔ Custom Validators
- All validation rules now match between data annotations and custom validators
- No more conflicting length constraints

### ✅ Entity Configuration ↔ Validation Rules
- Database constraints align with validation rules
- Entity configuration matches custom validator limits

### ✅ Swagger Examples ↔ Validation Rules
- API documentation examples reflect actual validation constraints
- Users can see realistic examples that will pass validation

## 🚀 Benefits

1. **Consistent User Experience**: No more confusing validation errors due to conflicting rules
2. **Accurate API Documentation**: Swagger examples match actual validation requirements
3. **Maintainable Code**: Single source of truth for validation rules
4. **Better Security**: Proper password complexity and input validation
5. **Identity Framework Compliance**: Username and email lengths match ASP.NET Core Identity defaults

## 📝 Next Steps

1. **Test Validation**: Run the application and test all endpoints with the new validation rules
2. **Update Client Applications**: Ensure frontend applications use the correct field lengths
3. **Database Migration**: Verify existing data complies with new constraints
4. **Documentation**: Update API documentation to reflect the new validation standards

## 🔍 Validation Flow

```
Request → Data Annotations → Custom Validator → Business Logic → Database
```

Each layer now enforces consistent validation rules, ensuring data integrity throughout the application stack.
