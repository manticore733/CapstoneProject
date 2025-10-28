namespace APCapstoneProject.DTO.Beneficiary
{
    public class BeneficiaryReadDto
    {
        public int BeneficiaryId { get; set; }
        public int ClientUserId { get; set; }
        public string BeneficiaryName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string IFSC { get; set; }
        public bool IsActive { get; set; }
    }
}
