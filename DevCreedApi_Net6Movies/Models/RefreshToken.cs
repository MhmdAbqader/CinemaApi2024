using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DevCreedApi_Net6Movies.Models
{
    [Owned]
    public class RefreshToken
    {
        [Required]
        public string Token { get; set; }
        public DateTime ExpireOn { set; get; }// when the token is expired 
        public DateTime CreatedOn { set; get; }
        public DateTime? RevokedOn { set; get; } //actual time that token is revoked  متي حصله ابطال بالفعل 
        public bool IsExpired  => DateTime.UtcNow >= ExpireOn;
        public bool IsActive => RevokedOn is null && IsExpired == false;
    }
}