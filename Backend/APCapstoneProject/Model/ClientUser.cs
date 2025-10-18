using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Security.Principal;

namespace APCapstoneProject.Model
{
    public class ClientUser: User
    {

        // testing commit by pranav


 
        //nav property
        public List<Beneficiary>? Beneficiaries { get; set; } = new List<Beneficiary>();
        //nav property
        public List<Employee>? Employees { get; set; } = new List<Employee>();




        [Required(ErrorMessage = "Establishment Date is Required!")]
        [DataType(DataType.Date)]
        public DateTime EstablishmentDate { get; set; }





        [Required(ErrorMessage = "Address is Required!")]
        public string Address { get; set; }

        public bool IsVerified { get; set; } = false;

        //nav property
        public virtual ICollection<Document>? Documents { get; set; } = new List<Document>();


        //nav property
        public int? AccountId { get; set; }
        public virtual Account? Account { get; set; }


        //nav property
        public int? BankUserId { get; set; }
        public virtual BankUser? BankUser { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


    }




}
