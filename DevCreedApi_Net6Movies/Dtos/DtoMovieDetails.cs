namespace DevCreedApi_Net6Movies.Dtos
{
    public class DtoMovieDetails
    {
        public int Id { get; set; }

         
        public string Title { get; set; }

        public int Year { get; set; }

        public double Rate { get; set; }

        
         
        public string StoreLine { get; set; }

       

        public int GenreID { get; set; }
        
        public string GenreName_from_dto { get; set; }
        public byte[] Poster { get; set; }
    }
}
