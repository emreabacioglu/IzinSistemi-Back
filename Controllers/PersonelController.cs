using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IzinSistemi_Back.Data;
using IzinSistemi_Back.Models;
using Microsoft.EntityFrameworkCore;

namespace IzinSistemi_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonelController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PersonelController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonels()
        {
            var personels = await _context.Personels.ToListAsync();
            return Ok(personels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonel(int id)
        {
            var personel = await _context.Personels.FindAsync(id);
            if (personel == null)
            {
                return NotFound("Böyle bir personel bulunamadı.");
            }
            return Ok(personel);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersonel([FromBody] Personel personel)
        {
            if (personel == null)
            {
                return BadRequest("Gönderilen personel verisi boş olamaz.");
            }
            _context.Personels.Add(personel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPersonel), new { id = personel.Id }, personel);

        }
    }
}
