# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Bankman is a banking system API built with .NET 10 and C# 14, following Clean Architecture and Domain-Driven Design principles.

## Commands

```bash
# Build (also runs code style checks and static analysis — warnings are errors)
dotnet build

# Run all tests
dotnet test

# Run a single test by name
dotnet test --filter "FullyQualifiedName~TestClassName.MethodName"

# Run tests in a specific project
dotnet test Domain.UnitTests/
dotnet test ArchitectureTests/

# Run the API
dotnet run --project Api

# Docker
docker compose up
```

## Architecture

The solution follows Clean Architecture with strict dependency rules enforced by `ArchitectureTests/` using NetArchTest.Rules:

```
Api → Application → Domain
Infrastructure → Application → Domain
```

No layer may reference a layer above it. Each layer exposes an `IMarker` interface used by the architecture tests for assembly reflection.

### Layers

- **Domain** — Pure business logic, no framework dependencies. Contains all abstractions (see below) and will contain domain entities/aggregates/value objects.
- **Application** — Use cases and orchestration. References only Domain.
- **Infrastructure** — Persistence, external services. References Application.
- **Api** — ASP.NET Core minimal API host. References Application and Infrastructure.

### Domain Abstractions (`Domain/Abstractions/`)

| Type | Purpose |
|---|---|
| `Entity<TId>` | Base for identity-bearing objects; equality by ID |
| `AggregateRoot<TId>` | Extends Entity; owns `IDomainEvent` collection via `RaiseDomainEvent()` / `ClearDomainEvents()` |
| `ValueObject` | Abstract record; equality by structural value |
| `Result<TValue>` / `Result` | Railway-oriented programming — always return instead of throwing for expected failures |
| `Error` | Immutable `(Code, Description, ErrorType)` record; factory methods: `Validation`, `NotFound`, `Conflict`, `Unauthorized`, `Forbidden` |
| `Guard` | Input validation with `CallerArgumentExpression`; throws on invalid input at boundaries |
| `IDomainEvent` | Marker interface for domain events; carries `OccurredOn` timestamp |
| `IRepository<TAggregate, TId>` | Generic repo contract: `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` |
| `IUnitOfWork` | Single `SaveChangesAsync` method; manages transaction boundaries |

### Result Pattern

Use `Result`/`Result<T>` for all expected failure paths. Prefer `Result.Success()` / `Result.Failure(error)` factory methods. `Result<T>` supports implicit conversion from a value (success) or null (failure with `Error.NullValue`). Accessing `.Value` on a failed result throws `InvalidOperationException`.

## Testing

- Framework: **xUnit** with **Shouldly** assertions
- Domain unit tests mirror the `Abstractions/` folder structure
- Use sealed inner classes to instantiate abstract base types in tests
- Architecture tests use `NetArchTest.Rules` — add a test whenever a new layer dependency rule is established

## Code Quality

`Directory.Build.props` enforces across all projects:
- `TreatWarningsAsErrors = true` — the build is the linter
- `EnforceCodeStyleInBuild = true` — `.editorconfig` rules are build errors
- `SonarAnalyzer.CSharp` runs on every build

`.editorconfig` key rules: explicit accessibility modifiers required, no `this.` qualification, use language keywords over BCL types (`int` not `Int32`), avoid unnecessary parentheses.

## Package Management

All NuGet package versions are centralized in `Directory.Packages.props` at the solution root. Add new packages with a `<PackageVersion>` entry there; reference them without versions in individual `.csproj` files.
