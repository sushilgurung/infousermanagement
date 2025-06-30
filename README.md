
# Info Home Technical Challenge - User Management

This project is an updated and extended version of the original [Inflosoftware TechTest](https://github.com/inflosoftware/TechTest), now migrated from **.NET 7** to **.NET 9**, using **SQL Server 2022** and a **Clean Architecture** pattern. It allows for user CRUD operations with enhanced logging, validation, and scalability support.


# 1. Filters Section (Standard)
**Original Requirement:**
The users page should include three buttons below the user list to filter users by their active status:

- Show All (already implemented)

- Active Only (show users with IsActive = true)

- Non Active (show users with IsActive = false)

### What I implemented:

- Replaced the three separate buttons with a single dropdown filter control for a cleaner and more user-friendly interface.

- The dropdown includes options: Show All, Active Only, and Non Active.

- Selecting an option filters the user list dynamically based on the IsActive property.

- Added pagination to the user list to improve usability and performance when displaying large numbers of users.

- All filtering and pagination logic is performed in the Blazor WebAssembly frontend, ensuring a responsive user experience.


# 2. User Model Properties (Standard)
**Original Requirement:**
- Add a new property to the User class called DateOfBirth which should be used and displayed in relevant sections of the app.

### What I implemented:

- Added the DateOfBirth property to the User entity/model.

- Updated the Blazor UI to support adding, editing, viewing, and listing users with their DateOfBirth.

- Ensured DateOfBirth is displayed appropriately on the user list and detail pages with proper formatting.

- Added validation for DateOfBirth where necessary (e.g., required or valid date).

# 3. Actions Section (Standard)
**Original Requirement:**
Create the code and UI flows for the following user actions with appropriate data validation communicated to the end user:

- Add – Screen to create a new user and return to the list

- View – Screen to display user details

- Edit – Screen to modify an existing user

- Delete – Screen to remove a user from the list

### What I implemented:

- Developed full CRUD operations (Create, Read, Update, Delete) for users.

- Implemented client-side validation in Blazor for immediate feedback.

- Added server-side validation to ensure data integrity and security.

- Provided clear validation messages and UI feedback on all relevant screens.

- Ensured smooth navigation flows between list and detail/edit pages.

# 4. Data Logging (Advanced)
**Original Requirement:**
Extend the system to capture log information about primary actions performed on each user.

- The user View screen should display a list of all actions performed against that user.

- A new Logs page should list all log entries across the application.

- From the Logs page, users should be able to click on entries to view detailed information.

- The Logs page should be designed to provide a good user experience even with many entries.

### What I implemented:

- Implemented user action logging that records each primary user action (Add, Edit, Delete, View) into the database.

- Created a dedicated Logs page to display a paginated and searchable list of all user action logs across the system.

- On the Logs page, users can click any log entry to view detailed information.

- Integrated the user action logs display on individual user View screens.

- Added pagination to efficiently handle large volumes of log data, ensuring fast load times and responsive UI.

# 5. Extend the Application (Expert)
**Original Requirement:**
Make a significant architectural improvement to enhance maintainability, scalability, or testability. Suggested ideas include:

- Re-implement the UI using a client-side framework connected to an API (Blazor preferred)

- Support asynchronous operations in the data access layer

- Implement authentication and login based on stored users

- Implement bundling of static assets

- Use a real database with schema migrations

### What I implemented:

- Rebuilt the UI using Blazor WebAssembly, providing a modern, responsive single-page application experience.

- Developed backend endpoints using both ASP.NET Core REST API Controllers and Minimal APIs for lightweight, flexible routing.

- Employed SQL Server 2022 as the primary database, leveraging Entity Framework Core (Code-First) for data access and schema migrations.

- Implemented the Repository Pattern to abstract data access and improve testability and maintainability.

- Added asynchronous support in all data access operations to improve scalability and responsiveness.

- Built a login page with full authentication and authorisation flows using JWT Bearer tokens, securing API endpoints and Blazor routes.

- Applied authorisation policies to protect sensitive data and actions based on user roles or claims.

# 6. Future-Proof the Application (Platform)
**Original Requirement:**
Add additional architectural layers to ensure scalability for many users and developers. Examples include:

- CI pipelines for automated testing and build

- CD pipelines for automated deployment to the cloud

- Infrastructure as Code (IaC) for environment provisioning

- Message bus or background workers for long-running operations

- What I implemented:

- Adopted Test-Driven Development (TDD) to ensure code quality and maintainability.

- Created comprehensive unit tests, integration tests, and Blazor client-side component tests (using bunit).

- Built and configured CI/CD pipelines using GitHub Actions to automate building, testing, and deploying the application.

- Hosted backend API on Azure App Service, frontend Blazor WebAssembly on Azure Static Web Apps, and database on Azure SQL Server.

- Integrated Azure Service Bus Queue to asynchronously save user logs, improving scalability and reliability.


### Live Demo
Frontend (Blazor WebAssembly UI):
https://white-plant-0ba158a03.6.azurestaticapps.net/

Backend API:
https://inflousermanagement-c2a7gtbkejgndegg.uksouth-01.azurewebsites.net/index.html

Note: These are deployed on free-tier Azure services, so availability may be intermittent.


## 🔧 Technology Stack

| Layer          | Tech Used                                      |
|----------------|------------------------------------------------|
| Backend API    | ASP.NET Core 9 Web API using both Controllers and Minimal APIs |
| Frontend       | Blazor WebAssembly (.NET 9)                    |
| Architecture   | Clean Architecture + CQRS                      |
| ORM            | Entity Framework Core 9                        |
| Database       | SQL Server 2022                                |
| Validation     | FluentValidation                               |
| Containerisation | Docker, Docker Compose, NGINX                 |
| Auth (optional)| JWT Bearer Tokens                              |
| Documentation  | Swagger UI                                     |

---


## 📂 Project Structure 
 Clean Architecture 
``` 
/src
  /presentation
    - BlazorClient
  /Domain
    - Entities
    - Interfaces
    - Specifications
  /Application
    - Interfaces
    - Services
    - Commands
    - Queries
    - DTOs
  /Infrastructure
    - Data 
    - Services 
  /WebAPI
    /Api
      - Controllers
    - Program.cs 
/tests
  /UserManagement.BlazorClient.UnitTests
  /UserManagement.Data.Tests
  /UserManagement.IntegrationTesting
  /UserManagement.UnitTests
```

## 🚀 Getting Started

### ✅ Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

| Package                                                     | Purpose                                               |
| ----------------------------------------------------------- | ----------------------------------------------------- |
| `Microsoft.AspNetCore.OpenApi` (9.0.6)                      | OpenAPI / Swagger integration                         |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (9.0.6) | Identity integration with EF Core                     |
| `Microsoft.EntityFrameworkCore.SqlServer` (9.0.6)           | EF Core provider for SQL Server                       |
| `Microsoft.EntityFrameworkCore.Design` (9.0.6)              | Design-time EF Core tools for migrations              |
| `Microsoft.EntityFrameworkCore.Tools` (9.0.6)               | EF Core CLI tooling                                   |
| `Microsoft.CodeAnalysis.Common` (4.11.0)                    | Roslyn compiler APIs                                  |
| `Microsoft.CodeAnalysis.CSharp.Workspaces` (4.11.0)         | C# Workspace APIs for code analysis                   |
| `Bogus` (35.6.3)                                            | Fake data generator for testing/seeding               |
| `Azure.Messaging.ServiceBus` (7.18.2)                       | Azure Service Bus SDK for messaging                   |
| `Carter` (9.0.0)                                            | Minimal API extension library                         |
| `FluentValidation.DependencyInjectionExtensions` (11.11.0)  | Validation framework integration                      |
| `Gurung.RepositoryPattern` (9.0.0)                          | Custom repository pattern library                     |
| `Gurung.ServicesRegister` (1.0.0)                           | Custom service registration utility                   |
| `MediatR` (12.4.1)                                          | CQRS and mediator pattern implementation              |
| `Serilog.AspNetCore` (9.0.0)                                | Structured logging for ASP.NET Core                   |
| `System.IdentityModel.Tokens.Jwt` (8.0.1)                   | JWT token handling                                    |
| `Microsoft.AspNetCore.Authentication.JwtBearer` (9.0.2)     | JWT Bearer Authentication middleware                  |
| `Swashbuckle.AspNetCore` (9.0.1)                            | Swagger/OpenAPI documentation generation              |
| `bunit` (1.40.0)                                            | Blazor component testing framework                    |
| `coverlet.collector` (6.0.2)                                | Code coverage collector for tests                     |
| `Microsoft.NET.Test.Sdk` (17.12.0)                          | Test SDK for running unit and integration tests       |
| `Moq` (4.20.72)                                             | Mocking framework for unit testing                    |
| `xunit` (2.9.2)                                             | Unit testing framework                                |
| `xunit.runner.visualstudio` (2.8.2)                         | xUnit Visual Studio test runner integration           |
| `FluentAssertions` (8.4.0)                                  | Fluent syntax for asserting unit tests                |
| `Microsoft.AspNetCore.Mvc.Testing` (9.0.6)                  | Integration testing helpers for ASP.NET Core          |
| `Testcontainers.MsSql` (4.6.0)                              | Dockerized SQL Server test containers for integration |

---



##  Getting Started

To run the project locally, please follow these steps:

1. **Configure Azure Service Bus**  
   Add your Service Bus connection string in the `appsettings.json` or `appsettings.Development.json` file as:  
   ```json
   "ServiceBusConnectionString": "Your_Azure_Service_Bus_Connection_String"
2. **Set up SQL Server Connection**
Update the database connection string in the same settings file to point to your SQL Server instance:

``` json
"ConnectionStrings": {
    "DefaultConnection": "Your_SQL_Server_Connection_String"
}
```

3. **Run the Application**
You can run both the Web API and the Blazor WebAssembly client projects separately via your IDE (Visual Studio or VS Code) or use the CLI commands:

bash
Copy
Edit
dotnet run --project WebAPI
dotnet run --project BlazorClient


# 🙏 Thank You
Thank you for taking the time to review this project. Your feedback and suggestions are greatly appreciated.

