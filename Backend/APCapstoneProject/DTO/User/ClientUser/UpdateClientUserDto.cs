using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.User.ClientUser
{
    public class UpdateClientUserDto
    {
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? Address { get; set; }
        public int? StatusId { get; set; } // To approve/reject a client
    }
}
