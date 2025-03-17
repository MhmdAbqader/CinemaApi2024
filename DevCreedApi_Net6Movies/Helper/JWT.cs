namespace DevCreedApi_Net6Movies.Helper
{
    public class JWT
    {
        public string key { get; set; }
        public string issuer { get; set; }
        public string audience { get; set; }
        public double expireIn { get; set; }

    }
}
