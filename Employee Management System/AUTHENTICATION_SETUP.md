# Authentication Setup Guide

## Overview
This application uses **Cookie Authentication** - a simple and standard approach for ASP.NET Core MVC applications. Users must login to access employee management features.

## Architecture

### Components Created

1. **User Entity** (`Models/Entities/User.cs`)
   - Stores user information (FirstName, LastName, Email, PasswordHash)
   - Tracks creation time and last login

2. **User Repository** (`Repositories/UserRepository.cs`)
   - Handles database operations for users
   - Implements `IUserRepository` interface

3. **Auth Service** (`Services/AuthService.cs`)
   - Handles authentication logic
   - Password hashing using SHA256
   - User registration and login validation

4. **Account Controller** (`Controllers/AccountController.cs`)
   - Login action (GET/POST)
   - Register action (GET/POST)
   - Logout action (GET/POST)

5. **Views**
   - `Views/Account/Login.cshtml` - Login form
   - `Views/Account/Register.cshtml` - Registration form

## Setup Instructions

### 1. Run Database Migration

Since we added a new `User` entity, you need to create a migration:

```bash
dotnet ef migrations add AddUserTable
dotnet ef database update
```

This will create the `Users` table in your database.

### 2. Test the Application

1. **Start the application:**
   ```bash
   dotnet run
   ```

2. **Register a new user:**
   - Navigate to `/Account/Register`
   - Fill in the registration form
   - Submit to create an account

3. **Login:**
   - Navigate to `/Account/Login`
   - Enter your email and password
   - You'll be redirected to the home page

4. **Access Employee Management:**
   - After login, you'll see the "Employees" link in the navigation
   - Click to access employee management features
   - Without login, accessing `/Employee` will redirect to login page

5. **Logout:**
   - Click on your name in the navigation bar
   - Select "Logout"
   - You'll be logged out and redirected to login page

## Security Features

### Password Hashing
- Passwords are hashed using SHA256 before storing in database
- Plain text passwords are never stored
- Passwords are verified during login

### Protected Routes
- Employee management routes are protected with `[Authorize]` attribute
- Unauthorized users are redirected to login page
- Login and Register pages are accessible to everyone (`[AllowAnonymous]`)

### Cookie Authentication
- Uses secure cookie-based authentication
- Cookie expires after 30 minutes (or 30 days if "Remember Me" is checked)
- Sliding expiration extends cookie lifetime on activity

## How It Works

### Registration Flow
1. User fills registration form
2. System checks if email already exists
3. Password is hashed
4. User is saved to database
5. User is redirected to login page

### Login Flow
1. User enters email and password
2. System finds user by email
3. Password is verified against stored hash
4. If valid, claims are created and user is signed in
5. Cookie is set in browser
6. User is redirected to requested page or home

### Protected Route Access
1. User tries to access `/Employee`
2. `[Authorize]` attribute checks authentication
3. If not authenticated, redirect to `/Account/Login`
4. After login, redirect back to originally requested page

### Logout Flow
1. User clicks logout
2. Cookie is cleared
3. User is redirected to login page

## Database Schema

### Users Table
```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    LastLoginAt DATETIME2 NULL
);
```

## Code Examples

### Protecting a Controller
```csharp
[Authorize]
public class EmployeeController : Controller
{
    // All actions require authentication
}
```

### Allowing Anonymous Access
```csharp
[AllowAnonymous]
public IActionResult Login()
{
    return View();
}
```

### Checking Authentication in View
```csharp
@if (User.Identity!.IsAuthenticated)
{
    <p>Welcome, @User.Identity.Name!</p>
}
```

### Getting User Claims
```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
```

## Important Notes

### Password Security
The current implementation uses SHA256 for password hashing. For production applications, consider using:
- **BCrypt.Net** (recommended)
- **ASP.NET Core Identity's PasswordHasher**
- **Argon2** (for maximum security)

To upgrade to BCrypt:
1. Install package: `dotnet add package BCrypt.Net-Next`
2. Update `AuthService.cs` to use BCrypt for hashing/verification

### Session Management
- Cookies expire after 30 minutes of inactivity (or 30 days with "Remember Me")
- Users are automatically logged out when cookie expires
- Sliding expiration extends cookie on each request

### Error Handling
- Invalid login attempts show error message
- Duplicate email registration is prevented
- All errors are logged for debugging

## Troubleshooting

### Issue: Migration fails
**Solution:** Ensure Entity Framework Core Tools are installed:
```bash
dotnet tool install --global dotnet-ef
```

### Issue: Can't access Employee pages after login
**Solution:** Check that `UseAuthentication()` comes before `UseAuthorization()` in `Program.cs`

### Issue: Redirect loop on login
**Solution:** Ensure login page has `[AllowAnonymous]` attribute

### Issue: Password not working
**Solution:** Make sure you're using the correct email and password. Check database if user exists.

## Testing

### Test Cases
1. ✅ Register new user with valid data
2. ✅ Register with duplicate email (should fail)
3. ✅ Login with correct credentials
4. ✅ Login with incorrect credentials (should fail)
5. ✅ Access protected route without login (should redirect)
6. ✅ Access protected route after login (should work)
7. ✅ Logout functionality
8. ✅ "Remember Me" functionality

## Next Steps

### Optional Enhancements
1. **Email Verification** - Send verification email after registration
2. **Password Reset** - Allow users to reset forgotten passwords
3. **Role-Based Authorization** - Add roles (Admin, User, etc.)
4. **Two-Factor Authentication** - Add 2FA for enhanced security
5. **Account Lockout** - Lock account after failed login attempts
6. **Password Requirements** - Enforce stronger password policies
