using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackerDeFavorisApi.Models;
using TrackerDeFavorisApi.Services;

namespace TrackerDeFavorisApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OmdbController : ControllerBase
    {
        private readonly ApplicationContext _applicationContext;
        private readonly OmdbService _service;
        public OmdbController(ApplicationContext applicationContext, OmdbService service) // constructeur appelé automatiquement par le builder dans Program.cs
        {
            _applicationContext = applicationContext;
            _service = service;
        }

        [HttpGet("search/{title}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<List<FilmInfo>>> GetFilmsByTitleOmdb(string title)
        {
            List<FilmInfo> films = await _service.SearchByTitle(title);

            return films;
        }

        [HttpGet("import/{imdbId}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<Film>> GetFilmAddedIfNecessary(string imdbId)
        {

            Film? film = await _applicationContext.FilmList.FirstOrDefaultAsync(f => f.ImdbId == imdbId);

            if (film != null)
            {
                return Ok(film);
            }

            FilmInfo? filmInfo = await _service.GetByImdbId(imdbId);
            if (filmInfo == null)
            {
                return NotFound();
            }

            film = filmInfo.AsFilm();

            _applicationContext.FilmList.Add(film);
            await _applicationContext.SaveChangesAsync();

            return Ok(film); //CreatedAtAction(nameof(FilmController.GetFilm), nameof(FilmController), new { id = film.Id }, film);
        }
    }
}
