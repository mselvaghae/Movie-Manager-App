namespace TrackerDeFavorisApi.Models
{
    public class FilmInfo
    {
        public string Titre { get; set; }
        public string Poster { get; set; }
        public string ImdbId { get; set; }
        public string AnneeSortie { get; set; }

        public FilmInfo(string titre, string poster, string imdbId, string anneeSortie)
        {
            Titre = titre;
            Poster = poster;
            ImdbId = imdbId;
            AnneeSortie = anneeSortie;
        }
        public Film AsFilm()
        {
            return new Film(Titre, Poster, ImdbId, AnneeSortie);
        }
    }
}
