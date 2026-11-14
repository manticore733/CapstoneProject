namespace APCapstoneProject.DTO.Employee
{
    public class EmployeeUploadResultDto
    {
        public int SuccessCount { get; set; } = 0;
        public int FailedCount { get; set; } = 0;
        public List<FailedRowInfo> FailedRows { get; set; } = new();
    }

    public class FailedRowInfo
    {
        public int RowNumber { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
