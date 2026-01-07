using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Employee_Management_System.Models.DTOs;
using Employee_Management_System.Models.ViewModels;
using Employee_Management_System.Services.Interfaces;

namespace Employee_Management_System.Controllers
{
    [Authorize] // Require authentication to access employee management
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(
            IEmployeeService employeeService,
            ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            try
            {
                var employeeDtos = await _employeeService.GetAllEmployeesAsync();
                var viewModels = employeeDtos.Select(MapToViewModel);
                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employees");
                TempData["ErrorMessage"] = "An error occurred while loading employees.";
                return View(new List<EmployeeViewModel>());
            }
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var employeeDto = await _employeeService.GetEmployeeByIdAsync(id.Value);
                if (employeeDto == null)
                {
                    return NotFound();
                }

                var viewModel = MapToViewModel(employeeDto);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee details");
                TempData["ErrorMessage"] = "An error occurred while retrieving employee details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View(new CreateEmployeeDto());
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return View(createDto);
            }

            try
            {
                var employeeDto = await _employeeService.CreateEmployeeAsync(createDto);
                TempData["SuccessMessage"] = "Employee created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                ModelState.AddModelError("", "Unable to create employee. Please try again.");
                return View(createDto);
            }
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var employeeDto = await _employeeService.GetEmployeeByIdAsync(id.Value);
                if (employeeDto == null)
                {
                    return NotFound();
                }

                var updateDto = MapToUpdateDto(employeeDto);
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee for edit");
                TempData["ErrorMessage"] = "An error occurred while retrieving employee details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateEmployeeDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(updateDto);
            }

            try
            {
                var employeeDto = await _employeeService.UpdateEmployeeAsync(updateDto);
                if (employeeDto == null)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Employee updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee");
                ModelState.AddModelError("", "Unable to update employee. Please try again.");
                return View(updateDto);
            }
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var employeeDto = await _employeeService.GetEmployeeByIdAsync(id.Value);
                if (employeeDto == null)
                {
                    return NotFound();
                }

                var viewModel = MapToViewModel(employeeDto);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee for delete");
                TempData["ErrorMessage"] = "An error occurred while retrieving employee details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _employeeService.DeleteEmployeeAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Employee deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Employee not found or could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee");
                TempData["ErrorMessage"] = "An error occurred while deleting the employee.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Mapping Methods: DTO ↔ ViewModel
        private static EmployeeViewModel MapToViewModel(EmployeeDto dto)
        {
            return new EmployeeViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Department = dto.Department,
                Position = dto.Position,
                DateOfBirth = dto.DateOfBirth,
                HireDate = dto.HireDate,
                Salary = dto.Salary,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        private static UpdateEmployeeDto MapToUpdateDto(EmployeeDto dto)
        {
            return new UpdateEmployeeDto
            {
                Id = dto.Id,
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
    }
}
