namespace Employee_Management_System.Models.ViewModels
{
    public class EmployeeQueryParameters
    {
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; } = "asc";

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
