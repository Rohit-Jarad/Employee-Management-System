using Employee_Management_System.Models.DTOs;
using Employee_Management_System.Models.ViewModels;

namespace Employee_Management_System.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createDto);
        Task<EmployeeDto?> UpdateEmployeeAsync(UpdateEmployeeDto updateDto);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<bool> EmployeeExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<PagedResult<EmployeeDto>> GetPagedEmployeesAsync(
        string? search,
        string? sortColumn,
        string? sortDirection,
        int pageNumber,
        int pageSize);
        Task<int> GetTotalDepartmentsAsync();

    }
}
