using Application.Interfaces.Service.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace Cloud.Service.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Send(string from, string to, string subject, string html)
        {
            throw new NotImplementedException();
            // create message
            //var email = new MimeMessage();
            //email.From.Add(MailboxAddress.Parse(from));
            //email.To.Add(MailboxAddress.Parse(to));
            //email.Subject = subject;
            //email.Body = new TextPart(TextFormat.Html) { Text = html };

            //// send email
            //using var smtp = new SmtpClient();
            //smtp.Connect(_configuration["MailServer:SMTPServer"], int.Parse(_configuration["MailServer:Port"]), SecureSocketOptions.StartTls);
            //smtp.Authenticate(_configuration["MailServer:Username"], _configuration["MailServer:Password"]);
            //smtp.Send(email);
            //smtp.Disconnect(true);
        }

        public async Task SendAsync(string from, string to, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["MailServer:SenderAddress"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["MailServer:SMTPServer"], int.Parse(_configuration["MailServer:Port"]), SecureSocketOptions.Auto);
            await smtp.AuthenticateAsync(_configuration["MailServer:Username"], _configuration["MailServer:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}