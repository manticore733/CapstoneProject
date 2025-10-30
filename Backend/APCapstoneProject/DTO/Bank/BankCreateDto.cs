using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Bank
{
    public class BankCreateDto
    {
        [Required(ErrorMessage = "Bank name is required!")]
        public string BankName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bank IFSC is required!")]
        public string IFSC { get; set; } = string.Empty;
       
        [DataType(DataType.Date)]
        public DateTime EstablishmentDate { get; set; }
    }
}
