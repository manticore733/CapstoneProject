using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace APCapstoneProject.Model
{
    public class Bank
    {
        public int BankId { get; set; }

        [Required(ErrorMessage = "Bank name is required!")]
        public string BankName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Bank IFSC is required")]
        public string IFSC { get; set; } = string.Empty;


        [DataType(DataType.Date)]
        public DateTime EstablishmentDate { get; set; }


        public virtual ICollection<User> Users { get; set; } = new List<User>();

        //nav property
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();


        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
