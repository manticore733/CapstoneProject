using APCapstoneProject.Model;

namespace APCapstoneProject.DTO.User
{
    public class ClientStatusReadDto
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }

        public int UserRoleId { get; set; }
        public string? RoleName { get; set; }

        public int? BankId { get; set; }
        public string? Address { get; set; }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }



        // --- ADD THIS LINE ---
        public string? AccountNumber { get; set; }

    }
}
