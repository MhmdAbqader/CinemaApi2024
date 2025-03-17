using System.ComponentModel.DataAnnotations;

namespace DevCreedApi_Net6Movies.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
