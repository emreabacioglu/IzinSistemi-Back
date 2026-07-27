using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IzinSistemi_Back.Data;
using Microsoft.EntityFrameworkCore;
using IzinSistemi_Back.Models;

namespace IzinSistemi_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaves()
        {
            var leaves = await _context.Leaves.ToListAsync();
            return Ok(leaves);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeave(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return NotFound("Böyle bir izin bulunamadı.");
            }
            return Ok(leave);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeave([FromBody] Leave leave)
        {
            if (leave == null)
            {
                return BadRequest("Gönderilen izin verisi boş olamaz.");
            }
            var existingLeave = await _context.Leaves.FirstOrDefaultAsync(l => l.EmployeeId == leave.EmployeeId && l.StartDate <= leave.EndDate && l.EndDate >= leave.StartDate);
            if (existingLeave != null)
            {
                existingLeave.Status = leave.Status;
            }
            else
            {
                var newLeave = new Leave
                {
                    EmployeeId = leave.EmployeeId,
                    StartDate = leave.StartDate,
                    EndDate = leave.EndDate,
                    RequestDate = leave.RequestDate,
                    Status = leave.Status
                };
                _context.Leaves.Add(newLeave);
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "İzin başarıyla kaydedildi." });
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetLeavesByEmployeeId(int employeeId)
        {
            var leaves = await _context.Leaves.Where(l => l.EmployeeId == employeeId).ToListAsync();
            return Ok(leaves);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeave(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return NotFound("Silinecek izin bulunamadı.");
            }
            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();

            return Ok(new { message = "İzin başarıyla iptal edildi." });
        }

        public class LeaveRequestDto
        {
            public int EmployeeId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime RequestDate { get; set; }
            public string Status { get; set; }
        }
    }
}