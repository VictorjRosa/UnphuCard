using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace UnphuCard_Api.Service
{
    public interface IServicioEmail
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
        Task SendContactEmailAsync(string fromEmail, string fromName, string subject, string message);
    }

    public class EmailService : IServicioEmail
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("UnphuCard", _configuration["Smtp:Username"]));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendContactEmailAsync(string fromEmail, string fromName, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(fromName, fromEmail));
            emailMessage.To.Add(new MailboxAddress("UnphuCard", _configuration["Smtp:Username"]));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };
            emailMessage.ReplyTo.Add(new MailboxAddress(fromName, fromEmail));


            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}