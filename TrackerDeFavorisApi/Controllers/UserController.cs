using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using TrackerDeFavorisApi.Models;
using System.Security.Claims;
using TrackerDeFavorisApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;


namespace TrackerDeFavorisApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly PasswordHasher<UserInfo> _hasher;
    private readonly ApplicationContext _applicationContext;
    private readonly JwtService _jwtService;

    public UserController(
        ApplicationContext applicationContext,
        PasswordHasher<UserInfo> hasher,
        JwtService jwtService
    )
    {
        _applicationContext = applicationContext;
        _hasher = hasher;
        _jwtService = jwtService;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PublicUser>>> GetUsers()
    {
        var users = await _applicationContext.UserList.ToListAsync();
        if (users == null || users.Count == 0)
        {
            users = new List<User>();
        }
        return Ok(users.Select(u => u.AsPublicUser()));
    }


    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PublicUser>> GetUser(int id)
    {
        var user = await _applicationContext.UserList.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user.AsPublicUser());
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [HttpPost("login")] // get user only if username and password exists and are correct
    [AllowAnonymous]
    public async Task<ActionResult<UserLogin>> LoginUser(UserInfo userinfo)
    {
        var users = await _applicationContext.UserList.Where(u => u.Username == userinfo.Username).ToListAsync();

        User? user = users.Where(u => _hasher.VerifyHashedPassword(userinfo, u.Password, userinfo.Password) == PasswordVerificationResult.Success).FirstOrDefault();

        if (user != null)
        {            
            return Ok(
                new UserLogin(
                    user.AsPublicUser(),
                    _jwtService.GetJWT(user.AsPublicUser())
                )
            );
        }
        return BadRequest("Utilisateur inconnu");
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [HttpPost("register")] // add a new user
    [AllowAnonymous]
    public async Task<ActionResult<UserLogin>> RegisterUser(UserInfo userInfo)
    {
        var nbUsersSameName = await _applicationContext.UserList.Where(u => u.Username == userInfo.Username).CountAsync();
        if (nbUsersSameName > 0)
        {
            return BadRequest("un utilisateur possède déjà ce nom");
        }

        string userPassword = _hasher.HashPassword(userInfo, userInfo.Password);
        User user = new User(userInfo.Username, userPassword);

        // on l'ajoute a notre contexte (BDD)
        _applicationContext.UserList.Add(user);
        
        // on enregistre les modifications dans la BDD ce qui remplira le champ Id de notre objet
        await _applicationContext.SaveChangesAsync();
        
        // on retourne un code 201 pour indiquer que la création a bien eu lieu
        return CreatedAtAction(
            nameof(GetUser), new { id = user.Id }, 
            new UserLogin(
                user.AsPublicUser(),
                _jwtService.GetJWT(user.AsPublicUser())
            )
        );
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    [HttpPut("{id}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<UserLogin>> UpdateUser(int id, UserUpdate userUpdate)
    {
        TokenValidity access = _jwtService.GetTokenValidity(HttpContext.User, id);
        if (access == TokenValidity.Invalid)
        {
            return Unauthorized("Not Authorized");
        }
        if (access == TokenValidity.Forbidden)
        {
            return StatusCode(403, "Vous n'avez pas le droit d'accéder à cette ressource");
        }
        else
        {
            User? user = await _applicationContext.UserList.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Username != userUpdate.Username) // changement de nom
            {
                var nbUsersSameName = await _applicationContext.UserList.Where(u => u.Username == userUpdate.Username).CountAsync();
                if (nbUsersSameName > 0)
                {
                    return BadRequest("un utilisateur possède déjà ce nom");
                }
            }

            user.Username = userUpdate.Username;
            user.Password = _hasher.HashPassword(userUpdate, userUpdate.Password);
            if (user.Role == Role.ADMIN)
            {
                user.Role = userUpdate.Role;
            }

            _applicationContext.Entry(user).State = EntityState.Modified;
            try
            {
                // on enregistre les modifications
                await _applicationContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // si une erreur de concurrence survient on retourne un code 500
                return StatusCode(500, "Erreur de concurrence");
            }

            return Ok(
                new UserLogin(
                    user.AsPublicUser(),
                    _jwtService.GetJWT(user.AsPublicUser())
                )
            );
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [HttpDelete("{id}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        TokenValidity access = _jwtService.GetTokenValidity(HttpContext.User, id);
        if (access == TokenValidity.Invalid)
        {
            return Unauthorized("Not Authorized");
        }
        else if (access == TokenValidity.Forbidden)
        {
            return StatusCode(403, "Vous n'avez pas le droit d'accéder à cette ressource");
        }
        else
        {
            // on récupère l'user que l'on souhaite supprimer
            User? user = await _applicationContext.UserList.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // on indique a notre contexte que l'objet a été supprimé
            _applicationContext.UserList.Remove(user);
            // on enregistre les modifications
            await _applicationContext.SaveChangesAsync();
            // on retourne un code 204 pour indiquer que la suppression a bien eu lieu
            return NoContent();
        }
    }
}
