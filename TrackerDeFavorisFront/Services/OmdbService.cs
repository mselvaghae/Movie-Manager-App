using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using TrackerDeFavorisFront.Models;

namespace TrackerDeFavorisFront.Services;

class OmdbService
{
    private readonly HttpClient _httpClient;
    private readonly string _url;

    public OmdbService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        string? url = config["AdresseBack"];
        if (url == null)
        {
            throw new ArgumentNullException("AdresseBack non définie");
        }
        _url = url + "/Omdb";
    }
    public async Task<List<FilmInfo>> GetOmdbFilms(string title, string token)
    {
        List<FilmInfo>? list;
        try
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            list = await _httpClient.GetFromJsonAsync<List<FilmInfo>>($"{_url}/search/{title}");
        }
        catch (HttpRequestException)
        {
            throw new ServiceException("Serveur inaccessible");
        }
        catch (JsonException)
        {
            throw new ServiceException("Exception de déserialisation");
        }
        if (list == null)
        {
            throw new ServiceException("Erreur Omdb");
        }
        return list;
    }

    public async Task<FilmInfo> ImportOmdbFilm(string imdbId, string token)
    {
        FilmInfo? filmInfo;

        try
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            filmInfo = await _httpClient.GetFromJsonAsync<FilmInfo>($"{_url}/import/{imdbId}");
        }
        catch (HttpRequestException)
        {
            throw new ServiceException("Serveur inaccessible");
        }
        catch (JsonException)
        {
            throw new ServiceException("Exception de déserialisation");
        }
        if (filmInfo == null)
        {
            throw new ServiceException("Erreur Omdb");
        }
        return filmInfo;
    }
}