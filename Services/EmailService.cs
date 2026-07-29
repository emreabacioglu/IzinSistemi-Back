using System.Net;
using System.Net.Mail;

namespace IzinSistemi_Back.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mail = "ZT.emre.72@gmail.com";
            var pw = "agna vzuh vtbm vclf";

            var client = new SmtpClient("smtp.gmail.com",587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            var mailMessage = new MailMessage(from: mail, to: toEmail, subject, body);
            {
                mailMessage.IsBodyHtml = true;
            }
            ;

            await client.SendMailAsync(mailMessage);
        }
    }
}
