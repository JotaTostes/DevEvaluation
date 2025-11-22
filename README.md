# Developer Evaluation - Sales API

A .NET 8 REST API for managing sales records, built with DDD, Clean Architecture, and CQRS patterns.

## Tech Stack

- .NET 8
- PostgreSQL
- Entity Framework Core
- MediatR (CQRS)
- AutoMapper
- FluentValidation
- Rebus (Event Publishing)
- Serilog (Logging)
- xUnit + FluentAssertions + NSubstitute + Bogus (Testing)

## Prerequisites

- .NET 8 SDK
- PostgreSQL 14+
- Docker (optional)

## Configuration

### 1. Database Connection

Update `appsettings.json` in the WebApi project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=DeveloperEvaluation;Username=postgres;Password=postgres"
  }
}
```

### 2. Using Docker (Optional)

```bash
docker-compose up -d
```

This starts PostgreSQL on port 5432.

## Running the Application

### 1. Apply Migrations

```bash
cd src/Ambev.DeveloperEvaluation.ORM
dotnet ef database update --startup-project ../Ambev.DeveloperEvaluation.WebApi
```

### 2. Run the API

```bash
cd src/Ambev.DeveloperEvaluation.WebApi
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5119`
- HTTPS: `https://localhost:7181`
- Swagger: `https://localhost:7181/swagger`

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/sales` | Create a sale |
| GET | `/api/sales/{id}` | Get sale by ID |
| GET | `/api/sales` | List sales (with filters) |
| PUT | `/api/sales/{id}` | Update a sale |
| PATCH | `/api/sales/{id}/cancel` | Cancel a sale |
| PATCH | `/api/sales/{id}/items/{itemId}/cancel` | Cancel an item |
| DELETE | `/api/sales/{id}` | Delete a sale |
| GET | `/health` | Health check |

### Query Parameters (GET /api/sales)

| Parameter | Type | Description |
|-----------|------|-------------|
| pageNumber | int | Page number (default: 1) |
| pageSize | int | Items per page (default: 10, max: 100) |
| customerId | string | Filter by customer |
| branchId | string | Filter by branch |
| startDate | datetime | Filter by start date |
| endDate | datetime | Filter by end date |
| status | int | 1 = Active, 2 = Cancelled |
| orderBy | string | SaleNumber, SaleDate, Customer, Branch, TotalAmount, Status |
| ascending | bool | Sort direction (default: false) |

## Business Rules

| Quantity | Discount |
|----------|----------|
| 1-3 items | 0% |
| 4-9 items | 10% |
| 10-20 items | 20% |
| >20 items | Not allowed |

## Running Tests

```bash
# All tests
dotnet test

# Unit tests only
dotnet test tests/Ambev.DeveloperEvaluation.Unit

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Project Structure

```
src/
├── Adapters/
│   ├── Driven/
│   │   └── Ambev.DeveloperEvaluation.ORM        # EF Core, Repositories
│   └── Drivers/
│       └── Ambev.DeveloperEvaluation.WebApi     # Controllers, Middleware
├── Core/
│   ├── Ambev.DeveloperEvaluation.Application    # Commands, Queries, Handlers
│   └── Ambev.DeveloperEvaluation.Domain         # Entities, Events, Interfaces
└── Crosscutting/
    ├── Ambev.DeveloperEvaluation.Common         # Logging, Security
    └── Ambev.DeveloperEvaluation.IoC            # Dependency Injection

tests/
├── Ambev.DeveloperEvaluation.Unit               # Unit tests
├── Ambev.DeveloperEvaluation.Integration        # Integration tests
└── Ambev.DeveloperEvaluation.Functional         # E2E tests
```

## Sample Requests

### Create Sale

```bash
curl -X POST https://localhost:7181/api/sales \
  -H "Content-Type: application/json" \
  -d '{
    "saleNumber": "SALE-001",
    "saleDate": "2024-01-15T10:30:00Z",
    "customerId": "CUST-001",
    "customerName": "John Doe",
    "branchId": "BRANCH-001",
    "branchName": "Downtown Store",
    "items": [
      {
        "productId": "PROD-001",
        "productName": "Brahma 350ml",
        "quantity": 5,
        "unitPrice": 4.50
      }
    ]
  }'
```

### Get Sales with Filters

```bash
curl "https://localhost:7181/api/sales?pageNumber=1&pageSize=10&customerId=CUST-001&orderBy=SaleDate&ascending=false"
```

## Events

The API publishes domain events (logged by default, can be configured for message brokers):

- `SaleCreatedEvent`
- `SaleModifiedEvent`
- `SaleCancelledEvent`
- `ItemCancelledEvent`

Configure Rebus in `appsettings.json`:

```json
{
  "Rebus": {
    "Enabled": true,
    "QueueName": "sales-events"
  }
}
```

## License

MIT
