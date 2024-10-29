using System.ComponentModel.DataAnnotations;

namespace DevCreedApi_Net6Movies.Models
{
    public class RegisterModel
    {
        [Required, MaxLength(30)]
        public string FirstName { get; set; }
        [Required, MaxLength(20)]
        public string LastName { get; set; }
        [Required, MaxLength(30)]
        public string Username { get; set; }
        [Required, MaxLength(30)]
        public string Email { get; set; }
        [Required, MaxLength(30)]
        public string Password { get; set; }
    }
}
