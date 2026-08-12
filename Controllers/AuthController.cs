using IzinSistemi_Back.Data;
using IzinSistemi_Back.Models;
using IzinSistemi_Back.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                {
                    return BadRequest(new { message = "E-posta adresi boş olamaz." });
                }

                string cleanEmail = dto.Email.Trim().ToLower();

                var existingUser = await _context.Employees.FirstOrDefaultAsync(e => e.Email.ToLower() == cleanEmail);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Bu e-posta adresi ile zaten bir kayıt mevcut. Lütfen giriş yapın." });
                }

                string spamKey = $"SpamCheck_{cleanEmail}";
                if (_cache.TryGetValue(spamKey, out int requestCount))
                {
                    if (requestCount >= 5)
                    {
                        return BadRequest(new { message = "Çok fazla kod istediniz. Lütfen 10 dakika bekleyin." });
                    }
                    _cache.Set(spamKey, requestCount + 1, TimeSpan.FromMinutes(10));
                }
                else
                {
                    _cache.Set(spamKey, 1, TimeSpan.FromMinutes(10));
                }

                Random rnd = new Random();
                string otpCode = rnd.Next(100000, 999999).ToString();

                var cacheData = new UserRegistrationCacheData
                {
                    UserInfo = new RegisterDto
                    {
                        Name = dto.Name,
                        Surname = dto.Surname,
                        Email = cleanEmail,
                        Password = dto.Password
                    },
                    OtpCode = otpCode
                };

                _cache.Set(cleanEmail, cacheData, TimeSpan.FromMinutes(5));

                string subject = $"Kurumsal İzin Sistemi Doğrulama Kodunuz: {otpCode}";
                string body = $@"
        <div style='font-family: Arial, sans-serif; text-align: center; padding: 20px;'>
            <h2>Hoş Geldiniz, {dto.Name}!</h2>
            <p>Kayıt işleminizi tamamlamak için doğrulama kodunuz aşağıdadır (5 dakika geçerlidir):</p>
            <h1 style='color: #E10514; background: #f8f9fa; padding: 15px; border-radius: 8px; display: inline-block;'>
                {otpCode}
            </h1>
        </div>";

                await _emailService.SendEmailAsync(cleanEmail, subject, body);

                return Ok(new { message = "Doğrulama kodu e-posta adresinize gönderildi." });
            }
            catch (Exception ex)
            {
                string errorDetails = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { message = "Kayıt hatası: " + errorDetails });
            }
        }

        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest("Geçersiz istek.");
            }

            string cleanEmail = dto.Email.Trim().ToLower();

            if (_cache.TryGetValue(cleanEmail, out UserRegistrationCacheData savedData))
            {
                if (savedData.OtpCode == dto.OtpCode)
                {
                    var alreadyExists = await _context.Employees.AnyAsync(e => e.Email.ToLower() == cleanEmail);
                    if (alreadyExists)
                    {
                        _cache.Remove(cleanEmail);
                        return BadRequest("Bu e-posta adresi zaten doğrulanmış ve kaydedilmiş.");
                    }

                    var newEmployee = new Employee
                    {
                        Name = savedData.UserInfo.Name,
                        Surname = savedData.UserInfo.Surname,
                        Email = savedData.UserInfo.Email,
                        Password = BCrypt.Net.BCrypt.HashPassword(savedData.UserInfo.Password),
                        Department = "Belirtilmedi",
                        Title = "Belirtilmedi"
                    };

                    _context.Employees.Add(newEmployee);

                    await _context.SaveChangesAsync();

                    _cache.Remove(cleanEmail);

                    return Ok(new
                    {
                        id = newEmployee.Id,
                        name = newEmployee.Name,
                        surname = newEmployee.Surname,
                        email = newEmployee.Email,
                        isAdmin = newEmployee.IsAdmin,
                        department = newEmployee.Department,
                        title = newEmployee.Title,
                        message = "Doğrulama başarılı. Sisteme giriş yapılıyor..."
                    });
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
                return NotFound("Kullanıcı bulunamadı");
            }

            employee.Department = dto.Department;
            employee.Title = dto.Title;

            if (dto.TotalLeaveDays.HasValue)
            {
                employee.TotalLeaveDays = dto.TotalLeaveDays.Value;
            }

            employee.BirthDay = dto.Birthday;
            employee.LeaveReset = dto.LeaveReset;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil başarıyla güncellendi." });
        }

        [HttpPost("toggle-admin/{id}")]
        public async Task<IActionResult> ToggleAdminStatus(int id, [FromBody] ToggleAdminDto request)
        {
            var user = await _context.Employees.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı" });
            }

            user.IsAdmin = request.IsAdmin;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Kullanıcı yetkisi başarıyla güncellendi.", isAdmin = user.IsAdmin });
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                {
                    return BadRequest(new { message = "E-posta adresi boş olamaz." });
                }

                string cleanEmail = dto.Email.Trim().ToLower();

                var user = await _context.Employees.FirstOrDefaultAsync(e => e.Email.ToLower() == cleanEmail);
                if (user == null)
                {
                    return Ok(new { message = "Eğer sistemde kayıtlı bir e-posta adresi girdiyseniz, şifre sıfırlama kodu gönderilmiştir." });
                }

                Random rnd = new Random();
                string otpCode = rnd.Next(100000, 999999).ToString();

                _cache.Set($"Reset_{cleanEmail}", otpCode, TimeSpan.FromMinutes(10));

                string subject = $"Şifre Sıfırlama Kodunuz: {otpCode}";
                string body = $@"
            <div style='font-family: Arial, sans-serif; text-align: center; padding: 20px;'>
                <h2>Şifre Sıfırlama Talebi</h2>
                <p>Hesabınızın şifresini sıfırlamak için aşağıdaki kodu kullanın (10 dakika geçerlidir):</p>
                <h1 style='color: #E10514; background: #f8f9fa; padding: 15px; border-radius: 8px; display: inline-block;'>
                    {otpCode}
                </h1>
                <p style='font-size: 11px; color: #6c757d;'>Bu işlemi siz talep etmediyseniz, lütfen bu e-postayı dikkate almayın.</p>
            </div>";

                await _emailService.SendEmailAsync(cleanEmail, subject, body);

                return Ok(new { message = "Eğer sistemde kayıtlı bir e-posta adresi girdiyseniz, şifre sıfırlama kodu gönderilmiştir." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Mail gönderilirken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest("Geçersiz istek.");
            }

            string cleanEmail = dto.Email.Trim().ToLower();

            if (_cache.TryGetValue($"Reset_{cleanEmail}", out string savedCode) && savedCode == dto.OtpCode)
            {
                var user = await _context.Employees.FirstOrDefaultAsync(e => e.Email.ToLower() == cleanEmail);
                if (user == null) return BadRequest("Kullanıcı bulunamadı.");

                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                await _context.SaveChangesAsync();

                _cache.Remove($"Reset_{cleanEmail}");

                return Ok(new { message = "Şifreniz başarıyla değiştirildi. Yeni şifrenizle giriş yapabilirsiniz." });
            }

            return BadRequest(new { message = "Hatalı veya süresi dolmuş kod girdiniz." });
        }

        [HttpGet("VerifySession/{id}")]
        public async Task<IActionResult> VerifySession(int id)
        {
            var user = await _context.Employees.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı hesabı bulunamadı veya silinmiş." });
            }

            return Ok(new
            {
                id = user.Id,
                name = user.Name,
                surname = user.Surname,
                email = user.Email,
                isAdmin = user.IsAdmin,
                department = user.Department,
                title = user.Title
            });
        }
    }

    public class RegisterDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Şifreniz en az 8 karakter uzunluğunda olmalı; en az 1 büyük harf, 1 küçük harf, 1 sayı ve 1 özel karakter (!@#$%^&*) içermelidir.")]
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
        public int? TotalLeaveDays { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? LeaveReset { get; set; }
    }

    public class ToggleAdminDto
    {
        public bool IsAdmin { get; set; }
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Şifreniz en az 8 karakter uzunluğunda olmalı; en az 1 büyük harf, 1 küçük harf, 1 sayı ve 1 özel karakter (!@#$%^&*) içermelidir.")]
        public string NewPassword { get; set; }
    }
}