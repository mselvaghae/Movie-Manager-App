using TrackerDeFavorisApi.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text; // Encoding.UTF8.GetBytes(string s)

namespace TrackerDeFavorisApi.Services;

public enum TokenValidity
{
    Ok,
    Invalid,
    Forbidden
};

public class JwtService
{
    private readonly string _jwtKey;
    private readonly SymmetricSecurityKey _symKey;
    private readonly SigningCredentials _signCreds;

    private JwtSecurityTokenHandler _jwtTokenHandler;
    public JwtService(IConfiguration configuration)
    {
        string? str_key = configuration["JWTKey"];
        if (string.IsNullOrWhiteSpace(str_key))
        {
            throw new InvalidAppSettingsException("Clé JWT non trouvée");
        }
        _jwtKey = str_key;

        _symKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));

        _signCreds = new SigningCredentials(_symKey, SecurityAlgorithms.HmacSha256);

        _jwtTokenHandler = new JwtSecurityTokenHandler();
    }

    public string GetJWT(PublicUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Sid  , user.Id.ToString()           ),
            new Claim(ClaimTypes.Name , user.Username                ),
            new Claim(ClaimTypes.Role , User.RoleToString(user.Role) )
        };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "localhost:5152",          // Qui émet le token ici c'est notre API
            audience: "localhost:5152",        // Qui peut utiliser le token ici c'est notre API
            claims: claims,                    // Les informations sur l'utilisateur
            expires: DateTime.Now.AddHours(3), // Date d'expiration du token
            signingCredentials: _signCreds     // La clé secrète
        );

        return _jwtTokenHandler.WriteToken(token);
    }

    public TokenValidity GetTokenValidity(ClaimsPrincipal HttpCurrUser, int id)
    {
        string? currUserIdString = HttpCurrUser.FindFirstValue(ClaimTypes.Sid);
        string? currUserRole = HttpCurrUser.FindFirstValue(ClaimTypes.Role);

        if (currUserIdString == null || currUserRole == null)
        {
            return TokenValidity.Invalid;
        }
        try
        {
            int currUserId = int.Parse(currUserIdString);
            if (!((currUserRole == "Admin") || (currUserId == id)))
            {
                return TokenValidity.Forbidden;
            }
        }
        catch (Exception) // fail du int.Parse()
        {
            return TokenValidity.Invalid;
        }

        return TokenValidity.Ok;
    }
}


