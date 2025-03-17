using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DevCreedApi_Net6Movies.Models
{
   
    public class ApplicationUser:IdentityUser
    {
        [Required,MaxLength(30)]
        public string FirstName { get; set; }
        [Required,MaxLength(20)]
        public string LastName { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }

    }
}
