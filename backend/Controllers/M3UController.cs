using Microsoft.AspNetCore.Mvc;
using StreamingAPI.Services;
using System.Threading.Tasks;

namespace StreamingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class M3UController : ControllerBase
    {
        private readonly M3UParser _parser;

        public M3UController(M3UParser parser)
        {
            _parser = parser;
        }

        [HttpGet("parse")]
        public async Task<IActionResult> ParseM3U([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("L'URL de la playlist est requise");
            }

            try
            {
                var channels = await _parser.ParseFromUrlAsync(url);
                return Ok(channels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors du parsing de la playlist: {ex.Message}");
            }
        }

        [HttpGet("samples")]
        public IActionResult GetSamples()
        {
            var samples = _parser.GetSamplePlaylists();
            return Ok(samples);
        }
    }
}
