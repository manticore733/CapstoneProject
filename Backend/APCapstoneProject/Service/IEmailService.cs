namespace APCapstoneProject.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendTemplateEmailAsync(string toEmail, string subject, string templateName, IDictionary<string, string?> tokens);
    }
}
