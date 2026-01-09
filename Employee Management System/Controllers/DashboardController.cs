using Employee_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class DashboardController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IAuthService _authService;

    public DashboardController(IEmployeeService employeeService, IAuthService authService)
    {
        _employeeService = employeeService;
        _authService = authService;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();

        ViewBag.TotalEmployees = employees.Count();
        ViewBag.TotalDepartments = await _employeeService.GetTotalDepartmentsAsync();
        ViewBag.ActiveUsers = await _authService.GetActiveUsersCountAsync();

        return View();
    }
}
