using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevCreedApi_Net6Movies.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }  
        
        public int Year{ get; set; }

        public double Rate { get; set; }

        [Required]
        [MaxLength(2000)]
        public string StoreLine { get; set; }

        public byte[] Poster { get; set; }

        public int GenreID { get; set; }
        [ForeignKey(nameof(GenreID))]
        public Genre Genre { get; set; }


    }
}
