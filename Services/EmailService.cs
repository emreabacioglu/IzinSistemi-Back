using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IzinSistemi_Back.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mail = "ZT.emre.72@gmail.com";
            var pw = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");

            if (string.IsNullOrEmpty(pw))
            {
                throw new InvalidOperationException("GMAIL_APP_PASSWORD ortam değişkeni bulunamadı!");
            }

            // Boşlukları tamamen temizle
            pw = pw.Replace(" ", "").Trim();

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(mail, pw);
                client.Timeout = 10000; // 10 saniye zaman aşımı ekle

                using (var mailMessage = new MailMessage(from: mail, to: toEmail, subject, body))
                {
                    mailMessage.IsBodyHtml = true;
                    await client.SendMailAsync(mailMessage);
                }
            }
        }
    }
}