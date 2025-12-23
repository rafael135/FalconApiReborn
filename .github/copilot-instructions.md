# Falcon API Reborn - AI Coding Agent Instructions

## Architecture Overview

**Clean Architecture** implementation with 4 layers:
- **Falcon.Api**: ASP.NET Core Web API with endpoints
- **Falcon.Core**: Domain entities, business rules, value objects (no external dependencies)
- **Falcon.Infrastructure**: EF Core, ASP.NET Identity, MassTransit/RabbitMQ
- **Falcon.Worker**: Background worker service

**Technology Stack**: .NET 10, EF Core, ASP.NET Identity, MediatR, MassTransit, SQL Server, RabbitMQ

## Key Architectural Patterns

### 1. Vertical Slice Architecture (Features)
Features are organized in self-contained folders under `src/Falcon.Api/Features/`:
```
Features/
  Auth/
    RegisterUser/
      RegisterUserCommand.cs     # IRequest<TResponse>
      RegisterUserHandler.cs     # IRequestHandler<TCommand, TResult>
      RegisterUserEndpoint.cs    # Minimal API endpoint
      RegisterUserResult.cs      # Response DTO
```

**When creating new features**: Always create all 4 files (Command, Handler, Endpoint, Result) in a dedicated folder.

### 2. Domain-Driven Design with Business Rules
Domain entities inherit from `Entity` base class in `Falcon.Core` and validate business rules using:
```csharp
protected static void CheckRule(IBusinessRule rule)
```

**Creating business rules**: Implement `IBusinessRule` interface in `Domain/{Entity}/Rules/`:
- Example: `GroupCannotHaveMoreThanMaxMembersRule` validates max 3 members per group
- Thrown via `BusinessRuleValidationException` when rule is broken

### 3. ASP.NET Identity Integration
`User` entity extends `IdentityUser`. Identity is configured in [DependencyInjection.cs](src/Falcon.Infrastructure/DependencyInjection.cs):
- Password requirements are relaxed (6 chars minimum, no special requirements)
- `UserName` is always set to `Email`
- Custom properties: `RA` (student ID), `JoinYear`, `Department`, `GroupId`

**Important**: When working with users, inject `UserManager<User>` and `SignInManager<User>`, not direct DbContext access.

### 4. Role-Based Access Control
The system supports three user roles with distinct registration rules:
- **Student**: Default role, standard registration flow
- **Teacher**: Requires valid `accessCode` during registration (validated via token service)
- **Admin**: Cannot be created through standard registration endpoint (must be seeded/created manually)

**Registration validation** (in [RegisterUserHandler.cs](src/Falcon.Api/Features/Auth/RegisterUser/RegisterUserHandler.cs)):
- Email and RA must be unique across all users
- Teacher role requires `accessCode` parameter
- Admin role registration is explicitly blocked with error message

## Development Workflows

### Database Migrations
Use provided PowerShell scripts (NOT direct `dotnet ef` commands):
```powershell
# Create new migration
.\add-migration.ps1

# Apply migrations
.\update-db.ps1
```

Scripts automatically target:
- **Project**: `src/Falcon.Infrastructure`
- **Startup Project**: `src/Falcon.Api`

### Running the Application
```powershell
# Start RabbitMQ (required for MassTransit)
docker-compose up -d

# Build solution
dotnet build

# Run API (Development mode with Scalar API docs)
dotnet run --project src/Falcon.Api
```

**API Documentation**: Available at `/scalar/v1` in development (Scalar with purple theme).

### Configuration
Connection string in [appsettings.Development.json](src/Falcon.Api/appsettings.Development.json):
- SQL Server: `localhost:1433`
- Database: `falcon-reborn-dev`
- Credentials: `sa` user (password in config)

RabbitMQ runs on `localhost:5672` with management UI at `localhost:15672` (guest/guest).

## Project Conventions

### Naming & Structure
- **Entities**: Pascal case, private setters, constructors enforce invariants
- **Feature folders**: Match entity/action name (e.g., `RegisterUser`, not `UserRegistration`)
- **MediatR requests**: Named `{Action}{Entity}Command`, returns `{Action}{Entity}Result`
- **Endpoints**: Use minimal APIs, defined in `{Action}{Entity}Endpoint.cs`

### Entity Design Patterns
1. **Private constructors** for EF Core
2. **Public constructors** with validation for business invariants
3. **Private setters** on all properties
4. **Collections**: Private `List<T>` fields, public `IReadOnlyCollection<T>` properties
5. **Business logic methods**: Public methods like `AddMember()`, `Rename()` that call `CheckRule()`

Example from [Group.cs](src/Falcon.Core/Domain/Groups/Group.cs):
```csharp
private readonly List<User> _users = new();
public virtual IReadOnlyCollection<User> Users => _users.AsReadOnly();

public void AddMember(User user)
{
    CheckRule(new GroupCannotHaveMoreThanMaxMembersRule(_users.Count));
    _users.Add(user);
}
```

### Code Quality Notes
- **TODOs in RegisterUserHandler**: 
  - `ITokenService` not implemented (needed for Teacher accessCode validation)
  - `FormException` not implemented (custom validation error response format)
- **Commented code**: Validation logic for email/RA uniqueness and role checks is commented out waiting for FormException implementation (see lines 52-89 in [RegisterUserHandler.cs](src/Falcon.Api/Features/Auth/RegisterUser/RegisterUserHandler.cs))
- **Worker project**: Empty placeholder for future background jobs

## Common Pitfalls

1. **Don't bypass domain rules**: Always use entity methods (e.g., `group.AddMember(user)`), never manipulate collections directly
2. **Identity constraints**: Remember `EmailConfirmed` defaults to false, handle this in authentication logic
3. **EF Core configurations**: Located in `src/Falcon.Infrastructure/Database/Configurations/`, auto-discovered via `ApplyConfigurationsFromAssembly`
4. **MassTransit**: Configured with kebab-case endpoints, consumers not yet implemented
5. **Concurrency**: `Group` has `RowVersion` for optimistic concurrency control

## Integration Points

- **Database**: EF Core with Identity tables + custom `Groups` table
- **Message Bus**: MassTransit with RabbitMQ (guest/guest on localhost)
- **API Docs**: Scalar (replaces Swagger) with purple theme at `/scalar/v1`
- **Logging**: Standard ASP.NET Core ILogger injection
