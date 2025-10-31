using System;
using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Bank
{
    public class BankUpdateDto
    {
        public string? BankName { get; set; }
        public string? IFSC { get; set; }

        //when sending null, it sets to 00:00 as default when it should remain unchanged. review needed.
        //[DataType(DataType.Date)]
        //public DateTime? EstablishmentDate { get; set; }
    }
}
