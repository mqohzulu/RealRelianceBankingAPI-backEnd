# RealRelianceBankingAPI-backEnd

Backend service for managing persons, accounts, and transactions for the RealReliance Banking assessment. The solution follows a Clean Architecture layout and exposes REST endpoints through the API project.

## Solution structure

- `RealRelianceBankingAPI`: ASP.NET Core Web API entry point, controllers, and configuration.
- `RealRelianceBanking.Application`: Application layer with commands, queries, validators, and interfaces.
- `RealRelianceBanking.Domain`: Domain entities and aggregates.
- `RealRelianceBanking.Infrastructure`: Dapper persistence, database context, and infrastructure services.
- `Tests`: Unit tests for core handlers and application behavior.

## Key capabilities

- Person management with uniqueness rules and search support.
- Account creation and closure with balance rules.
- Transaction creation, updates, and transfers with balance updates and capture-date tracking.
- Validation pipeline enforced through FluentValidation.

## Data setup

Use `InteractiveMVC_Tables_With_Data.sql` to create the required tables and seed sample data in SQL Server.

## Development notes

- Default API host is configured in `RealRelianceBankingAPI/Program.cs`.
- Configure connection strings and JWT settings in `RealRelianceBankingAPI/appsettings.json`.

## Running tests

Run tests from the repository root:

```
dotnet test Tests/Tests.csproj
```
