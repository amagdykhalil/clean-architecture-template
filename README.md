# 🏗️ Clean Architecture Template

## 📝 Overview
This is a .NET-based clean architecture template that provides a solid foundation for building scalable and maintainable applications. It follows clean architecture principles, separating concerns into distinct layers and promoting loose coupling between components.

## 🛠️ Technologies & Libraries
- **.NET 9**
- **ASP.NET Core**
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Identity**
- **JWT Authentication**
- **MediatR**
- **Serilog with Seq**
- **Docker & Docker Compose**
- **Azure Key Vault** (optional)

## ✨ Supported Features
- **API Versioning** - Built-in support for API versioning
- **Rate Limiting** - Configured  service to control incoming requests and protect against abuse
- **JWT Authentication** - Ready-to-use JWT authentication system
- **ASP.NET Identity** - Integrated ASP.NET Identity for user management
- **Unified API Response** - Consistent response format across all endpoints
- **Global Exception Handling** - Centralized exception handling with proper error responses
- **Paginated Queries** - Base implementation for paginated data queries
- **Email Service** - Ready-to-use email service with SMTP support
- **Domain Events** - Full support for domain events
- **Unit of Work and Repository Pattern** – Clean and testable data access layer those design patterns
- **Soft Delete & Auditing** Fully configured and supported

## 📁 Solution Structure
```
src/
├── API/                 # Presentation layer
├── Core/                # Domain and Application layers
└── Infrastructure/      # Infrastructure and Persistence layer
```

## 🚀 Getting Started

### 1. Clone the Template
```bash
git clone [repository-url](https://github.com/amagdykhalil/clean-architecture-template.git)
cd clean-architecture-template
```

### 2. Rename the Solution
Run the rename script to replace the placeholder name (**SolutionName**) with your custom solution name:
```powershell
.\rename-project.ps1
```

### 3. Add Secrets

#### Option A: Use appsettings

3. 📁 In  `appsettings.json` add the following secrets:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=YourDb;User Id=sa;Password=yourStrongPassword;Encrypt=False;"
},
"JWT": {
  "Secret": "your_jwt_secret_here"
},
"SmtpSettings": {
  "Username": "your_email_user",
  "Password": "your_email_password"
}
```
> ℹ️ **Note:** The `SmtpSettings` section is **optional**


#### Option B: Using Azure Key Vault 
1. Create an Azure Key Vault
2. Add your secrets to the vault
3. Update `appsettings.json` with your vault name:
```json
{
  "KeyVault": {
    "VaultName": "your-vault-name"
  }
}
```
4.  Remove secrets from `appsettings`.

### 4. Database Setup

#### Option A: Using Visual Studio
- Run the following commands in **Package Manager Console** 
- Set the default project to `YourSolution.Persistence`
- Run the following commands:
```powershell
Add-Migration Initial
Update-Database
```

### 5. Running the Application

#### Option A: Using Visual Studio Profiles (HTTP/HTTPS)
1. Download and install [Seq](https://datalust.co/Download) for log tracking
2. Set the `YourSolution.API` project as the startup project
3. Run the application

📊 **View logs:** [http://localhost:5341](http://localhost:5341)

#### Option B: Using Docker Compose
1. Edit `docker-compose.override.yml` and set the connection string:
```yaml 
environment:
  - ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=YourDb;User Id=sa;Password=yourStrongPassword;Encrypt=False;

```
2. Enable TCP/IP in SQL Server

Required for Docker Compose to access the local SQL Server.

#### Steps:

- Open **SQL Server Configuration Manager**
	- Press `Win + R` to open the Run dialog.
	- Type the following command based on your SQL Server version:
		- **SQL Server 2022**: `SQLServerManager16.msc`
		- **SQL Server 2019**: `SQLServerManager15.msc`
		- **SQL Server 2017**: `SQLServerManager14.msc`
		- **SQL Server 2016**: `SQLServerManager13.msc` 	
- Navigate to: **SQL Server Network Configuration → Protocols for MSSQLSERVER**
- Enable **TCP/IP**
- Restart **SQL Server Service** `SQL Server (MSSQLSERVER)`

3. Run the application:
### 3. 🚀 Run the application

You have two options to run the application using Docker Compose:

##### ✅ Option A: Use the terminal

```bash
docker-compose up

```bash
docker-compose up
```
##### ✅ Option B: Use Visual Studio
- In **Solution Explorer**, right-click on the `docker-compose` project.
- Select **Set as Startup Project**.
- Run the solution.
- 
📊 **View logs:** [http://localhost::8081](http://localhost::8081)

## 📊 Logging
The application uses Serilog with Seq for structured logging. Logs can be viewed:
- Locally: [http://localhost::5341](http://localhost::5341) (when running with Visual Studio)
- Docker: [http://localhost::8081](http://localhost::8081) (when running with Docker Compose)

## 📚 API Documentation
The API documentation is available at:
`http://localhost:<port>/scalar/v1`
(where <port> corresponds to the running API port)