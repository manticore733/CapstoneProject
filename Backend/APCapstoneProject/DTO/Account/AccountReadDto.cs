namespace APCapstoneProject.DTO.Account
{
    public class AccountReadDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public int ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
