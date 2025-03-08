using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrackerDeFavorisApi.Models;

namespace TrackerDeFavorisApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;

        public FilmController(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Film>>> GetFilms()
        {
            var films = await _applicationContext.FilmList.ToListAsync();

            return Ok(films);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Film>>> GetSearchedFilm(string title)
        {
            var films = await _applicationContext.FilmList.Where(
                f => EF.Functions.Like(f.Titre, $"%{title}%"))
                .ToListAsync();

            if (films.IsNullOrEmpty())
            {
                return NotFound();
            }
            return Ok(films);
        }

        [HttpGet("info")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Film>>> GetFilmsByIds([FromQuery] int[] ids)
        {
            var films = await _applicationContext.FilmList.Where(f => ids.Contains(f.Id)).ToListAsync();
            if (films.IsNullOrEmpty())
            {
                return NotFound();
            }
            return Ok(films);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Film>> GetFilm(int id)
        {
            var film = await _applicationContext.FilmList.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            return Ok(film);
        }
    }
}
