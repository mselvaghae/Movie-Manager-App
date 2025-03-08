using Microsoft.AspNetCore.Http.HttpResults;
using TrackerDeFavorisFront.Models;

namespace TrackerDeFavorisFront.Services
{
    // Gère les requêtes avec notre back, concernant les Users
    public class FilmService
    {
        private readonly HttpClient _client;
        private readonly string _url;
        public FilmService(HttpClient client, IConfiguration configuration) // constructeur appelé automatiquement par le builder dans Program.cs
        {
            _client = client;
            string? adresseBack = configuration["AdresseBack"];
            if (adresseBack == null)
            {
                throw new InvalidAppSettingsException("Adresse du Back introuvable");
            }
            _url = adresseBack + "/Film";
        }

        public async Task<List<Film>?> GetFilms(string token)
        {
            List<Film>? listFilms = null;
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                listFilms = await _client.GetFromJsonAsync<List<Film>>(_url);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new DeserialisationException(e.Message);
            }
            return listFilms;
        }

        public async Task<List<Film>> SearchFilms(string recherche, string token)
        {
            List<Film>? listFilms = null;
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                listFilms = await _client.GetFromJsonAsync<List<Film>>($"{_url}/search?title={recherche}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new DeserialisationException(e.Message);
            }
            return listFilms ?? new List<Film>();
        }

        public async Task<FilmInfo?> GetFilmById(int id, string token)
        {
            Film? filmUserResp = null;
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage httpResp = await _client.GetAsync($"{ _url}/{id}");
            try
            {
                httpResp.EnsureSuccessStatusCode();
                filmUserResp = await httpResp.Content.ReadFromJsonAsync<Film>();
            }
            catch (HttpRequestException)
            {
                throw new ServiceException();
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new DeserialisationException(e.Message);
            }
            if (filmUserResp ==  null)
            {
                return null;
            }
            return filmUserResp.FilmInfo;
        }
    }
}
