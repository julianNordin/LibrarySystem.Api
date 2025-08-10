# LibrarySystem.Api

A small ASP.NET Core Web API for managing a library's book lending — books, members, and loans — with real lending rules enforced server-side, not just CRUD.

**Status:** 🚧 Under active development, built in dated phases. See [Roadmap](#roadmap) below.

## Why this project

Most CRUD tutorials stop at create/read/update/delete. This one adds a genuine relationship (a `Loan` links a `Book` and a `Member`) and real business rules that live in a service layer:

- A book that's already out on loan can't be borrowed again until it's returned.
- A member is capped at a fixed number of active loans at once.
- Loans have a due date, and overdue status is computed from it.

## Tech stack

| Layer | Choice |
|---|---|
| Framework | ASP.NET Core Web API (.NET 9, controllers) |
| Data access | EF Core (code-first + migrations) |
| Database | SQL Server LocalDB |
| Validation | FluentValidation |
| API docs | OpenAPI (`Microsoft.AspNetCore.OpenApi`) |
| Testing | xUnit |

## Getting started

**Prerequisites:** [.NET 9 SDK](https://dotnet.microsoft.com/download), SQL Server LocalDB (ships with Visual Studio, or install separately).

```bash
git clone <this-repo>
cd LibrarySystem.Api  # repo root contains LibrarySystem.sln
dotnet build
dotnet test
dotnet run --project LibrarySystem.Api
```

The API listens on `http://localhost:5018` by default. The OpenAPI document is served at `/openapi/v1.json`.

## Project structure

```
LibrarySystem.sln
LibrarySystem.Api/          # the API project
  Domain/                   # entities: Book, Member, Loan
  Data/                     # AppDbContext, migrations
  Services/                 # business rules (borrow/return, overdue calc, active-loan cap)
  DTOs/                     # request/response models + FluentValidation validators
  Controllers/              # REST endpoints
LibrarySystem.Api.Tests/    # xUnit unit + integration tests
```

## Business rules

- **Active loan cap:** a member may have at most **5** active (not-yet-returned) loans.
- **No double-lending:** a book with an active loan cannot be borrowed again until returned.
- **Loan period:** 14 days from the borrow date; a loan is overdue once the due date has passed with no return recorded.

## Roadmap

- [x] Project scaffolding (solution, Web API project, xUnit project, gitignore)
- [ ] NuGet packages & solution folder structure
- [ ] Domain entities (`Book`, `Member`, `Loan`)
- [ ] `AppDbContext` & entity configuration
- [ ] Initial migration, LocalDB, seed data
- [ ] Service layer & DI wiring
- [ ] Book service (CRUD)
- [ ] Member service (CRUD)
- [ ] Loan service — borrow/return business rules
- [ ] DTOs & FluentValidation
- [ ] Book & Member controllers
- [ ] Loan controller (borrow/return/overdue)
- [ ] Swagger/OpenAPI polish & global error handling
- [ ] Unit tests for business rules
- [ ] Integration tests
- [ ] Final cleanup & polish

## License

Personal learning/portfolio project — no license specified yet.
