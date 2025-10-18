using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{
    public class BankUser : User
    {

        //optional : review for later idk what it is
        [Required(ErrorMessage = "Refferal code is Required!")]
        public string RefferalCode { get; set; }


        //mutiole bank users of the same bank can be in diffrent branches,thus branch

        [Required(ErrorMessage = "branch is Required!")]
        public string Branch { get; set; }

        public bool isActive { get; set; } = true;



        //confustion btw i colelction nd ienumerable
        public ICollection<ClientUser> Clients { get; set; } = new List<ClientUser>();
    }
}
