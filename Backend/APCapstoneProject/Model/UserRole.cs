using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{
    public enum Role { SUPER_ADMIN, BANK_USER, CLIENT_USER }
    public class UserRole
    {
        public int UserRoleId { get; set; }

        [Required(ErrorMessage = "Role is Required!")]
        public Role Role { get; set; }
    }
}
