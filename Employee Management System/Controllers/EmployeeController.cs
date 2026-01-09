using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Employee_Management_System.Models.DTOs;
using Employee_Management_System.Models.ViewModels;
using Employee_Management_System.Services.Interfaces;

namespace Employee_Management_System.Controllers
{
    [Authorize]
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

        public async Task<IActionResult> Index(
        string? searchTerm,
        string? sortColumn,
        string? sortDirection,
        int pageNumber = 1)
        {
            try
            {
                // 1️⃣ Get DTOs from service
                var dtoResult = await _employeeService.GetPagedEmployeesAsync(
                    searchTerm,
                    sortColumn,
                    sortDirection,
                    pageNumber,
                    5);

                // 2️⃣ Convert DTO → ViewModel
                var viewModelResult = new PagedResult<EmployeeViewModel>
                {
                    Items = dtoResult.Items.Select(MapToViewModel).ToList(),
                    PageNumber = dtoResult.PageNumber,
                    PageSize = dtoResult.PageSize,
                    TotalRecords = dtoResult.TotalRecords
                };

                ViewBag.SearchTerm = searchTerm;
                ViewBag.SortColumn = sortColumn;
                ViewBag.SortDirection = sortDirection;

                return View(viewModelResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employees");
                TempData["ErrorMessage"] = "An error occurred while loading employees.";

                return View(new PagedResult<EmployeeViewModel>());
            }
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var ws = workbook.Worksheets.Add("Employees");

            ws.Cell(1, 1).Value = "Name";
            ws.Cell(1, 2).Value = "Email";
            ws.Cell(1, 3).Value = "Department";
            ws.Cell(1, 4).Value = "Position";
            ws.Cell(1, 5).Value = "Phone";

            int row = 2;
            foreach (var e in employees)
            {
                ws.Cell(row, 1).Value = $"{e.FirstName} {e.LastName}";
                ws.Cell(row, 2).Value = e.Email;
                ws.Cell(row, 3).Value = e.Department;
                ws.Cell(row, 4).Value = e.Position;
                ws.Cell(row, 5).Value = e.PhoneNumber;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Employees.xlsx");
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
