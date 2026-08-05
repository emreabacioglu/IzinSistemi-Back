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
            var pw = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD") ?? "agna vzuh vtbm vclf";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            var mailMessage = new MailMessage(from: mail, to: toEmail, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
