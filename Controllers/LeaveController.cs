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
            _context.Leaves.Add(leave);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLeave), new { id = leave.Id }, leave);
        }
    }
}
