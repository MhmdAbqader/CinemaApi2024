using System.ComponentModel.DataAnnotations;

namespace DevCreedApi_Net6Movies.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }


        [Required]
        
        public string Name { get; set; }

    }
}
