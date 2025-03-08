using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackerDeFavorisApi.Models;

namespace TrackerDeFavorisApi.Services
{
    // Gère les requêtes au serveur Omdb (externe à notre application)
    public class OmdbService
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;
        public OmdbService(HttpClient client, IConfiguration configuration) // constructeur appelé automatiquement par le builder dans Program.cs
        {
            _client = client;
            string? key = configuration["APIKey"];
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidAppSettingsException("Clé Omdb non trouvée");
            }
            _apiKey = key;
        }
        public async Task<List<FilmInfo>> SearchByTitle(string title)
        {
            OmdbSearchResponse? omdbResponse = null;
            HttpResponseMessage response = await _client.GetAsync($"https://www.omdbapi.com/?s={title}&page=1&type=movie&apikey={_apiKey}");

            List<FilmInfo> filmList = new List<FilmInfo>();
            try
            {
                response.EnsureSuccessStatusCode();
                omdbResponse = await response.Content.ReadFromJsonAsync<OmdbSearchResponse>();

            }
            catch (HttpRequestException)
            {
                return filmList;
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new DeserialisationException(e.Message);
            }
            if (omdbResponse == null)
            {
                throw new DeserialisationException();
            }
            foreach (OmdbFilm f in omdbResponse.Search)
            {
                filmList.Add(f.AsFilmInfo());
            }
            return filmList;
        }

        public async Task<FilmInfo> GetByImdbId(string imdbId)
        {
            HttpResponseMessage response = await _client.GetAsync($"https://www.omdbapi.com/?i={imdbId}&apikey={_apiKey}");
            OmdbFilm? omdbResponse = await response.Content.ReadFromJsonAsync<OmdbFilm>();
            if ( omdbResponse == null )
            {
                // Ne devrait pas se produire
                throw new DeserialisationException();
            }

            return omdbResponse.AsFilmInfo();
        }

    }
}
