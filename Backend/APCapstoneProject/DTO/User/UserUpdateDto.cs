using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.User
{
    public class UserUpdateDto
    {
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
    }
}
