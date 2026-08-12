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
                    e.Email,
                    e.IsAdmin,
                    e.Department,
                    e.Title,
                    e.TotalLeaveDays,

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

        
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}