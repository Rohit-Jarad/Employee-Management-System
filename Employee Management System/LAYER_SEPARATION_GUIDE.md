# Controller, Service, and Repository - Layer Separation Guide

## 🎯 Simple Overview

Think of it like a **restaurant**:
- **Controller** = Waiter (takes order, serves food, handles customer)
- **Service** = Chef (business logic, recipes, cooking rules)
- **Repository** = Pantry/Storage (gets ingredients from storage, stores items)

---

## 📋 Responsibilities by Layer

### 1. **Controller** (Waiter)
**Role**: Handle HTTP requests and responses

**What Controllers SHOULD do:**
- ✅ Receive HTTP requests
- ✅ Validate model state (basic validation)
- ✅ Call service methods
- ✅ Return views or redirects
- ✅ Handle HTTP-specific concerns (cookies, sessions, status codes)

**What Controllers SHOULD NOT do:**
- ❌ Business logic (calculations, rules)
- ❌ Database operations
- ❌ Data transformation
- ❌ Complex validations

### 2. **Service** (Chef)
**Role**: Business logic and orchestration

**What Services SHOULD do:**
- ✅ Business rules and validations
- ✅ Data transformation (Entity ↔ DTO)
- ✅ Coordinate multiple repository calls
- ✅ Apply business logic (e.g., email uniqueness check)
- ✅ Complex calculations
- ✅ Call repositories to get/save data

**What Services SHOULD NOT do:**
- ❌ Direct database access
- ❌ HTTP concerns (cookies, sessions)
- ❌ View-specific logic

### 3. **Repository** (Pantry)
**Role**: Data access (database operations)

**What Repositories SHOULD do:**
- ✅ Query database (CRUD operations)
- ✅ Use Entity Framework Core
- ✅ Return entities or simple data
- ✅ Handle database errors

**What Repositories SHOULD NOT do:**
- ❌ Business logic
- ❌ Validation (except basic data constraints)
- ❌ HTTP concerns
- ❌ Transform data (keep it simple)

---

## 🔄 How They Work Together

```
HTTP Request
    ↓
Controller (Receives request, validates input)
    ↓
Service (Business logic, rules, transformations)
    ↓
Repository (Database operations)
    ↓
Database
```

### Example Flow: Creating an Employee

```
1. User fills form → POST /Employee/Create
   ↓
2. Controller receives CreateEmployeeDto
   - Validates ModelState
   - Calls EmployeeService.CreateEmployeeAsync()
   ↓
3. Service receives CreateEmployeeDto
   - Checks if email already exists (business rule)
   - Transforms DTO → Entity
   - Calls EmployeeRepository.CreateAsync()
   ↓
4. Repository receives Entity
   - Saves to database via DbContext
   - Returns saved Entity
   ↓
5. Service receives Entity
   - Transforms Entity → DTO
   - Returns DTO to Controller
   ↓
6. Controller receives DTO
   - Sets success message
   - Redirects to Index page
```

---

## 💻 Code Examples from Your Project

### Example 1: Creating an Employee

#### ❌ BAD: All Logic in Controller
```csharp
[HttpPost]
public async Task<IActionResult> Create(CreateEmployeeDto dto)
{
    // ❌ Business logic in controller (WRONG!)
    if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
    {
        ModelState.AddModelError("", "Email already exists");
        return View(dto);
    }
    
    // ❌ Data transformation in controller (WRONG!)
    var employee = new Employee
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        // ... mapping code
    };
    
    // ❌ Database access in controller (WRONG!)
    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();
    
    return RedirectToAction("Index");
}
```

#### ✅ GOOD: Proper Separation
**Controller** (Simple - just handles HTTP):
```csharp
[HttpPost]
public async Task<IActionResult> Create(CreateEmployeeDto createDto)
{
    if (!ModelState.IsValid)
    {
        return View(createDto);  // Basic validation
    }

    try
    {
        // ✅ Just calls service - no business logic
        var employeeDto = await _employeeService.CreateEmployeeAsync(createDto);
        TempData["SuccessMessage"] = "Employee created successfully.";
        return RedirectToAction(nameof(Index));
    }
    catch (InvalidOperationException ex)
    {
        // ✅ Handles exceptions from service
        ModelState.AddModelError("", ex.Message);
        return View(createDto);
    }
}
```

