using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IzinSistemi_Back.Data;
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
    }
}
