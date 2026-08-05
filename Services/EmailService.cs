using System.Net;
using System.Net.Mail;

namespace IzinSistemi_Back.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mail = "ZT.emre.72@gmail.com";

            // Şifreyi önce Render/Sunucu ortam değişkeninden oku, yoksa (local test için) yedek değeri kullan
            var pw = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");
            if (string.IsNullOrEmpty(pw))
            {
                throw new InvalidOperationException("GMAIL_APP_PASSWORD environment variable is not set.");
            }

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(mail, pw.Replace(" ", ""));

                using (var mailMessage = new MailMessage(from: mail, to: toEmail, subject, body))
                {
                    mailMessage.IsBodyHtml = true;
                    await client.SendMailAsync(mailMessage);

                }
            }
        }
    }
}