**Service** (Business logic):
```csharp
public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createDto)
{
    // ✅ Business rule: Check email uniqueness
    if (await _repository.EmailExistsAsync(createDto.Email))
    {
        throw new InvalidOperationException(
            $"An employee with email '{createDto.Email}' already exists.");
    }

    // ✅ Data transformation: DTO → Entity
    var employee = MapToEntity(createDto);
    
    // ✅ Calls repository - no direct database access
    var createdEmployee = await _repository.CreateAsync(employee);
    
    // ✅ Data transformation: Entity → DTO
    return MapToDto(createdEmployee);
}
```

**Repository** (Database operations):
```csharp
public async Task<Employee> CreateAsync(Employee employee)
{
    // ✅ Simple database operation - no business logic
    employee.CreatedAt = DateTime.Now;
    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();
    return employee;
}
```

---

### Example 2: Updating an Employee

#### Controller (HTTP handling):
```csharp
[HttpPost]
public async Task<IActionResult> Edit(int id, UpdateEmployeeDto updateDto)
{
    if (id != updateDto.Id)
    {
        return NotFound();  // HTTP concern
    }

    if (!ModelState.IsValid)
    {
        return View(updateDto);  // HTTP validation
    }

    try
    {
        // ✅ Just calls service
        var employeeDto = await _employeeService.UpdateEmployeeAsync(updateDto);
        TempData["SuccessMessage"] = "Employee updated successfully.";
        return RedirectToAction(nameof(Index));
    }
    catch (InvalidOperationException ex)
    {
        ModelState.AddModelError("", ex.Message);
        return View(updateDto);
    }
}
```

#### Service (Business logic):
```csharp
public async Task<EmployeeDto?> UpdateEmployeeAsync(UpdateEmployeeDto updateDto)
{
    // ✅ Business rule: Check if employee exists
    var existingEmployee = await _repository.GetByIdAsync(updateDto.Id);
    if (existingEmployee == null)
    {
        return null;
    }

    // ✅ Business rule: Check email uniqueness (excluding current employee)
    if (await _repository.EmailExistsAsync(updateDto.Email, updateDto.Id))
    {
        throw new InvalidOperationException(
            $"An employee with email '{updateDto.Email}' already exists.");
    }

    // ✅ Business logic: Update timestamp
    var employee = MapToEntity(updateDto, existingEmployee);
    
    // ✅ Call repository
    var updatedEmployee = await _repository.UpdateAsync(employee);
    
    // ✅ Transform back to DTO
    return MapToDto(updatedEmployee);
}
```

#### Repository (Database):
```csharp
public async Task<Employee> UpdateAsync(Employee employee)
{
    // ✅ Simple database update - no business logic
    employee.UpdatedAt = DateTime.Now;
    _context.Employees.Update(employee);
    await _context.SaveChangesAsync();
    return employee;
}
```

---

## 📊 Decision Matrix: Where Does Logic Go?

| Type of Logic | Controller | Service | Repository |
|---------------|------------|---------|------------|
| **HTTP Request/Response** | ✅ | ❌ | ❌ |
| **ModelState Validation** | ✅ | ❌ | ❌ |
| **Redirect/Action Results** | ✅ | ❌ | ❌ |
| **TempData/ViewBag** | ✅ | ❌ | ❌ |
| **Business Rules** | ❌ | ✅ | ❌ |
| **Email Uniqueness Check** | ❌ | ✅ | ❌ |
| **Data Transformation (DTO ↔ Entity)** | ❌ | ✅ | ❌ |
| **Complex Calculations** | ❌ | ✅ | ❌ |
| **Orchestrate Multiple Operations** | ❌ | ✅ | ❌ |
| **Database Queries (CRUD)** | ❌ | ❌ | ✅ |
| **SaveChanges()** | ❌ | ❌ | ✅ |
| **EF Core Operations** | ❌ | ❌ | ✅ |

