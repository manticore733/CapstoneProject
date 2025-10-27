using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.User
{
    public class UpdateBankUserDto
    {
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? Branch { get; set; }
    }
}
