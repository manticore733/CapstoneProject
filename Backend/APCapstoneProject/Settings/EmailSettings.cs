namespace APCapstoneProject.Settings
{
    public class EmailSettings
    {
        public string FromEmail { get; set; } = null!;
        public string FromName { get; set; } = null!;
        public string SmtpServer { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        public string? OverrideToEmail { get; set; }
    }
}
