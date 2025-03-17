using DevCreedApi_Net6Movies.Models;
using Microsoft.EntityFrameworkCore;

namespace DevCreedApi_Net6Movies.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;

        public MovieService(ApplicationDbContext db)
        {
            _context = db;
        }


        public async Task<IEnumerable<Movie>> GetAll(int genreId = 0)
        {
            return await _context.Movies.Where(x => x.GenreID == genreId || genreId ==0)
                         .OrderByDescending(x => x.Rate)
                         .Include(x => x.Genre)
                         .ToListAsync();
        }

        public async Task<Movie> GetByIdl(int id)
        {
            return await _context.Movies.Include(x => x.Genre).SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Movie> Add(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            _context.SaveChanges();
            return movie;
        }

        public  Movie Delete(Movie movie)
        {
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return movie;
        }


        public Movie Update(Movie movie)
        {
            _context.Movies.Update(movie);
            _context.SaveChanges();
            return movie;
        }

        public async Task<bool> IsExistMovie(int id)
        {
          return await _context.Movies.AnyAsync(x => x.Id == id);
        }
    }
}
