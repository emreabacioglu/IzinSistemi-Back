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

            // Render Environment'tan oku
            var pw = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");

            if (string.IsNullOrEmpty(pw))
            {
                Console.WriteLine("❌ HATA: GMAIL_APP_PASSWORD ortam değişkeni boş veya bulunamadı!");
                throw new InvalidOperationException("GMAIL_APP_PASSWORD bulunamadı.");
            }

            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(mail, pw.Trim());

                    using (var mailMessage = new MailMessage(from: mail, to: toEmail, subject, body))
                    {
                        mailMessage.IsBodyHtml = true;
                        await client.SendMailAsync(mailMessage);
                        Console.WriteLine($"✅ MAİL BAŞARIYLA GÖNDERİLDİ: {toEmail}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SMTP MAİL GÖNDERME HATASI: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"❌ DETAY: {ex.InnerException.Message}");
                }
                throw; // Hatanın yukarı fırlatılıp görülmesini sağla
            }
        }
    }
}