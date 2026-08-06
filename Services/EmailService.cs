using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IzinSistemi_Back.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {

            var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("BREVO_API_KEY ortam değişkeni bulunamadı!");
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", apiKey.Trim());

            var payload = new
            {
                sender = new { name = "Kurumsal İzin Sistemi", email = "zt.emre.72@gmail.com" },
                to = new[] { new { email = toEmail } },
                subject = subject,
                htmlContent = body
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.brevo.com/v3/smtp/email", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ API Mail Hatası: {errorDetails}");
                throw new Exception($"E-posta API üzerinden gönderilemedi: {response.StatusCode}");
            }

            Console.WriteLine($"✅ API MAİLİ BAŞARIYLA GÖNDERİLDİ: {toEmail}");
        }
    }
}