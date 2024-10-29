using DevCreedApi_Net6Movies.Dtos;
using DevCreedApi_Net6Movies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevCreedApi_Net6Movies.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var genres = await _context.Genres.OrderBy(a=>a.Name).ToListAsync();

            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateDto dto) {

            var genre = new Genre { Name = dto.Name };
              await _context.Genres.AddAsync(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]CreateDto dto) { 
         var genre=await _context.Genres.SingleOrDefaultAsync(x=>x.Id==id);
            if (genre == null)
                return NotFound($"no genre with id ={id}");
            genre.Name = dto.Name;
            _context.SaveChanges();

            return Ok(genre);        
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(x => x.Id == id);
            if (genre == null)
            { return NotFound($"no Genre with ID= {id}"); }

            _context.Genres.Remove(genre);
            _context.SaveChanges();

            return Ok("done");
        }


    }
}
