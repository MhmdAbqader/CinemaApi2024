using DevCreedApi_Net6Movies.Dtos;
using DevCreedApi_Net6Movies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevCreedApi_Net6Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        #region
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext db)
        {
            _context = db;
        }
        #endregion

        //by using anonymous object and select function i could choose genreName without DTo 
        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            //select by new anynonmous object
            var movie = await _context.Movies.Include(x => x.Genre)
                .Select(x => new
                {
                    myid = x.Id,
                    mytitle = x.Title,
                    myyear = x.Year,
                    rate = x.Rate,
                    //myimage=x.Poster,
                    mystorline = x.StoreLine,
                    genreid = x.GenreID,
                    mygenreName_from_anynomousobject = x.Genre.Name
                }).OrderByDescending(x => x.rate)
                .ToListAsync();
            return Ok(movie);
        }

        //FromDtO

        ////[HttpGet]
        ////public async Task<IActionResult> Getall()
        ////{

        ////    var movie = await _context.Movies.Include(x => x.Genre)
        ////        .Select(x =>new DtoMovieDetails {
        ////            Id=x.Id,                                     
        ////            Rate=x.Rate,
        ////            StoreLine=x.StoreLine, 
        ////            Title=x.Title,
        ////            Year=x.Year,
        ////            Poster = x.Poster,
        ////            GenreID = x.Genre.Id,
        ////            GenreName_from_dto = x.Genre.Name,
        ////        })
        ////        .ToListAsync();

        ////    return Ok(movie);
        ////}



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) {

            //var mov = await _context.Movies.FindAsync(id);
            //if (mov == null)
            //{ return NotFound($"the id={id}  not found......  try again! "); }
            // return Ok(mov);


            //my Idea [praise To Allah]
            bool IsValid= await _context.Movies.AnyAsync(x=>x.Id==id);

            if (!IsValid)
                return NotFound($"the id={id}  not found...  try again! ");

            var movie =await _context.Movies.Include(x=>x.Genre).SingleOrDefaultAsync(x => x.Id == id);

            byte[] x = new byte[0];
            var dto = new DtoMovieDetails
            {
                Id=movie.Id,
                Title=movie.Title,
                Year=movie.Year,
                Rate=movie.Rate,
                StoreLine=movie.StoreLine,
                GenreID=movie.GenreID,
                GenreName_from_dto=movie.Genre.Name ,
                Poster = x

            };

            return Ok(dto);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromForm]CreateMovieDto dto) {

            long maxsize = 1048576; // 1 Megabyte this is the allowed size for img that can user upload
            List<string> inextension = new List<string> { ".png", ".jpg" };

            if (!inextension.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
            { return BadRequest("allowed extension png , jpg"); }

            if (dto.Poster.Length > maxsize)
                return BadRequest("size is too large");

            ////my Idea Praise To allah
            //if(dto.GenreID<0 && dto.GenreID>10)
            //    return BadRequest($"Invalid GenreId ");

            var isvalid = await _context.Genres.AnyAsync(x => x.Id == dto.GenreID);
            if (!isvalid)
                return BadRequest($"Invalid GenreId ");
                
            using var datastream = new MemoryStream();
            await dto.Poster.CopyToAsync(datastream);

            var movie = new Movie
            {
                Title = dto.Title,  
                Rate = dto.Rate,
                Year = dto.Year,
                StoreLine = dto.StoreLine,
                GenreID=dto.GenreID,
                Poster=datastream.ToArray(),                
            };


            await _context.Movies.AddAsync(movie);
            _context.SaveChanges();

            return Ok(movie);
        }






        [HttpGet("Genre")]
        public async Task<IActionResult> GetGenre_and_relatedMovies(int id)
        {
            //select by new anynonmous object
            var Genre = await _context.Movies.Include(x => x.Genre).Where(m=>m.GenreID==id)
                .Select(x => new
                {
                    myid = x.Id,
                    mytitle = x.Title,
                    myyear = x.Year,
                    rate = x.Rate,
                    //myimage=x.Poster,
                    mystorline = x.StoreLine,
                    genreid = x.GenreID,
                    mygenreName_from_anynomousobject = x.Genre.Name
                }).OrderByDescending(x => x.rate)
                .ToListAsync();
            return Ok(Genre);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromForm] CreateMovieDto dto) {
            var movie= await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound($"No movie with id={id}");
            }
            if (dto.Poster != null) {
                long maxsize = 1048576;
                List<string> inextension = new List<string> { ".png", ".jpg" };

                if (!inextension.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                { return BadRequest("allowed extension png , jpg"); }

                if (dto.Poster.Length > maxsize)
                    return BadRequest("size is too large");
        
                using var datastream = new MemoryStream();
                await dto.Poster.CopyToAsync(datastream);
                movie.Poster= datastream.ToArray();

            }


            bool isvalid = await _context.Genres.AnyAsync(x => x.Id == dto.GenreID);
            if (!isvalid)
                return BadRequest($"Invalid GenreId ");

            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;
            movie.StoreLine = dto.StoreLine;
            movie.GenreID = dto.GenreID;

            _context.SaveChanges();


            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.SingleOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            { return NotFound($"no movie with ID= {id}"); }

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return StatusCode(201);
        }
    }
}
