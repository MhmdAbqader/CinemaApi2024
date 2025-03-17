using DevCreedApi_Net6Movies.Models;
using Microsoft.EntityFrameworkCore;

namespace DevCreedApi_Net6Movies.Services
{
    public class GenreService : IGenreService
    {
        private readonly ApplicationDbContext _context;

        public GenreService(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<Genre>> GetAll()
        {
          return  await _context.Genres.OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<Genre> GetByIdl(int id)
        {
            return await _context.Genres.FindAsync(id);
        }
        public async Task<Genre> Add(Genre genre)
        {
           
            await _context.Genres.AddAsync(genre);
            _context.SaveChanges();
            return genre;
        }

        public Genre Delete(Genre genre)
        {
            _context.Genres.Remove(genre);
            _context.SaveChanges();
            return genre;
        }



        public Genre Update(Genre genre)
        {

            _context.Genres.Update(genre);
            _context.SaveChanges();
            return   genre ;
        }

        public async Task<bool> IsValid(int id)
        {
            return await _context.Genres.AnyAsync(x => x.Id == id);
        }
    }
}
