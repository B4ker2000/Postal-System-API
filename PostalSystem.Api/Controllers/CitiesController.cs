using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostalSystem.Api.Data;

namespace PostalSystem.Api.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly PostalSystemDbContext _context; // write a service for this!

        public CitiesController(PostalSystemDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cities = await _context.Cities
                .OrderBy(c => c.CityName)
                .Select(c => new { c.CityId, c.CityName })
                .ToListAsync();

            return Ok(cities);
        }
    }
}