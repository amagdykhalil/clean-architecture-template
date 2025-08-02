# 🏗️ Clean Architecture Template

## 📝 Overview

This is a .NET-based clean architecture template that provides a solid foundation for building scalable and maintainable applications. It follows clean architecture principles, separating concerns into distinct layers and promoting loose coupling between components.

## 🛠️ Technologies & Libraries

- **.NET 9** - Latest .NET framework
- **ASP.NET Core** - Web framework
- **Entity Framework Core** - ORM for data access
- **SQL Server** - Database engine
- **ASP.NET Identity** - User management system
- **JWT Authentication** - Token-based authentication
- **MediatR** - CQRS and mediator pattern implementation
- **Serilog with Seq** - Structured logging
- **Docker & Docker Compose** - Containerization
- **Azure Key Vault** - Secure secret management (optional)

## ✨ Supported Features

- **API Versioning** - Built-in support for API versioning
- **Rate Limiting** - Configured service to control incoming requests and protect against abuse
- **JWT Authentication** - Ready-to-use JWT authentication system
- **Authentication & Authorization** - Complete auth system with email confirmation and password reset, ...
- **ASP.NET Identity** - Integrated ASP.NET Identity for user management
- **Unified API Response** - Consistent response format across all endpoints
- **Global Exception Handling** - Centralized exception handling with proper error responses
- **Paginated Queries** - Base implementation for paginated data queries
- **Email Service** - Ready-to-use email service with SMTP support
- **Unit of Work and Repository Pattern** – Clean and testable data access layer those design patterns
- **Soft Delete & Auditing** Fully configured and supported
- **Comprehensive Testing** - Unit tests, integration tests, and architecture tests

## 📁 Solution Structure

```sh

  ├── SolutionName.sln
  ├── README.md
  ├── docker-compose.dcproj
  ├── docker-compose.override.yml # Docker Compose override for local development
  ├── docker-compose.yml
  ├── launchSettings.json
  ├── src
  │   ├── API                  # ASP.NET Core Web API project
  │   ├── Core
  │   │   ├── SolutionName.Application  # Application layer: business logic, CQRS, MediatR, validation
  │   │   └── SolutionName.Domain       # Domain layer: entities, enums, core business models
  │   └── Infrastructure
  │       ├── SolutionName.Infrastructure # Infrastructure services: email, authentication
  │       └── SolutionName.Persistence    # Persistence layer: Entity Framework, repositories, migrations
  │   ├── SolutionName.Shared           # Shared  email templates
  └── tests
      ├── SolutionName.IntegrationTests  # Integration tests for infrastructure and persistence layers
      ├── SolutionName.Tests.Common      # Shared test utilities and data generators
      └── SolutionName.UnitTests
```

## 🚀 Quick Start

### 1. Clone and Setup

```bash
git clone https://github.com/amagdykhalil/clean-architecture-template.git
cd clean-architecture-template
```

### 2. Customize Project Name

Execute the rename script to replace **SolutionName** with your project name:

```powershell
.\rename-project.ps1
```

### 3. Configure Application Settings

#### Option A: Using appsettings

📁 Configure `appsettings.json` with your secrets:

```json
"JwtSettings": {
  "Secret": "your_jwt_secret_here"
},
"SmtpSettings": {
  "Username": "your_email_user",
  "Password": "your_email_password",
  "FromEmail": "example@gmail.com"
}
```

📁 Configure `appsettings.Development.json` with your database connection:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=YourDb;User Id=sa;Password=yourStrongPassword;Encrypt=False;"
}
```

> ℹ️ **Note:** The `SmtpSettings` section is **optional**

#### Option B: Using Azure Key Vault

1. Create an Azure Key Vault
2. Add your secrets to the vault
3. Update `appsettings.json` with your vault configuration:

```json
{
  "KeyVault": {
    "VaultName": "your-vault-name"
  }
}
```

4. Remove sensitive configuration from `appsettings.json`

### 4. Database Setup

Run the following commands to set up the database:

```bash
# Navigate to the Persistence project
cd src/Infrastructure/SolutionName.Persistence

# Add initial migration
dotnet ef migrations add Initial

# Update database
dotnet ef database update
```

### 5. Running the Application

#### Option A: Using .NET CLI

1. Download and install [Seq](https://datalust.co/Download) for log tracking
2. Navigate to the API project and run:

```bash
cd src/API/SolutionName.API
dotnet run
```

📊 **View logs:** [http://localhost:5341](http://localhost:5341)

#### Option B: Using Docker Compose

1. Edit `docker-compose.override.yml` and set the connection string:

```yaml
environment:
  - ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=YourDb;User Id=sa;Password=yourStrongPassword;Encrypt=False;
```

2. Enable TCP/IP in SQL Server

Required for Docker Compose to access the local SQL Server.

- Open SQL Server Configuration Manager
- Navigate to: **SQL Server Network Configuration → Protocols for MSSQLSERVER**
- Enable **TCP/IP**
- Restart **SQL Server Service** `SQL Server (MSSQLSERVER)`

3. **Run the application:**

```bash
docker-compose up --build
```


📊 **View logs:** [http://localhost:8081](http://localhost:8081)

## 📊 Logging

The application uses Serilog with Seq for structured logging. Logs can be viewed:

- Local Development: [http://localhost:5341](http://localhost:5341)
- Docker Environment: [http://localhost:8081](http://localhost:8081)

## 📚 API Documentation

The API documentation is available at:
`https://localhost:<port>/scalar/v1`
(where <port> corresponds to the running API port)
