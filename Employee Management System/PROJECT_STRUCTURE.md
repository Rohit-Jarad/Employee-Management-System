# Employee Management System - Project Structure

## Recommended Folder Structure

```
Employee Management System/
│
├── Controllers/                    # MVC Controllers (HTTP Request Handlers)
│   ├── AccountController.cs       # Authentication & Authorization
│   ├── EmployeeController.cs      # Employee Management Operations
│   └── HomeController.cs          # Home Page
│
├── Models/                         # All Model Classes
│   ├── Entities/                  # Database Entities (EF Core Models)
│   │   └── Employee.cs           # Employee entity mapped to database
│   │
│   ├── DTOs/                      # Data Transfer Objects (Between Layers)
│   │   ├── EmployeeDto.cs        # Employee DTO for service layer
│   │   ├── CreateEmployeeDto.cs  # DTO for creating employees
│   │   └── UpdateEmployeeDto.cs  # DTO for updating employees
│   │
│   └── ViewModels/                # View Models (For Views)
│       ├── EmployeeViewModel.cs   # Employee view model
│       ├── LoginViewModel.cs      # Login view model
│       └── RegisterViewModel.cs   # Register view model
│
├── Repositories/                   # Data Access Layer
│   ├── Interfaces/                # Repository Interfaces
│   │   └── IEmployeeRepository.cs
│   │
│   └── EmployeeRepository.cs      # Repository Implementation
│
├── Services/                       # Business Logic Layer
│   ├── Interfaces/                # Service Interfaces
│   │   └── IEmployeeService.cs
│   │
│   └── EmployeeService.cs         # Service Implementation
│
├── Data/                           # Data Context
│   └── ApplicationDbContext.cs    # EF Core DbContext
│
├── Views/                          # Razor Views
│   ├── Home/
│   ├── Account/
│   └── Employee/
│
├── wwwroot/                        # Static Files
│
└── Program.cs                      # Application Entry Point
```

## Architecture Layers & Data Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│  Controllers → ViewModels → Views                            │
└───────────────────────┬─────────────────────────────────────┘
                        │ Uses DTOs
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                   Business Logic Layer                       │
│  Services (Business Rules, Validation, Transformation)       │
└───────────────────────┬─────────────────────────────────────┘
                        │ Uses Entities
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                    Data Access Layer                         │
│  Repositories (CRUD Operations)                              │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                      Database Layer                          │
│  ApplicationDbContext → SQL Server                           │
└─────────────────────────────────────────────────────────────┘
```

## Responsibilities by Layer

### 1. **Controllers**
- Handle HTTP requests/responses
- Validate model state
- Call services
- Return views or redirects
- **Do NOT**: Direct database access, business logic

### 2. **Services**
- Business logic and validation
- Data transformation (Entity ↔ DTO)
- Orchestrate repository operations
- Handle business rules
- **Do NOT**: Direct database access, HTTP concerns

### 3. **Repositories**
- Database operations (CRUD)
- Query data using EF Core
- Return entities
- **Do NOT**: Business logic, HTTP concerns

### 4. **DTOs (Data Transfer Objects)**
- Transfer data between layers
- Lightweight, no business logic
- Protect domain entities from exposure
- Can combine data from multiple entities

### 5. **Entities**
- Database table representations
- EF Core navigation properties
- Minimal validation (only data constraints)

### 6. **ViewModels**
- Optimized for views
- May contain data from multiple DTOs/Entities
- Display-specific formatting
- Validation attributes for forms

## Benefits of This Structure

1. **Separation of Concerns**: Each layer has a single responsibility
2. **Maintainability**: Easy to locate and modify code
3. **Testability**: Each layer can be tested independently
4. **Scalability**: Easy to add new features
5. **Reusability**: Services and repositories can be reused
6. **Security**: Entities are not directly exposed to controllers/views

## Common Patterns Used

- **Repository Pattern**: Abstracts data access
- **DTO Pattern**: Transfers data between layers
- **Service Pattern**: Encapsulates business logic
- **Dependency Injection**: Loose coupling between layers
