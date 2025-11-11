using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.User
{
    //this dto is only being consumed, no entity is being created, so this is not present in automapper
    public class ClientApprovalDto
    {
        [Required(ErrorMessage = "Approval status is required.")]
        public bool IsApproved { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Initial balance must be a positive value.")]
        public decimal InitialBalance { get; set; } = 0;


        public string? Remark { get; set; }
    }
}

