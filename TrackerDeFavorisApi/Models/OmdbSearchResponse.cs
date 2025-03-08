namespace TrackerDeFavorisApi.Models
{
    // Le serveur Omdb nous renvoie un json contenant des classes avec ces noms et attributs
    // Nous avons donc ajouté des méthodes afin de pouvoir les convertir en classe Film utilisable dans notre application
    public class OmdbSearchResponse // réponse de la requête Omdb
    {
        public required IEnumerable<OmdbFilm> Search { get; set; }
        public required string totalResults { get; set; }
        public required string Response { get; set; }

        public OmdbSearchResponse(IEnumerable<OmdbFilm> search, string totalResults, string response)
        {
            Search = search;
            this.totalResults = totalResults;
            Response = response;
        }
    }

    public class OmdbFilm
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string imdbID { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }

        public OmdbFilm(string title, string year, string imdbID, string type, string poster)
        {
            Title = title;
            Year = year;
            this.imdbID = imdbID;
            Type = type;
            Poster = poster;
        }

        public FilmInfo AsFilmInfo()
        {
            return new FilmInfo(Title, Poster, imdbID, Year);
        }
    }
}
