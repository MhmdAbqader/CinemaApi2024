using System.ComponentModel.DataAnnotations;

namespace DevCreedApi_Net6Movies.Dtos
{
    public class CreateMovieDto
    {
        
        [Required]
        public string Title { get; set; }

        public int Year { get; set; }

        public double Rate { get; set; }

        [Required]
        [MaxLength(2000)]
        public string StoreLine { get; set; }

        public IFormFile Poster { get; set; }

        public int GenreID { get; set; }
    }
}
