using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{

    public enum AccType { SAVINGS, CURRENT, SALARY }
    public class AccountType
    {
        public int AccountTypeId { get; set; }

        [Required(ErrorMessage = "Type is Required!")]
        public AccType Type { get; set; }
    }
}
