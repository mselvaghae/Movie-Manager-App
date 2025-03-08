namespace TrackerDeFavorisFront.Models
{
    public class Film
    {
        private FilmInfo _filmInfo ;
        public FilmInfo FilmInfo {  get { return _filmInfo; } }
        public int Id { get; set; }
        public string Titre
        {
            get { return _filmInfo.Titre; }
            set { _filmInfo.Titre = value; }
        }
        public string Poster
        {
            get { return _filmInfo.Poster; }
            set { _filmInfo.Poster = value; }
        }
        public string ImdbId
        {
            get { return _filmInfo.ImdbId; }
            set { _filmInfo.ImdbId = value; }
        }
        public string AnneeSortie
        {
            get { return _filmInfo.AnneeSortie; }
            set { _filmInfo.AnneeSortie = value; }
        }

        public override bool Equals( object? other )
        {
            if (other != null && other is Film)
            {
                Film f = (Film)other;
                return this.Id == f.Id;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public Film(string titre, string poster, string imdbId, string anneeSortie)
        {
            _filmInfo = new FilmInfo(titre, poster, imdbId, anneeSortie);
        }
    }
}
