namespace APCapstoneProject.DTO.Employee
{
    public class EmployeeReadDto
    {
        public int EmployeeId { get; set; }
        public int ClientUserId { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string IFSC { get; set; }
        public decimal Salary { get; set; }
        public DateTime DateOfJoining { get; set; }
        public DateTime? DateOfLeaving { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
