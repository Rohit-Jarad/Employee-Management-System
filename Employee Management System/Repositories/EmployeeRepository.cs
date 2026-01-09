using Microsoft.EntityFrameworkCore;
using Employee_Management_System.Data;
using Employee_Management_System.Models.Entities;
using Employee_Management_System.Repositories.Interfaces;

namespace Employee_Management_System.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(
            ApplicationDbContext context,
            ILogger<EmployeeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
                return await _context.Employees
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all employees");
                throw;
            }
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Employees.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee with id {EmployeeId}", id);
                throw;
            }
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee with email {Email}", email);
                throw;
            }
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            try
            {
                employee.CreatedAt = DateTime.Now;
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                throw;
            }
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            try
            {
                employee.UpdatedAt = DateTime.Now;
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return employee;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating employee with id {EmployeeId}", employee.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee with id {EmployeeId}", employee.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return false;
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee with id {EmployeeId}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Employees
                    .AnyAsync(e => e.Email == email && e.Id != excludeId.Value);
            }
            return await _context.Employees.AnyAsync(e => e.Email == email);
        }

        public async Task<(IEnumerable<Employee> Employees, int TotalCount)>
         GetPagedAsync(string? search,
                  string? sortColumn,
                  string? sortDirection,
                  int pageNumber,
                  int pageSize)
        {
            IQueryable<Employee> query = _context.Employees;

            // 🔍 SEARCH
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.FirstName.Contains(search) ||
                    e.LastName.Contains(search) ||
                    e.Email.Contains(search) ||
                    e.Department!.Contains(search) ||
                    e.Position!.Contains(search));
            }

            // 🔃 SORT
            query = (sortColumn, sortDirection?.ToLower()) switch
            {
                ("name", "desc") => query.OrderByDescending(e => e.FirstName),
                ("email", "desc") => query.OrderByDescending(e => e.Email),
                ("department", "desc") => query.OrderByDescending(e => e.Department),
                ("position", "desc") => query.OrderByDescending(e => e.Position),

                ("email", _) => query.OrderBy(e => e.Email),
                ("department", _) => query.OrderBy(e => e.Department),
                ("position", _) => query.OrderBy(e => e.Position),

                _ => query.OrderBy(e => e.FirstName)
            };

            int totalCount = await query.CountAsync();

            var employees = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (employees, totalCount);
        }
        public async Task<int> GetTotalDepartmentsAsync()
        {
            return await _context.Employees
                .Where(e => !string.IsNullOrEmpty(e.Department))
                .Select(e => e.Department)
                .Distinct()
                .CountAsync();
        }

    }
}
