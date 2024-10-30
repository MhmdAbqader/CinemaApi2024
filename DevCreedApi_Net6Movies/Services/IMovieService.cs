using DevCreedApi_Net6Movies.Models;

namespace DevCreedApi_Net6Movies.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetAll(int genreId = 0);
        Task<Movie> GetByIdl(int id);
        Task<Movie> Add(Movie movie);

        Movie Update(Movie movie);
        Movie Delete(Movie movie);

        Task<bool> IsExistMovie(int id);
    }
}
