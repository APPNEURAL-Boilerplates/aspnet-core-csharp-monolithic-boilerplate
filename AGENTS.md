# Agent instructions

This repository is an ASP.NET Core modular-monolith boilerplate.

## Goals

- Keep it a single deployable API unless explicitly asked to split services.
- Organize new business areas under `src/DotnetMonolith.Api/Modules/<FeatureName>`.
- Use the pattern: controller -> service -> repository.
- Keep HTTP concerns in controllers and business rules in services.
- Keep persistence behind repository interfaces.
- Return the existing `ApiResponse<T>` envelope for JSON endpoints.
- Use `AppException` for expected application errors.

## Before finishing changes

Run:

```bash
dotnet restore DotnetMonolith.sln
dotnet build DotnetMonolith.sln
dotnet test DotnetMonolith.sln
```

## Style

- Target `net10.0`.
- Nullable reference types stay enabled.
- Prefer small, explicit classes over hidden magic.
- Do not commit `.env` files or secrets.
