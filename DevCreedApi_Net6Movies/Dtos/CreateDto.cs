using System.ComponentModel.DataAnnotations;

namespace DevCreedApi_Net6Movies.Dtos
{
    public class CreateDto
    {
        [Required]
       
        public string Name { get; set; }
    }
}
