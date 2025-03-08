using System.Net.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackerDeFavorisFront.Models;

namespace TrackerDeFavorisFront.Services
{
    // Gère les requêtes avec notre back, concernant les Users
    public class UserService
    {
        private readonly HttpClient _client;
        private readonly string _url;
        public UserService( // constructeur appelé automatiquement par le builder dans Program.cs
            HttpClient client, 
            IConfiguration configuration 
        )
        {
            _client = client;
            string? adresseBack = configuration["AdresseBack"];
            if (adresseBack == null)
            {
                throw new InvalidAppSettingsException("Adresse du backend introuvable");
            }
            _url = adresseBack + "/user";
        }

        public async Task<List<PublicUser>?> GetAllUsers()
        {
            List<PublicUser>? listUsers = null;
            // allow anonymous donc pas de header avec jwt
            try
            {
                HttpResponseMessage httpResp = await _client.GetAsync(_url);
                httpResp.EnsureSuccessStatusCode();
                listUsers = await httpResp.Content.ReadFromJsonAsync<List<PublicUser>?>();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                return null;
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new DeserialisationException(e.Message);
            }
            return listUsers;
        }

        public async Task<UserLogin?> GetUserById(int id)
        {
            UserLogin? publicUserResp = null;
            HttpResponseMessage httpResp = await _client.GetAsync($"{_url}/{id}");
            try
            {
                httpResp.EnsureSuccessStatusCode();
                publicUserResp = await httpResp.Content.ReadFromJsonAsync<UserLogin>();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                return null;
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new DeserialisationException(e.Message);
            }
            return publicUserResp;
        }

        public async Task<UserLogin?> LoginUser(UserInfo userInfo)
        {
            UserLogin? publicUserResp = null;
            try
            {
                HttpResponseMessage httpResp = await _client.PostAsJsonAsync($"{_url}/login", userInfo);
                httpResp.EnsureSuccessStatusCode();
                publicUserResp = await httpResp.Content.ReadFromJsonAsync<UserLogin>();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                return null;
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new DeserialisationException(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            return publicUserResp;
        }

        public async Task<UserLogin?> RegisterUser(UserInfo userInfo)
        {
            UserLogin? publicUserResp = null;

            HttpResponseMessage httpResp = await _client.PostAsJsonAsync($"{_url}/register", userInfo);
            try
            {
                httpResp.EnsureSuccessStatusCode();
                publicUserResp = await httpResp.Content.ReadFromJsonAsync<UserLogin>();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                return null;
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new DeserialisationException(e.Message);
            }
            return publicUserResp;
        }

        public async Task<UserLogin?> UpdateUser(int id, UserUpdate userUpdate, string token)
        {
            UserLogin? user = null;

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            HttpResponseMessage httpResp = await _client.PutAsJsonAsync($"{_url}/{id}", userUpdate);
            try
            {
                httpResp.EnsureSuccessStatusCode();
                user = await httpResp.Content.ReadFromJsonAsync<UserLogin>();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                return null;
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new DeserialisationException(e.Message);
            }
            return user;
        }

        public async Task<bool> DeleteUser(int id, string token)
        {

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            HttpResponseMessage httpResp = await _client.DeleteAsync($"{_url}/{id}");
            try
            {
                httpResp.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                return false;
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new DeserialisationException(e.Message);
            }
            return true;
        }
    }
}
