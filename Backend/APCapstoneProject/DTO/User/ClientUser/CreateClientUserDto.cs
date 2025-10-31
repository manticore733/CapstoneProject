using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.User.ClientUser
{
    public class CreateClientUserDto
    {
        [Required(ErrorMessage = "Company Name is required")]
        public string UserFullName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{10}$")]
        public string UserPhone { get; set; }
        [Required]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EstablishmentDate { get; set; }
    }
}
