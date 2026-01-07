# Employee Management System - Setup Guide

## Prerequisites
- .NET 8.0 SDK installed
- SQL Server (LocalDB, Express, or Full SQL Server)
- Visual Studio 2022 or VS Code

## Setup Steps

### 1. Install Required NuGet Packages

Run these commands in the Package Manager Console or Terminal:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
```

Or add to your `.csproj` file:
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
</ItemGroup>
```

### 2. Update Connection String

The connection string is already configured in `appsettings.json`. 
- For LocalDB: Already set (default)
- For SQL Server Express: Change to `Server=.\SQLEXPRESS;Database=EmployeeManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true`
- For SQL Server: Update with your server name and credentials

### 3. Create Database Migration

Run these commands in the Package Manager Console or Terminal:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This will:
- Create a migration file with the Employee table schema
- Create the database in SQL Server
- Apply the migration to create tables

### 4. Update AccountController Authentication

The `AccountController` currently has placeholder authentication logic. You need to:

**Option A: Simple Authentication (Current Implementation)**
- Already configured with Cookie Authentication
- You can enhance the login logic to check against a user database

**Option B: ASP.NET Core Identity (Recommended for Production)**
- Install: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- Create ApplicationUser model
- Update ApplicationDbContext to inherit from IdentityDbContext
- Use UserManager and SignInManager in AccountController

### 5. Create Views

You need to create the following Razor views:

**Views/Account/**
- `Login.cshtml` - Login form using LoginViewModel
- `Register.cshtml` - Registration form using RegisterViewModel

**Views/Employee/**
- `Index.cshtml` - List all employees
- `Create.cshtml` - Create new employee form
- `Edit.cshtml` - Edit employee form
- `Details.cshtml` - Employee details view
- `Delete.cshtml` - Delete confirmation view

**Views/Shared/** (Optional updates)
- Update `_Layout.cshtml` to include login/logout links
- Add navigation menu for Employee management

### 6. Run the Application

```bash
dotnet run
```

Or press F5 in Visual Studio.

### 7. Test the Application

1. Navigate to `/Account/Register` to create a user account
2. Navigate to `/Account/Login` to login
3. After login, access `/Employee` to manage employees

## Current Project Structure

```
├── Controllers/
│   ├── AccountController.cs    ✓ Created (needs authentication implementation)
│   ├── EmployeeController.cs   ✓ Created
│   └── HomeController.cs       ✓ Existing
│
├── Models/
│   ├── Employee.cs             ✓ Created
│   ├── ViewModels/
│   │   ├── LoginViewModel.cs   ✓ Created
│   │   └── RegisterViewModel.cs ✓ Created
│   └── ErrorViewModel.cs       ✓ Existing
│
├── Services/
│   ├── Interfaces/
│   │   └── IEmployeeService.cs ✓ Created
│   └── EmployeeService.cs      ✓ Created
│
├── Data/
│   └── ApplicationDbContext.cs ✓ Created
│
└── Program.cs                  ✓ Updated with DI and Authentication
```

## Next Steps

1. **Implement Authentication Logic**: Update AccountController with actual user validation
2. **Create Views**: Build the Razor views for all controllers
3. **Add Validation**: Enhance client-side validation
4. **Add Search/Filter**: Implement employee search functionality
5. **Add Pagination**: For large employee lists
6. **Style the UI**: Improve the user interface with Bootstrap or custom CSS

## Troubleshooting

### Migration Errors
- Ensure SQL Server is running
- Check connection string in `appsettings.json`
- Verify Entity Framework Tools are installed

### Authentication Issues
- Ensure `UseAuthentication()` comes before `UseAuthorization()` in Program.cs
- Check cookie settings in authentication configuration

### Service Not Found Errors
- Verify all services are registered in `Program.cs`
- Check that service interfaces match implementations

## Additional Resources

- [ASP.NET Core MVC Documentation](https://docs.microsoft.com/aspnet/core/mvc/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Authentication](https://docs.microsoft.com/aspnet/core/security/authentication/)
