using IzinSistemi_Back.Data;
using IzinSistemi_Back.Models;
using IzinSistemi_Back.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IzinSistemi_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _context;

        public AuthController(IEmailService emailService, IMemoryCache cache, AppDbContext context)
        {
            _emailService = emailService;
            _cache = cache;
            _context = context;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // --- 1. SPAM VE CEZA KONTROLÜ (RATE LIMITING) ---
            string spamKey = $"SpamCheck_{dto.Email}";

            if (_cache.TryGetValue(spamKey, out int requestCount))
            {
                // Toplam 3 hakki doldurduysa (1 ilk istek + 2 tekrar) banla!
                if (requestCount >= 3)
                {
                    return BadRequest("Maksimum kod isteme sınırına ulaştınız. Lütfen 10 dakika bekleyin.");
                }

                // Sınırı aşmadıysa (2. veya 3. istekse) sayacı 1 artır ve süresini 10 dakika yap
                _cache.Set(spamKey, requestCount + 1, TimeSpan.FromMinutes(10));
            }
            else
            {
                // 1. İstek (Kullanıcı ilk defa butona basıyor), sayacı 1 yap
                _cache.Set(spamKey, 1, TimeSpan.FromMinutes(10));
            }


            // --- 2. YENİ KOD ÜRET VE ESKİSİNİ EZ ---
            Random rnd = new Random();
            string otpCode = rnd.Next(100000, 999999).ToString();

            // Kodun geçerlilik ömrü (Ekrana girme süresi) hala 5 dakika
            var cacheData = new UserRegistrationCacheData { UserInfo = dto, OtpCode = otpCode };
            _cache.Set(dto.Email, cacheData, TimeSpan.FromMinutes(5));


            // --- 3. MAİL GÖNDERİMİ ---
            string subject = "Kurumsal İzin Sistemi - Doğrulama Kodunuz";
            string body = $@"
        <div style='font-family: Arial, sans-serif; text-align: center; padding: 20px;'>
            <h2>Hoş Geldiniz, {dto.Name}!</h2>
            <p>Kayıt işleminizi tamamlamak için doğrulama kodunuz aşağıdadır (5 dakika geçerlidir):</p>
            <h1 style='color: #E10514; background: #f8f9fa; padding: 15px; border-radius: 8px; display: inline-block;'>
                {otpCode}
            </h1>
            <p style='font-size: 11px; color: #6c757d;'>Not: Güvenlik gereği çok fazla yeni kod isterseniz sisteminiz geçici olarak kilitlenir.</p>
        </div>";

            await _emailService.SendEmailAsync(dto.Email, subject, body);

            return Ok(new { message = "Doğrulama kodu e-posta adresinize gönderildi." });
        }

        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            
            if (_cache.TryGetValue(dto.Email, out UserRegistrationCacheData savedData))
            {
                
                if (savedData.OtpCode == dto.OtpCode)
                {
                    var newEmployee = new Employee
                    {
                        Name = savedData.UserInfo.Name,
                        Surname = savedData.UserInfo.Surname,
                        Email = savedData.UserInfo.Email,
                        Password = savedData.UserInfo.Password,
                        Department = "Belirtilmedi",
                        Title = "Belirtilmedi"
                    };

                    _context.Employees.Add(newEmployee);
                    await _context.SaveChangesAsync();

                    _cache.Remove(dto.Email);

                    return Ok(new { message = "Doğrulama başarılı. Sisteme giriş yapılıyor..." });
                }
            }

            return BadRequest("Hatalı veya süresi dolmuş kod girdiniz.");
        }

        [HttpPut("UpdateProfile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("KUllanıcı bulunamadı");
            }

            employee.Department = dto.Department;
            employee.Title = dto.Title;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil başarıyla güncellendi." });
        }
    }

        public class RegisterDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class VerifyOtpDto
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }

    public class UserRegistrationCacheData
    {
        public RegisterDto UserInfo { get; set; }
        public string OtpCode { get; set; }
    }

    public class UpdateProfileDto
    {
        public string Department { get; set; }
        public string Title { get; set; }
    }
}