using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{
    public class BankUser : User
    {


        //mutiple bank users of the same bank can be in diffrent branches,thus branch

        [Required(ErrorMessage = "branch is Required!")]
        public string Branch { get; set; }




        //confustion btw i colelction nd ienumerable
        public virtual ICollection<ClientUser> Clients { get; set; } = new List<ClientUser>();

    }
}
