using System.Text.Json.Serialization;

namespace DevCreedApi_Net6Movies.Models
{
    public class AuthenticationModel
    {
       
        public string Msg { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
      
        public string Email { get; set; }
         
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
        ////88888 *** Refresh Token props ****  888888888

        [JsonIgnore]
        public string RefreshToken { get;set; }
        public DateTime RefreshTokenExpirationOn { get;set; }
    }
}
