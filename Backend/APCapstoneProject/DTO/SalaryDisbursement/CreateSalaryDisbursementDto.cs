using System.ComponentModel.DataAnnotations;

public class CreateSalaryDisbursementDto : IValidatableObject
{
    public bool AllEmployees { get; set; } = false;

    public int? EmployeeId { get; set; }

    public List<int>? EmployeeIds { get; set; }

    [MaxLength(200)]
    public string? Remarks { get; set; }

    public IFormFile? CsvFile { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Normalize empty values
        if (EmployeeIds != null && EmployeeIds.Count == 0)
            EmployeeIds = null;

        // Case 1: CSV upload overrides everything
        if (CsvFile != null)
            yield break;

        // Case 2: All employees
        if (AllEmployees)
        {
            yield break; // ✅ No need to check others
        }

        // Case 3: Manual input mode
        if (EmployeeId == null && (EmployeeIds == null || EmployeeIds.Count == 0))
        {
            yield return new ValidationResult(
                "Either EmployeeId or EmployeeIds must be provided when AllEmployees is false.",
                new[] { nameof(EmployeeId), nameof(EmployeeIds) });
        }

        if (EmployeeId != null && EmployeeIds != null && EmployeeIds.Count > 0)
        {
            yield return new ValidationResult(
                "You can only provide either EmployeeId or EmployeeIds, not both.",
                new[] { nameof(EmployeeId), nameof(EmployeeIds) });
        }
    }

}
