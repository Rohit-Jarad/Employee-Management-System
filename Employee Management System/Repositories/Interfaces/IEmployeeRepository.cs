using Employee_Management_System.Models.Entities;

namespace Employee_Management_System.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByEmailAsync(string email);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<(IEnumerable<Employee> Employees, int TotalCount)>
        GetPagedAsync(string? search,
                  string? sortColumn,
                  string? sortDirection,
                  int pageNumber,
                  int pageSize);

        Task<int> GetTotalDepartmentsAsync();

    }

}
