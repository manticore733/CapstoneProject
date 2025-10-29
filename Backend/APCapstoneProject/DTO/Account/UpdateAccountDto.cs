namespace APCapstoneProject.DTO.Account
{
    public class UpdateAccountDto
    {
        public decimal? Balance { get; set; }
        public int? StatusId { get; set; } // e.g. blocked, active
    }
}
