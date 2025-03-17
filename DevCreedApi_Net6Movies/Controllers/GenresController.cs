using DevCreedApi_Net6Movies.Dtos;
using DevCreedApi_Net6Movies.Models;
using DevCreedApi_Net6Movies.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevCreedApi_Net6Movies.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genresList = await _genreService.GetAll();
            return Ok(genresList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {            
            return Ok(await _genreService.GetByIdl(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateDto dto) {

            var genre = new Genre { Name = dto.Name };
            await _genreService.Add(genre);           
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]CreateDto dto) {
            var genre = await _genreService.GetByIdl(id);
            if (genre == null)
                return NotFound($"no genre with id ={id}");

            genre.Name = dto.Name;
            _genreService.Update(genre);
            return Ok(genre);        
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreService.GetByIdl(id);
            if (genre == null)
            { return NotFound($"no Genre with ID= {id}"); }

            _genreService.Delete(genre);

            return Ok("deleted successfully!");
        }


    }
}
