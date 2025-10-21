# SimpleTradingApp

A simple trading application built with .NET 9 and Clean Architecture principles. This application allows users to manage trading accounts and execute buy/sell trades for securities.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
SimpleTradingApp/
├── SimpleTradingApp.Domain/          # Core business entities and rules
│   └── Entities/                     # Account, Trade entities
├── SimpleTradingApp.Application/     # Application logic and services
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Services/                    # Business logic implementation
│   ├── ServiceContracts/            # Service interfaces
│   ├── IRepositories/               # Repository interfaces
│   ├── Validators/                  # Input validation
│   └── Mappers/                     # Object mapping
├── SimpleTradingApp.Infrastructure/  # Data persistence layer
│   ├── Repositories/                # Repository implementations
│   └── Migrations/                  # Database migrations
├── SimpleTradingApp.Api/            # REST API layer
│   ├── Controllers/                 # API controllers
│   └── Middleware/                  # Custom middleware
└── *.Tests/                        # Unit and integration tests
```

## 🚀 Features

- **Account Management**: Create, update, and delete trading accounts
- **Trade Execution**: Place buy/sell orders for securities
- **Trade Status Management**: Update trade status (Placed, Executed, Expired)
- **Data Persistence**: PostgreSQL database with Entity Framework Core
- **API Documentation**: Swagger/OpenAPI integration
- **Input Validation**: FluentValidation for request validation
- **Error Handling**: Global exception middleware
- **Testing**: Comprehensive unit and integration tests

## 🛠️ Technology Stack

- **Framework**: .NET 9
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 9.0
- **API**: ASP.NET Core Web API
- **Validation**: FluentValidation
- **Documentation**: Swagger/Swashbuckle
- **Testing**: xUnit
- **Containerization**: Docker support

## 📋 Prerequisites

1. **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Docker** - [Download here](https://www.docker.com/products/docker-desktop)
3. **Entity Framework Tools**:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

## ⚙️ Setup Instructions

### 1. Clone the Repository
```bash
git clone <repository-url>
cd SimpleTradingApp
```

### 2. Setup PostgreSQL Database

Pull and run PostgreSQL container:
```bash
# Pull PostgreSQL image
docker pull postgres:latest

# Run PostgreSQL container
docker run --name simpletrading-postgres \
  -p 5432:5432 \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=password123 \
  -e POSTGRES_DB=SimpleTradingDb \
  -d postgres:latest
```

### 3. Configure User Secrets

Navigate to the API project and set up user secrets:
```bash
cd SimpleTradingApp.Api

# Set database connection secrets
dotnet user-secrets set "POSTGRES_USER" "postgres"
dotnet user-secrets set "POSTGRES_PASSWORD" "password123"
dotnet user-secrets set "POSTGRES_PORT" "5432"
```

### 4. Run Database Migrations

The application will automatically apply migrations on startup, or you can run them manually:
```bash
# From the root directory
dotnet ef database update --project SimpleTradingApp.Infrastructure --startup-project SimpleTradingApp.Api
```

### 5. Build and Run

```bash
# Build the solution
dotnet build

# Run the API
dotnet run --project SimpleTradingApp.Api
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger`

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test SimpleTradingApp.Domain.Tests
dotnet test SimpleTradingApp.Application.Tests
dotnet test SimpleTradingApp.Infrastructure.Tests
dotnet test SimpleTradingApp.Api.Tests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📡 API Endpoints

### Accounts
- `POST /api/accounts` - Create a new trading account
- `PUT /api/accounts/{accountId}` - Update an existing account
- `DELETE /api/accounts/{accountId}` - Delete an account
- `GET /api/accounts/{accountId}` - Get account details
- `GET /api/accounts` - Get all accounts

### Trades
- `POST /api/trades` - Place a new trade
- `PUT /api/trades/{tradeId}/status` - Update trade status

### Example Requests

**Create Account:**
```json
POST /api/accounts
{
  "firstName": "John",
  "lastName": "Doe"
}
```

**Place Trade:**
```json
POST /api/trades
{
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "securityCode": "AAPL",
  "amount": 100.50,
  "type": "Buy"
}
```

**Update Trade Status:**
```json
PUT /api/trades/{tradeId}/status
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Executed"
}
```

## 🏢 Domain Models

### Account
- `ID` (Guid)
- `FirstName` (string)
- `LastName` (string)
- `Trades` (Collection)

### Trade
- `ID` (Guid)
- `AccountId` (Guid)
- `SecurityCode` (string)
- `Timestamp` (DateTime)
- `Amount` (decimal)
- `Type` (Buy/Sell)
- `Status` (Placed/Executed/Expired)

## 🐳 Docker Support

The application includes Docker support. You can build and run the entire application using Docker:

```bash
# Build Docker image
docker build -t simpletrading-api .

# Run the application
docker run -p 8080:80 simpletrading-api
```

## 🔧 Configuration

Application settings can be configured in:
- `appsettings.json` - Default settings
- `appsettings.Development.json` - Development environment
- User Secrets - Sensitive data (database credentials)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add/update tests
5. Ensure all tests pass
6. Submit a pull request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## 🚨 Important Notes

- **Database Migrations**: The application automatically applies migrations on startup. For production, consider running migrations separately in your CI/CD pipeline.
- **Security**: User secrets are used for development. For production, use appropriate secret management solutions.
- **Error Handling**: The application includes global exception handling middleware for consistent error responses.