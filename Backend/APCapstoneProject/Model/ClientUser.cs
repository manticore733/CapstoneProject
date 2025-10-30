using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{
    public class ClientUser: User
    {
 
        //nav property
        public List<Beneficiary>? Beneficiaries { get; set; } = new List<Beneficiary>();
        //nav property
        public List<Employee>? Employees { get; set; } = new List<Employee>();




        [Required(ErrorMessage = "Establishment Date is Required!")]
        [DataType(DataType.Date)]
        public DateTime EstablishmentDate { get; set; }


        [Required(ErrorMessage = "Address is Required!")]
        public string Address { get; set; }

        //will be verified by BankUser
        //nav property
        [Required(ErrorMessage = "Verification Status is Required!")]
        public int StatusId { get; set; }
        public virtual Status? VerificationStatus { get; set; }

        //nav property
        public virtual ICollection<Document>? Documents { get; set; } = new List<Document>();


        //nav property
        //Add account ID if possible
        public virtual Account? Account { get; set; }


        //nav property
        public int? BankUserId { get; set; }
        public virtual BankUser? BankUser { get; set; }

    }
}
