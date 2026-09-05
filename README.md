# LibrarySystem.Api

A small ASP.NET Core Web API for managing a library's book lending — books, members, and loans — with real lending rules enforced server-side, not just CRUD.

**Status:** ✅ Feature-complete. See [Roadmap](#roadmap) below.

## Related projects

- [LibrarySystem.Web](https://github.com/julianNordin/LibrarySystem.Web) — the React + TypeScript frontend for this API
- [librarysystem-azure-deploy](https://github.com/julianNordin/librarysystem-azure-deploy) — this API and its frontend, deployed to Azure with Bicep, Key Vault, and a credential-free CI/CD pipeline

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
| Database | SQL Server (Express or LocalDB) |
| Validation | FluentValidation (auto-validation on model binding) |
| API docs | Swagger / OpenAPI (Swashbuckle, with XML doc comments) |
| Error handling | Global `IExceptionHandler` → RFC 9457 `ProblemDetails` |
| Testing | xUnit — unit tests (EF Core InMemory) + integration tests (`WebApplicationFactory`) |

## Getting started

**Prerequisites:** [.NET 9 SDK](https://dotnet.microsoft.com/download), a local SQL Server instance — SQL Server LocalDB (ships with Visual Studio) or SQL Server Express both work. Update the `DefaultConnection` string in `appsettings.Development.json` to match whichever you have (this repo defaults to a local SQL Server Express instance, `Server=.\SQLEXPRESS`).

```bash
git clone <this-repo>
cd LibrarySystem.Api  # repo root contains LibrarySystem.sln
dotnet build
dotnet test
dotnet run --project LibrarySystem.Api
```

The API listens on `http://localhost:5018` by default. On first run it applies pending migrations and seeds 5 books + 3 members automatically — no manual `dotnet ef database update` needed.

Browse the interactive Swagger UI at **`http://localhost:5018/swagger`**.

## Project structure

```
LibrarySystem.sln
LibrarySystem.Api/          # the API project
  Domain/                   # entities: Book, Member, Loan
  Data/                     # AppDbContext, migrations, DbInitializer (seed data)
  Services/                 # business rules (borrow/return, overdue calc, active-loan cap)
  DTOs/                     # request/response models, mapping extensions, FluentValidation validators
  Controllers/              # REST endpoints
  Common/                   # custom exceptions + global exception handler
LibrarySystem.Api.Tests/    # xUnit unit + integration tests
```

**No repository layer.** Services talk to `AppDbContext` directly — `DbContext` already is a Unit-of-Work/Repository, and with 3 entities and one data store, an extra abstraction would just be ceremony.

## Business rules

- **Active loan cap:** a member may have at most **5** active (not-yet-returned) loans.
- **No double-lending:** a book with an active loan cannot be borrowed again until returned.
- **Loan period:** 14 days from the borrow date; a loan is overdue once the due date has passed with no return recorded.

## API reference

All endpoints return JSON. Errors use `application/problem+json` (RFC 9457 `ProblemDetails`).

### Books — `/api/books`

| Method | Route | Description |
|---|---|---|
| GET | `/api/books` | List all books |
| GET | `/api/books/{id}` | Get one book |
| POST | `/api/books` | Add a book |
| PUT | `/api/books/{id}` | Update a book |
| DELETE | `/api/books/{id}` | Delete a book (409 if it has loan history) |

### Members — `/api/members`

Same shape as Books: `GET /api/members`, `GET /api/members/{id}`, `POST /api/members`, `PUT /api/members/{id}`, `DELETE /api/members/{id}` (409 if the member has loan history).

### Loans — `/api/loans`

| Method | Route | Description |
|---|---|---|
| GET | `/api/loans` | List all loans (active and returned) |
| GET | `/api/loans/{id}` | Get one loan |
| GET | `/api/loans/overdue` | List loans past their due date and not returned |
| GET | `/api/loans/member/{memberId}` | List all loans for one member |
| POST | `/api/loans/borrow` | Borrow a book |
| POST | `/api/loans/{id}/return` | Return a book |

### Sample: borrowing a book

```http
POST /api/loans/borrow
Content-Type: application/json

{ "bookId": 1, "memberId": 1 }
```

```json
HTTP/1.1 201 Created
Location: /api/loans/1

{
  "id": 1,
  "bookId": 1,
  "bookTitle": "Clean Code",
  "memberId": 1,
  "memberFullName": "Alice Johnson",
  "borrowedDate": "2025-08-18T23:53:31.87Z",
  "dueDate": "2025-09-01T23:53:31.87Z",
  "returnedDate": null,
  "isOverdue": false
}
```

Borrowing the same book again before it's returned:

```json
HTTP/1.1 409 Conflict
Content-Type: application/problem+json

{
  "title": "Book not available",
  "status": 409,
  "detail": "Book 1 is already on loan."
}
```

### Sample: validation error

```http
POST /api/books
Content-Type: application/json

{ "title": "", "author": "", "isbn": "", "publicationYear": 1800 }
```

```json
HTTP/1.1 400 Bad Request

{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["'Title' must not be empty."],
    "Author": ["'Author' must not be empty."],
    "Isbn": ["'Isbn' must not be empty."]
  }
}
```

## Roadmap

- [x] Project scaffolding (solution, Web API project, xUnit project, gitignore)
- [x] NuGet packages & solution folder structure
- [x] Domain entities (`Book`, `Member`, `Loan`)
- [x] `AppDbContext` & entity configuration
- [x] Initial migration, database, seed data
- [x] Service layer & DI wiring
- [x] Book service (CRUD)
- [x] Member service (CRUD)
- [x] Loan service — borrow/return business rules
- [x] DTOs & FluentValidation
- [x] Book & Member controllers
- [x] Loan controller (borrow/return/overdue)
- [x] Swagger UI & global error handling
- [x] Unit tests for business rules
- [x] Integration tests
- [x] Final cleanup & polish

## License

Personal learning/portfolio project — no license specified yet.
