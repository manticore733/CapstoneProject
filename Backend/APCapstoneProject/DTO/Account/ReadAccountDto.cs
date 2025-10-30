namespace APCapstoneProject.DTO.Account
{
    public class ReadAccountDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string? ClientName { get; set; }
        public string? BankName { get; set; }
    }

    public class TransactionAmountDto
    {
        public decimal Amount { get; set; }
    }
}
