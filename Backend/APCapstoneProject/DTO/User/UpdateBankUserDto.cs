using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.User
{
    public class UpdateBankUserDto
    {
        [Required]
        public int UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? Branch { get; set; }
        public bool? IsActive { get; set; }
    }
}
