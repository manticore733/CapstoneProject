using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{
    public enum StatusEnum { PENDING, APPROVED, REJECTED }
    public class Status
    {
        public int StatusId { get; set; }

        [Required(ErrorMessage = "Status is Required!")]
        public StatusEnum StatusEnum { get; set; } = StatusEnum.PENDING;
    }
}
