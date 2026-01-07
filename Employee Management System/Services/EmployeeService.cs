using Employee_Management_System.Models.DTOs;
using Employee_Management_System.Models.Entities;
using Employee_Management_System.Repositories.Interfaces;
using Employee_Management_System.Services.Interfaces;

namespace Employee_Management_System.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository repository,
            ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            try
            {
                var employees = await _repository.GetAllAsync();
                return employees.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all employees");
                throw;
            }
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(id);
                return employee == null ? null : MapToDto(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee with id {EmployeeId}", id);
                throw;
            }
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createDto)
        {
            try
            {
                // Business logic: Check if email already exists
                if (await _repository.EmailExistsAsync(createDto.Email))
                {
                    throw new InvalidOperationException($"An employee with email '{createDto.Email}' already exists.");
                }

                var employee = MapToEntity(createDto);
                var createdEmployee = await _repository.CreateAsync(employee);
                return MapToDto(createdEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                throw;
            }
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(UpdateEmployeeDto updateDto)
        {
            try
            {
                // Business logic: Check if employee exists
                var existingEmployee = await _repository.GetByIdAsync(updateDto.Id);
                if (existingEmployee == null)
                {
                    return null;
                }

                // Business logic: Check if email is taken by another employee
                if (await _repository.EmailExistsAsync(updateDto.Email, updateDto.Id))
                {
                    throw new InvalidOperationException($"An employee with email '{updateDto.Email}' already exists.");
                }

                var employee = MapToEntity(updateDto, existingEmployee);
                var updatedEmployee = await _repository.UpdateAsync(employee);
                return MapToDto(updatedEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee with id {EmployeeId}", updateDto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee with id {EmployeeId}", id);
                throw;
            }
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            return await _repository.EmailExistsAsync(email, excludeId);
        }

        // Mapping Methods: Entity ↔ DTO
        private static EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Department = employee.Department,
                Position = employee.Position,
                DateOfBirth = employee.DateOfBirth,
                HireDate = employee.HireDate,
                Salary = employee.Salary,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }

        private static Employee MapToEntity(CreateEmployeeDto dto)
        {
            return new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Department = dto.Department,
                Position = dto.Position,
                DateOfBirth = dto.DateOfBirth,
                HireDate = dto.HireDate,
                Salary = dto.Salary
            };
        }

        private static Employee MapToEntity(UpdateEmployeeDto dto, Employee existing)
        {
            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.Email = dto.Email;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.Department = dto.Department;
            existing.Position = dto.Position;
            existing.DateOfBirth = dto.DateOfBirth;
            existing.HireDate = dto.HireDate;
            existing.Salary = dto.Salary;
            return existing;
        }
    }
}
