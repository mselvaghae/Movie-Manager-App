using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TrackerDeFavorisFront.Models;

namespace TrackerDeFavorisFront.Services;

class FavoriteService
{
    private readonly HttpClient _httpClient;
    private readonly string _url;

    public FavoriteService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        string? url = config["AdresseBack"];
        if (url == null)
        {
            throw new ArgumentNullException("AdresseBack non définie");
        }
        _url = url + "/Favorite";
    }
    public async Task<List<Favorite>?> GetFavoris(int userId, string token)
    {
        List<Favorite>? list;
        try
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            list = await _httpClient.GetFromJsonAsync<List<Favorite>>($"{_url}/list/{userId}");
        }
        catch (HttpRequestException e)
        {
            list = null;
            Console.WriteLine(e);
        }
        catch (JsonException)
        {
            throw new ServiceException("Exception de déserialisation");
        }

        return list;
    }

    public async Task<HttpResponseMessage> CreateFavori(Favorite favori, string token)
    {
        HttpResponseMessage msg;
        try
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            msg = await _httpClient.PostAsync($"{_url}/add?filmId={favori.FilmId}&userId={favori.UserId}", null);
        }
        catch (HttpRequestException)
        {
            throw new ServiceException("Serveur inaccessible");
        }
        catch (JsonException)
        {
            throw new ServiceException("Exception de déserialisation");
        }
        return msg;
    }
    public async Task DeleteFavori(int filmId, int userId, string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            await _httpClient.DeleteAsync( $"{_url}/remove?filmId={filmId}&userid={userId}");
        }
        catch (HttpRequestException)
        {
            throw new ServiceException("Serveur inaccessible");
        }
    }
}

