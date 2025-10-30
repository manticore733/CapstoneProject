using APCapstoneProject.Model;

namespace APCapstoneProject.DTO.User.ClientUser
{
    public class ReadClientUserDto
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }

        public int UserRoleId { get; set; }
        public string? RoleName { get; set; }

        // public int? BankId { get; set; } // should not be here, discuss 
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
