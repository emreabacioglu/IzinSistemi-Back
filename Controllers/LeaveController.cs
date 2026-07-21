using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IzinSistemi_Back.Data;
using Microsoft.EntityFrameworkCore;

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
    }
}
