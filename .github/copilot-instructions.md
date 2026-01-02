# Falcon API Reborn - AI Coding Agent Instructions

> Modern .NET 10 backend for programming competitions with Clean Architecture, DDD, and real-time async processing.

## Architecture Overview

**Clean Architecture** with 4 projects enforcing strict dependency inversion:
- **Falcon.Api**: ASP.NET Core Web API with Minimal APIs + SignalR Hub
- **Falcon.Core**: Domain entities, business rules, value objects (zero external dependencies)
- **Falcon.Infrastructure**: EF Core, ASP.NET Identity, MassTransit/RabbitMQ, Judge API client
- **Falcon.Worker**: Standalone background worker (MassTransit consumers for async processing)

**Technology Stack**: .NET 10, EF Core 10, ASP.NET Identity, MediatR (CQRS), MassTransit, SQL Server, RabbitMQ, SignalR

**Dependency Flow**: Api/Worker → Infrastructure → Core (only Core has no dependencies)

## Key Architectural Patterns

### 1. Vertical Slice Architecture (Features)
Features are self-contained in `src/Falcon.Api/Features/{Domain}/{Action}/`:
```
Features/Auth/RegisterUser/
  ├── RegisterUserCommand.cs     # MediatR IRequest<RegisterUserResult>
  ├── RegisterUserHandler.cs     # IRequestHandler<Command, Result>
  ├── RegisterUserEndpoint.cs    # IEndpoint (Minimal API)
  └── RegisterUserResult.cs      # Response DTO
```

**Feature creation checklist**:
1. Create folder: `Features/{Domain}/{ActionName}/`
2. Add `{Action}Command.cs` implementing `IRequest<T>`
3. Add `{Action}Handler.cs` implementing `IRequestHandler<TCommand, TResult>`
4. Add `{Action}Endpoint.cs` implementing `IEndpoint` with `MapEndpoint()` method
5. Add `{Action}Result.cs` for response shape

**Auto-discovery**: Endpoints implementing `IEndpoint` are automatically registered via reflection in [EndpointExtensions.cs](src/Falcon.Api/Extensions/EndpointExtensions.cs). Never register manually in [Program.cs](src/Falcon.Api/Program.cs).

### 2. Domain-Driven Design with Explicit Business Rules
All domain entities inherit from `Entity` base class ([Entity.cs](src/Falcon.Core/Entity.cs)) and enforce invariants via:
```csharp
protected static void CheckRule(IBusinessRule rule)
{
    if (rule.IsBroken()) throw new BusinessRuleValidationException(rule);
}
```

**Creating business rules**:
1. Place in `Core/Domain/{Entity}/Rules/{RuleName}Rule.cs`
2. Implement `IBusinessRule` interface (properties: `Message`, method: `IsBroken()`)
3. Call from entity methods: `CheckRule(new YourRule(args))`
4. Example: [GroupCannotHaveMoreThanMaxMembersRule](src/Falcon.Core/Domain/Groups/Rules/GroupCannotHaveMoreThanMaxMembersRule.cs) limits groups to 3 members

**Critical**: `BusinessRuleValidationException` is caught by `GlobalExceptionHandler` and returned as 422 Unprocessable Entity.

### 3. Exception Handling & HTTP Problem Details
[GlobalExceptionHandler](src/Falcon.Api/Infrastructure/GlobalExceptionHandler.cs) converts all exceptions to RFC 7807 Problem Details:

| Exception Type | HTTP Status | Use Case | Example |
|---------------|-------------|----------|---------|
| `FormException` | 400 Bad Request | Field-level validation errors | Email/RA duplicates |
| `BusinessRuleValidationException` | 422 Unprocessable Entity | Domain rule violations | Max 3 members per group |
| `NotFoundException` | 404 Not Found | Entity not found | Group ID doesn't exist |
| `DomainException` | 500 Internal Server Error | Unexpected domain errors | Base class for custom errors |

**FormException pattern** (Portuguese field names):
```csharp
throw new FormException(new Dictionary<string, string> 
{ 
    { "email", "E-mail já utilizado" },
    { "ra", "RA já cadastrado" }
});
```

**Identity errors**: Use `ToFriendlyDictionary()` extension ([IdentityExtensions.cs](src/Falcon.Infrastructure/Extensions/IdentityException.cs)) to convert ASP.NET Identity `IdentityError[]` to Portuguese `Dictionary<string, string>`.

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

**Key implementation details**:
- **CompetitionHub**: SignalR hub with `[Authorize]` attribute, handles `SendExerciseAttempt()` method
- **SubmitExerciseResultConsumer** (API): Receives results from Worker and pushes via SignalR to specific client
- **SubmitExerciseConsumer** (Worker): Processes submission, calls Judge API, updates database, publishes result
- **Tracking**: `ConnectionId` targets specific SignalR clients, `CorrelationId` tracks request lifecycle

**CORS setup**: Allows `localhost:3000` and `localhost:5173` with credentials (see [Program.cs](src/Falcon.Api/Program.cs#L33-L42))

**Why this architecture?**:
- SignalR alone blocks: Worker must complete before API responds
- RabbitMQ decouples: API returns immediately, Worker processes async
- Guaranteed delivery: Messages persist if Worker is down
- Scalability: Multiple Worker instances can process queue

Documentation: [SIGNALR_RABBITMQ_ARCHITECTURE.md](docs/SIGNALR_RABBITMQ_ARCHITECTURE.md)

## Development Workflows

### Database Migrations
Use provided PowerShell scripts (NOT direct `dotnet ef` commands):
```powershell
# Create new migration (prompts for name)
.\add-migration.ps1

# Apply migrations to database
.\update-db.ps1
```

Scripts automatically target:
- **Project**: `src/Falcon.Infrastructure`
- **Startup Project**: `src/Falcon.Api`

**Why scripts?**: EF Core migrations require correct project paths. Scripts prevent common errors like targeting wrong project or missing startup project configuration.

### Running the Application
```powershell
# 1. Start RabbitMQ (required for MassTransit)
docker-compose up -d

# 2. Build solution
dotnet build

# 3. Run API (port 5000/5001)
dotnet run --project src/Falcon.Api

# 4. Run Worker (separate terminal)
dotnet run --project src/Falcon.Worker
```

**API Documentation**: Available at `/scalar/v1` in development (Scalar with purple theme, replaces Swagger).

### Testing
```powershell
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Falcon.Core.Tests

# Run with coverage (requires coverlet)
dotnet test /p:CollectCoverage=true
```

**Test structure**: Tests mirror domain structure ([Falcon.Core.Tests/Domain/](tests/Falcon.Core.Tests/Domain/)). Use xUnit + FluentAssertions pattern.

### Configuration
Connection string in [appsettings.Development.json](src/Falcon.Api/appsettings.Development.json):
- SQL Server: `localhost:1433`
- Database: `falcon-reborn-dev`
- Credentials: `sa` user (password in config)

RabbitMQ runs on `localhost:5672` with management UI at `localhost:15672` (guest/guest).

**Environment-specific**: Use `appsettings.{Environment}.json` for environment overrides. Never commit production credentials.

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
