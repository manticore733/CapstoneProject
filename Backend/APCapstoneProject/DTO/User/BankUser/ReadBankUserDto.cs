namespace APCapstoneProject.DTO.User.BankUser
{
    public class ReadBankUserDto
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public int UserRoleId { get; set; }
        public string? RoleName { get; set; }
        public int? BankId { get; set; }
        public string? BankName { get; set; }
        public string Branch { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // number of client users:
        public int ClientCount { get; set; }
    }
}
