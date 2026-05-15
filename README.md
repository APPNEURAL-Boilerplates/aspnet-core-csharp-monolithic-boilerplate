# .NET C# Monolithic Boilerplate

A production-friendly ASP.NET Core **monolithic / modular monolith** starter using C#, .NET 10, controllers, dependency injection, health checks, OpenAPI, tests, Docker, and a sample Users module.

## Stack

- .NET 10 / C#
- ASP.NET Core Web API controllers
- Modular monolith folder structure
- Built-in dependency injection
- Built-in OpenAPI endpoint
- MSTest integration tests
- Docker / Docker Compose

## Structure

```txt
aspnet-core-csharp-monolithic-boilerplate/
├─ src/
│  └─ DotnetMonolith.Api/
│     ├─ Program.cs
│     ├─ appsettings.json
│     ├─ Common/
│     │  ├─ Api/
│     │  ├─ Exceptions/
│     │  ├─ Extensions/
│     │  └─ Middleware/
│     └─ Modules/
│        ├─ Health/
│        ├─ Root/
│        └─ Users/
│           ├─ Controllers/
│           ├─ Dtos/
│           ├─ Entities/
│           ├─ Repositories/
│           └─ Services/
├─ tests/
│  └─ DotnetMonolith.Api.Tests/
├─ http/
│  └─ requests.http
├─ DotnetMonolith.sln
├─ Directory.Build.props
├─ global.json
├─ Dockerfile
├─ docker-compose.yml
├─ Makefile
└─ README.md
```

## Features

```txt
✅ .NET 10 / C#
✅ ASP.NET Core Web API
✅ Monolithic deployment
✅ Modular monolith folders
✅ Controller → service → repository pattern
✅ Health endpoint
✅ Users example module
✅ In-memory repository
✅ DTO validation with DataAnnotations
✅ Centralized exception middleware
✅ Request ID middleware
✅ Security headers middleware
✅ Consistent JSON response envelope
✅ 404 and 405 JSON handling
✅ Built-in OpenAPI JSON endpoint
✅ MSTest integration tests
✅ Dockerfile
✅ docker-compose.yml
✅ Makefile
✅ AGENTS.md for Codex
```

## Requirements

Install the .NET 10 SDK.

```bash
dotnet --version
```

## Run locally

```bash
cp .env.example .env
dotnet restore DotnetMonolith.sln
dotnet run --project src/DotnetMonolith.Api
```

Open:

```txt
http://localhost:8080
http://localhost:8080/health
http://localhost:8080/api/v1/users
http://localhost:8080/api/v1/openapi/v1.json
```

## Run with watch mode

```bash
dotnet watch --project src/DotnetMonolith.Api run
```

Or:

```bash
make dev
```

## Test

```bash
dotnet test DotnetMonolith.sln
```

Or:

```bash
make test
```

## Build

```bash
dotnet build DotnetMonolith.sln --configuration Release
```

## Docker

```bash
docker compose up --build
```

Open:

```txt
http://localhost:8080/health
```

The included Docker Compose file intentionally avoids a container healthcheck that depends on `curl`, because the official ASP.NET runtime image may not include it by default.

## API routes

```txt
GET    /                                      Root metadata
GET    /health                                App health check
GET    /actuator/health                       ASP.NET Core health check
GET    /api/v1/openapi/v1.json                OpenAPI document
GET    /api/v1/users                          List users
POST   /api/v1/users                          Create user
GET    /api/v1/users/{id}                     Get user by ID
PATCH  /api/v1/users/{id}                     Update user
DELETE /api/v1/users/{id}                     Delete user
```

## Example requests

Create user:

```bash
curl -X POST http://localhost:8080/api/v1/users \
  -H "content-type: application/json" \
  -d '{"name":"Grace Hopper","email":"grace@example.com"}'
```

List users:

```bash
curl http://localhost:8080/api/v1/users
```

Health:

```bash
curl http://localhost:8080/health
```

## Response format

Success:

```json
{
  "ok": true,
  "data": {},
  "requestId": "..."
}
```

Error:

```json
{
  "ok": false,
  "error": {
    "code": "NOT_FOUND",
    "message": "The requested endpoint was not found."
  },
  "requestId": "..."
}
```

## Design pattern

This is a single deployable app, organized by module:

```txt
controller → service → repository
```

Example:

```txt
UsersController.cs          HTTP layer
UserService.cs              Business rules
IUserRepository.cs          Persistence contract
InMemoryUserRepository.cs   Temporary persistence implementation
```

## Adding a new module

Create a new folder:

```txt
src/DotnetMonolith.Api/Modules/Orders/
├─ Controllers/
├─ Dtos/
├─ Entities/
├─ Repositories/
└─ Services/
```

Then register services in `Program.cs`:

```csharp
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
```

## Replace in-memory storage with a database

Keep controllers and services unchanged. Replace the repository implementation:

```txt
IUserRepository
└─ EfCoreUserRepository / DapperUserRepository / MongoUserRepository
```

Recommended next additions for a real app:

- EF Core or Dapper
- PostgreSQL or SQL Server
- AuthN/AuthZ
- Serilog or OpenTelemetry
- Rate limiting
- CI pipeline

## Environment variables

Do not commit `.env` files.

Example:

```txt
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:8080
Cors__AllowedOrigins__0=http://localhost:3000
```

## Codex prompt

```txt
Extend this ASP.NET Core modular monolith. Keep it as one deployable API. Add a new Orders module using controller -> service -> repository. Use ApiResponse<T>, AppException, tests, and no secrets in source control. Run dotnet build and dotnet test before finishing.
```
