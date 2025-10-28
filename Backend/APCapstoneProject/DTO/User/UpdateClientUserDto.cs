using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.User
{
    public class UpdateClientUserDto
    {
        [Required]
        public int UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? Address { get; set; }
        public bool? IsActive { get; set; }
        public int? StatusId { get; set; } // To approve/reject a client
    }
}
