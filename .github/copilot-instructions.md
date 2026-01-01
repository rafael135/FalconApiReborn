# Falcon API Reborn - AI Coding Agent Instructions

## Architecture Overview

**Clean Architecture** implementation with 4 layers:
- **Falcon.Api**: ASP.NET Core Web API with Minimal APIs + SignalR Hub
- **Falcon.Core**: Domain entities, business rules, value objects (no external dependencies)
- **Falcon.Infrastructure**: EF Core, ASP.NET Identity, MassTransit/RabbitMQ, Judge API client
- **Falcon.Worker**: Background worker service (MassTransit consumers)

**Technology Stack**: .NET 10, EF Core, ASP.NET Identity, MediatR, MassTransit, SQL Server, RabbitMQ, SignalR

## Key Architectural Patterns

### 1. Vertical Slice Architecture (Features)
Features are organized in self-contained folders under `src/Falcon.Api/Features/`:
```
Features/
  Auth/
    RegisterUser/
      RegisterUserCommand.cs     # IRequest<TResponse>
      RegisterUserHandler.cs     # IRequestHandler<TCommand, TResult>
      RegisterUserEndpoint.cs    # IEndpoint implementation
      RegisterUserResult.cs      # Response DTO
```

**When creating new features**: Always create all 4 files (Command, Handler, Endpoint, Result) in a dedicated folder.

**Endpoint registration**: Endpoints implement `IEndpoint` interface with `MapEndpoint(IEndpointRouteBuilder)` method. They are auto-discovered and registered via `AddEndpoints()` and `MapEndpoints()` extension methods in [Program.cs](src/Falcon.Api/Program.cs).

### 2. Domain-Driven Design with Business Rules
Domain entities inherit from `Entity` base class in [Falcon.Core/Entity.cs](src/Falcon.Core/Entity.cs) and validate business rules using:
```csharp
protected static void CheckRule(IBusinessRule rule)
{
    if (rule.IsBroken())
        throw new BusinessRuleValidationException(rule);
}
```

**Creating business rules**: Implement `IBusinessRule` interface in `Domain/{Entity}/Rules/`:
- Example: `GroupCannotHaveMoreThanMaxMembersRule` validates max 3 members per group
- Always throw `BusinessRuleValidationException` (handled by `GlobalExceptionHandler`)

### 3. Exception Handling & Validation
The system uses `GlobalExceptionHandler` in [Falcon.Api/Infrastructure/GlobalExceptionHandler.cs](src/Falcon.Api/Infrastructure/GlobalExceptionHandler.cs) to convert exceptions to Problem Details:

- **FormException**: For validation errors. Returns 400 with field-level error dictionary:
  ```csharp
  throw new FormException(new Dictionary<string, string> 
  { 
      { "email", "E-mail já utilizado" },
      { "ra", "RA já cadastrado" }
  });
  ```
- **BusinessRuleValidationException**: For domain rule violations. Returns 400.
- **DomainException**: Base class for domain-specific errors.

**Identity errors**: Use `ToFriendlyDictionary()` extension from [IdentityExtensions.cs](src/Falcon.Infrastructure/Extensions/IdentityException.cs) to convert ASP.NET Identity errors to Portuguese field-level errors.

### 4. ASP.NET Identity Integration
`User` entity extends `IdentityUser`. Identity is configured in [DependencyInjection.cs](src/Falcon.Infrastructure/DependencyInjection.cs):
- Password requirements: 8 chars minimum (no uppercase/digits/special chars required)
- `UserName` is always set to `Email`
- Custom properties: `RA` (student ID), `JoinYear`, `Department`, `GroupId`

**Important**: When working with users, inject `UserManager<User>` and `SignInManager<User>`, not direct DbContext access.

### 5. Role-Based Access Control
The system supports three user roles with distinct registration rules:
- **Student**: Default role, standard registration flow
- **Teacher**: Requires valid `accessCode` during registration (validated via `ITokenService`)
- **Admin**: Cannot be created through standard registration endpoint (must be seeded/created manually)

**Registration validation** (in [RegisterUserHandler.cs](src/Falcon.Api/Features/Auth/RegisterUser/RegisterUserHandler.cs)):
- Email and RA must be unique across all users
- Teacher role requires `accessCode` parameter
- Admin role registration is explicitly blocked

### 6. SignalR + RabbitMQ Real-Time Architecture
**CompetitionHub** ([CompetitionHub.cs](src/Falcon.Api/Features/Competitions/Hubs/CompetitionHub.cs)) handles real-time competition operations:

**Flow**: Client → SignalR Hub → RabbitMQ (MassTransit) → Worker → RabbitMQ → Consumer in API → SignalR Hub → Client

Key components:
- **CompetitionHub**: SignalR hub with `[Authorize]` attribute, handles `SendExerciseAttempt()` method
- **SubmitExerciseResultConsumer**: API-side consumer that receives results from Worker and pushes via SignalR
- **Worker consumers**: Process submission in background, call Judge API, update database
- All SignalR operations use `ConnectionId` to target specific clients and `CorrelationId` for request tracking

**CORS configuration**: Allows `localhost:3000` and `localhost:5173` with credentials for SignalR (see [Program.cs](src/Falcon.Api/Program.cs))

Documentation: See [SIGNALR_RABBITMQ_ARCHITECTURE.md](docs/SIGNALR_RABBITMQ_ARCHITECTURE.md) for complete flow diagram.

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
- **FormException**: Now implemented in [Falcon.Core/Domain/Shared/Exceptions/FormException.cs](src/Falcon.Core/Domain/Shared/Exceptions/FormException.cs)
- **ITokenService**: Implemented in [Falcon.Infrastructure/Auth/TokenService.cs](src/Falcon.Infrastructure/Auth/TokenService.cs)
- **Worker project**: Contains consumers for background jobs (e.g., processing exercise submissions via Judge API)

## Common Pitfalls

1. **Don't bypass domain rules**: Always use entity methods (e.g., `group.AddMember(user)`), never manipulate collections directly
2. **Identity constraints**: Remember `EmailConfirmed` defaults to false, handle this in authentication logic
3. **EF Core configurations**: Located in `src/Falcon.Infrastructure/Database/Configurations/`, auto-discovered via `ApplyConfigurationsFromAssembly`
4. **MassTransit**: Configured with kebab-case endpoints (see `AddApiMassTransit` in [DependencyInjection.cs](src/Falcon.Infrastructure/DependencyInjection.cs))
5. **Concurrency**: `Group` has `RowVersion` for optimistic concurrency control
6. **SignalR authorization**: All hub methods require `[Authorize]` attribute on hub class level

## Integration Points

- **Database**: EF Core with Identity tables + custom domain tables (Groups, Competitions, Exercises, etc.)
- **Message Bus**: MassTransit with RabbitMQ (guest/guest on localhost:5672)
- **Judge API**: External service for code execution (configured via `JudgeApi:Url` in appsettings)
- **API Docs**: Scalar (replaces Swagger) with purple theme at `/scalar/v1`
- **Logging**: Standard ASP.NET Core ILogger injection
- **File Storage**: Local file storage via `IFileStorageService` in `wwwroot/uploads/`
