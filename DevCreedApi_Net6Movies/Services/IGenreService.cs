using DevCreedApi_Net6Movies.Dtos;
using DevCreedApi_Net6Movies.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevCreedApi_Net6Movies.Services
{
    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetAll();
        Task<Genre> GetByIdl(int id);
        Task<Genre> Add(Genre genre);

        Genre Update(Genre genre);
        Genre Delete(Genre genre);

        // New Function
        Task<bool> IsValid(int id);

    }
}