---

## 🎯 Real-World Examples

### Example 1: Email Uniqueness Check

**❌ WRONG** - In Controller:
```csharp
// Controller checking email uniqueness (WRONG!)
if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
{
    ModelState.AddModelError("", "Email exists");
}
```

**✅ RIGHT** - In Service:
```csharp
// Service handles business rule
if (await _repository.EmailExistsAsync(dto.Email))
{
    throw new InvalidOperationException("Email already exists");
}
```

### Example 2: Calculate Employee Age

**❌ WRONG** - In Controller:
```csharp
// Controller calculating age (WRONG!)
var age = DateTime.Now.Year - employee.DateOfBirth.Value.Year;
ViewBag.Age = age;
```

**✅ RIGHT** - In Service or ViewModel:
```csharp
// Service calculates age (if needed in business logic)
public int CalculateAge(DateTime? dateOfBirth)
{
    if (!dateOfBirth.HasValue) return 0;
    var today = DateTime.Today;
    var age = today.Year - dateOfBirth.Value.Year;
    if (dateOfBirth.Value.Date > today.AddYears(-age)) age--;
    return age;
}
```

Or in ViewModel (if just for display):
```csharp
// ViewModel property (computed property)
public int? Age => DateOfBirth.HasValue 
    ? DateTime.Now.Year - DateOfBirth.Value.Year 
    : null;
```

### Example 3: Format Salary

**❌ WRONG** - In Controller:
```csharp
// Controller formatting (WRONG!)
ViewBag.FormattedSalary = employee.Salary?.ToString("C");
```

**✅ RIGHT** - In ViewModel or View:
```csharp
// ViewModel property
public string FormattedSalary => Salary?.ToString("C") ?? "N/A";

// Or in View (Razor)
@Model.Salary?.ToString("C")
```

---

## 🔍 Quick Reference: Common Patterns

### Pattern 1: Simple CRUD
```
Controller → Service → Repository → Database
```

### Pattern 2: Business Rule Validation
```
Controller validates ModelState
    ↓
Service checks business rules (email exists, etc.)
    ↓
Repository performs database operations
```

### Pattern 3: Data Transformation
```
Controller receives DTO from view
    ↓
Service transforms DTO → Entity
    ↓
Repository saves Entity to database
    ↓
Service transforms Entity → DTO
    ↓
Controller returns DTO to view
```

---

## ✅ Best Practices

### Controller Best Practices
1. ✅ Keep controllers thin (10-15 lines per action)
2. ✅ Only handle HTTP-specific concerns
3. ✅ Call services, don't call repositories directly
4. ✅ Handle exceptions and show user-friendly messages
5. ✅ Use TempData for success/error messages

### Service Best Practices
1. ✅ Contain all business logic
2. ✅ Validate business rules (not just data validation)
3. ✅ Transform data (DTO ↔ Entity)
4. ✅ Coordinate multiple repository calls
5. ✅ Throw meaningful exceptions

### Repository Best Practices
1. ✅ Only do database operations
2. ✅ Return entities or simple types
3. ✅ Keep methods simple and focused
4. ✅ Handle database exceptions
5. ✅ Don't contain business logic

---

## 🎓 Summary

### Controller = HTTP Handler
- Receives requests
- Validates input
- Calls services
- Returns responses

### Service = Business Logic
- Business rules
- Data transformation
- Orchestration
- Validations

### Repository = Data Access
- Database queries
- CRUD operations
- Simple data operations

### Simple Rule
**Controller asks Service, Service asks Repository, Repository asks Database**

Think: **C → S → R → DB**

---

## 📝 Your Current Implementation

Looking at your code, you've already implemented this correctly! ✅

- ✅ Controllers are thin and only handle HTTP
- ✅ Services contain business logic
- ✅ Repositories handle database operations
- ✅ Proper separation of concerns

**Your architecture is already following best practices!** 🎉
