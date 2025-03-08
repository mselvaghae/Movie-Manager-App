using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TrackerDeFavorisApi.Models;
using TrackerDeFavorisApi.Services;

namespace TrackerDeFavorisApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FavoriteController : ControllerBase
{
    private readonly ApplicationContext _applicationContext;
    private readonly JwtService _jwtService;

    public FavoriteController(ApplicationContext applicationContext, JwtService jwtService)
    {
        _applicationContext = applicationContext;
        _jwtService = jwtService;
    }

    // GET: api/<FavoriteController>
    [HttpGet("list/{userId}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<List<Favorite>>> GetFavoris(int userId)
    {
        TokenValidity access = _jwtService.GetTokenValidity(HttpContext.User, userId);
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
            int count = await _applicationContext.UserList.Where(u => u.Id == userId).CountAsync();
            if (count != 1)
            {
                return NotFound("La requête concerne un utilisateur inconnu");
            }
            var favoris = await _applicationContext.FavoriteList.Where(f => f.UserId == userId).ToListAsync();

            return Ok(favoris);
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Favorite>> GetFavori(int id)
    {
        var favori = await _applicationContext.FavoriteList.FindAsync(id);
        if (favori == null)
        {
            return NotFound();
        }
        return Ok(favori);
    }

    // POST api/<FavoriteController>
    [HttpPost("add")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<Favorite>> CreateFavoris(int filmId, int userId)
    {
        TokenValidity access = _jwtService.GetTokenValidity(HttpContext.User, userId);
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
            int count = await _applicationContext.FilmList.Where(f => f.Id == filmId).CountAsync();
            if (count != 1)
            {
                return BadRequest("Le favori concerne un film inconnu");
            }

            count = await _applicationContext.UserList.Where(u => u.Id == userId).CountAsync();
            if (count != 1)
            {
                return BadRequest("Le favori concerne un utilisateur inconnu");
            }

            Favorite favorite = new Favorite(userId, filmId);

            _applicationContext.FavoriteList.Add(favorite);
            await _applicationContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFavori), new { id = favorite.Id }, favorite);
        }
    }

    [HttpDelete("remove")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> DeleteFavori(int filmId, int userId)
    {
        TokenValidity access = _jwtService.GetTokenValidity(HttpContext.User, userId);
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
            // on récupère le favori que l'on souhaite supprimer
            Favorite? favori = await _applicationContext.FavoriteList.Where(f => f.FilmId == filmId && f.UserId == userId).FirstOrDefaultAsync();
            if (favori == null)
            {
                return NotFound();
            }
            // on indique a notre contexte que l'objet a été supprimé
            _applicationContext.FavoriteList.Remove(favori);
            // on enregistre les modifications
            await _applicationContext.SaveChangesAsync();
            // on retourne un code 204 pour indiquer que la suppression a bien eu lieu
            return NoContent();
        }
    }
}