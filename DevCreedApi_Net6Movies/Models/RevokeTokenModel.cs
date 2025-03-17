using System.ComponentModel.DataAnnotations;

namespace DevCreedApi_Net6Movies.Models
{
    public class RevokeTokenModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

    }
}