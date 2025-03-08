using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using TrackerDeFavorisFront.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TrackerDeFavorisFront.Services;

public class AuthProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _sessionStorage;

    public AuthProvider(ProtectedLocalStorage protectedLocalStorage)
    {
        _sessionStorage = protectedLocalStorage;
    }

    public ClaimsPrincipal GenerateClaimsPrincipal(PublicUser user)
    {

        var claims = new[]
        {
            new Claim(ClaimTypes.Sid,  user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        ClaimsIdentity identity = new ClaimsIdentity(claims, "custom");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
        return claimsPrincipal;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _sessionStorage.GetAsync<PublicUser>("User"); // Récupère l'utilisateur
        ClaimsPrincipal? claim;

        if (user.Value != null) // Si on a un utilisateur dans le local storage
        {
            claim = GenerateClaimsPrincipal(user.Value); // Génère un ClaimsPrincipal à partir de l'user du local storage
        }
        else
        {
            claim = new ClaimsPrincipal(new ClaimsIdentity()); // Génère un ClaimsPrincipal vide
        }
        return new AuthenticationState(claim);
    }
    
    public async Task SetUserLoginToSessionStorage(UserLogin user)
    {
        await _sessionStorage.SetAsync("User", user.PublicUser); // Stocke l'utilisateur
        await _sessionStorage.SetAsync("Token", user.JWT); // Stocke le token
        ClaimsPrincipal claim = GenerateClaimsPrincipal(user.PublicUser); // Génère un ClaimsPrincipal
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claim))); // Notifie le changement d'état de connexion
    }

    

    public async Task RemoveUserLoginFromSessionStorage()
    {
        await _sessionStorage.DeleteAsync("User");    // Supprime l'utilisateur
        await _sessionStorage.DeleteAsync("Token");   // Supprime le token

        // Notifie le changement d'état de connexion
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity()) // avec un ClaimsPrincipal vide
        )));
    }

    public async Task<int?> GetId()
    {
        AuthenticationState state = await GetAuthenticationStateAsync();
        string? userId = state.User.FindFirst(ClaimTypes.Sid)?.Value;
        try
        {
            return (userId != null) ? int.Parse(userId) : null;
        }
        catch (Exception ) // invalid jwt with non integer id => int.Parse throws
        {
            return null;
        }
    }
	public async Task<string?> GetUsername()
	{
		AuthenticationState state = await GetAuthenticationStateAsync();
		string? userName = state.User.FindFirst(ClaimTypes.Name)?.Value;
        return userName;
	}
	public async Task<Role?> GetRole()
	{
		AuthenticationState state = await GetAuthenticationStateAsync();
        string? userRole = state.User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole == null)
        {
            return null;
        }

        try
        {
            Role role = (Role)Enum.Parse(typeof(Role), userRole);
            return role;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
	}

    public async Task<string> GetToken()
    {
        var token = await _sessionStorage.GetAsync<string>("Token");
        if (token.Value == null)
        {
            // On renvoie un token vide pour avoir un en-tête en "Authorization: Bearer "
            return "";
        }
        return token.Value;
    }
}
