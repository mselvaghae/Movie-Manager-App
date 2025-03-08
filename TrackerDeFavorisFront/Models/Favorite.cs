namespace TrackerDeFavorisFront.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FilmId { get; set; }

        public Favorite(int userId,  int filmId)
        {
            UserId = userId;
            FilmId = filmId;
        }
    }
}
