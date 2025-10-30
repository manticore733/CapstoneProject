namespace APCapstoneProject.DTO.Bank
{
    public class BankReadDto
    {
        public int BankId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string IFSC { get; set; } = string.Empty;
        public DateTime EstablishmentDate { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int TotalBankUsers { get; set; }
        public int TotalClientUserAccountsHandled { get; set; }
    }
}
