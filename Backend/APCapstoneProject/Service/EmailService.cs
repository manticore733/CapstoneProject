using System.Collections.Generic;
using System.Threading.Tasks;
using APCapstoneProject.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace APCapstoneProject.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings; //our custom settings class like for cloudinary we had
        private readonly EmailTemplateRenderer _renderer;

        public EmailService(IOptions<EmailSettings> options, EmailTemplateRenderer renderer)
        {
            _settings = options.Value;
            _renderer = renderer;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();

            // Always send from configured sender
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));

            // Forced to personal email address
            var forcedRecipient = _settings.OverrideToEmail ?? _settings.FromEmail;
            message.To.Add(MailboxAddress.Parse(forcedRecipient));

            // Includes original recipient
            string infoHeader = $"<p style='color:#666;font-size:12px;'>[Originally intended for: {toEmail}]</p>";
            string finalBody = infoHeader + htmlBody;

            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = finalBody }.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
                await smtp.SendAsync(message);
                Console.WriteLine($" Email sent to {_settings.OverrideToEmail ?? _settings.FromEmail} [original: {toEmail}]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Email send failed: {ex.Message}");
                throw;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task SendTemplateEmailAsync(string toEmail, string subject, string templateName, IDictionary<string, string?> tokens)
        {
            var body = await _renderer.RenderAsync(templateName, tokens);
            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
