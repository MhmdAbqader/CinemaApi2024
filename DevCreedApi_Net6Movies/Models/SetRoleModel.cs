using System.ComponentModel.DataAnnotations;

namespace DevCreedApi_Net6Movies.Models
{
    public class SetRoleModel
    {
        [Required]
        public string UserId { get; set; }
        public string RoleName { get; set; }
    }
}
