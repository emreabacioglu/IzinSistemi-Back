using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace IzinSistemi_Back.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var senderEmail = "ZT.emre.72@gmail.com";
            var rawPw = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");

            if (string.IsNullOrEmpty(rawPw))
            {
                throw new InvalidOperationException("GMAIL_APP_PASSWORD ortam değişkeni bulunamadı!");
            }

            var pw = rawPw.Replace(" ", "").Trim();

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(senderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();

            try
            {
                await smtp.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                await smtp.AuthenticateAsync(senderEmail, pw);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MailKit 465 Port Hatası: {ex.Message}");
                throw;
            }
        }
    }
}