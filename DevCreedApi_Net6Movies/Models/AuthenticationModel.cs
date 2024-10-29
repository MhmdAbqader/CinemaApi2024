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
    }
}
