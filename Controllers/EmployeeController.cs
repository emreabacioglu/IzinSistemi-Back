using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IzinSistemi_Back.Data;
using IzinSistemi_Back.Models;
using Microsoft.EntityFrameworkCore;

namespace IzinSistemi_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.Leaves)
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Surname,
                    FullName = e.Name + " " + e.Surname,
                    e.Department,
                    e.Title,

                    Leaves = e.Leaves.Select(l => new
                    {
                        l.Id,
                        l.StartDate,
                        l.Status
                    }).ToList()
                })
                .ToListAsync();

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Böyle bir çalışan bulunamadı.");
            }
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Gönderilen çalışan verisi boş olamaz.");
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == request.Email);

            if (employee == null || !BCrypt.Net.BCrypt.Verify(request.Password, employee.Password))
            {
                return Unauthorized(new {message = "Geçersiz e-posta veya şifre."});
            }
            return Ok(employee);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Böyle bir çalışan bulunamadı." });
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Çalışan başarıyla silindi." });
        }

        /*
        //sil geçici çalışanlar
        [HttpPost("seed")]
        public async Task<IActionResult> SeedMockData()
        {
            // Eğer daha önce eklendiyse, mükerrer kayıt olmasın diye kontrol edelim
            if (_context.Employees.Any(e => e.Email.StartsWith("mock")))
            {
                return BadRequest("Test verileri zaten veritabanında mevcut.");
            }

            var mockUsers = new List<Employee>
            {
                new Employee { Name = "Ahmet", Surname = "Yılmaz", Email = "mock1@test.com", Password = "123", Department = "Temel Bankacılık", Title = "Yönetici" },
                new Employee { Name = "Ayşe", Surname = "Kaya", Email = "mock2@test.com", Password = "123", Department = "Temel Bankacılık", Title = "Yazılımcı" },
                new Employee { Name = "Mehmet", Surname = "Demir", Email = "mock3@test.com", Password = "123", Department = "Temel Bankacılık", Title = "Analist" },

                new Employee { Name = "Fatma", Surname = "Çelik", Email = "mock4@test.com", Password = "123", Department = "Nakit Yönetimi", Title = "Yönetici" },
                new Employee { Name = "Burak", Surname = "Şahin", Email = "mock5@test.com", Password = "123", Department = "Nakit Yönetimi", Title = "Yazılımcı" },
                new Employee { Name = "Ceren", Surname = "Güneş", Email = "mock6@test.com", Password = "123", Department = "Nakit Yönetimi", Title = "Analist" },
                new Employee { Name = "Ali", Surname = "Koç", Email = "mock7@test.com", Password = "123", Department = "Nakit Yönetimi", Title = "Yazılımcı" },

                new Employee { Name = "Emre", Surname = "Doğan", Email = "mock8@test.com", Password = "123", Department = "Çek Senet", Title = "Yönetici" },
                new Employee { Name = "Zeynep", Surname = "Öztürk", Email = "mock9@test.com", Password = "123", Department = "Çek Senet", Title = "Analist" },
                new Employee { Name = "Deniz", Surname = "Arslan", Email = "mock10@test.com", Password = "123", Department = "Çek Senet", Title = "Yazılımcı" }
            };

            // Eğer veritabanındaki Employee tablonun adı farklıysa (örn: _context.Users), yukarıdaki ve aşağıdaki Employees kısmını ona göre değiştir.
            _context.Employees.AddRange(mockUsers);
            await _context.SaveChangesAsync();

            return Ok("10 adet test kullanıcısı başarıyla eklendi! 🎉");
        }
        */
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}