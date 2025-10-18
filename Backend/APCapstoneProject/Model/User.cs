using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APCapstoneProject.Model
{
    public class User
    {

        
        [Required(ErrorMessage = "User Full Name is Required!")]
        public int UserId { get; set; }



        [Required(ErrorMessage = "UserName is Required!")]
        public string UserFullName { get; set; }
      


        //login purpose
        [Required(ErrorMessage = "UserName is Required!")]
        public string UserName { get; set; }
        


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is Required!")]
        public string PasswordHash { get; set; }


        //nav property
        [Required(ErrorMessage = "User Role is Required!")]
        public int UserRoleId { get; set; }
        public virtual UserRole? Role { get; set; }




        [Required(ErrorMessage = "User Email is Required!")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; } = null!;



        [Required(ErrorMessage = "User Phone is Required!")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string UserPhone { get; set; } = null!;



        //nav property
        public int BankId { get; set; }
        public virtual Bank? Bank { get; set; }



        [Required(ErrorMessage = "User Joining Date is Required!")]
        public DateTime UserJoiningDate { get; set; } = DateTime.Now.Date;



        public bool isActive { get; set; } = true;


        //optional : review later
        public DateTime LastLogin { get; set; }



     





    }
}
