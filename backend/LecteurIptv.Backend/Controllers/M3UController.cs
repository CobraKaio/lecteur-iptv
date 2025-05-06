using System;
using System.IO;
using System.Threading.Tasks;
using LecteurIptv.Backend.Models;
using LecteurIptv.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class M3UController : ControllerBase
    {
        private readonly ILogger<M3UController> _logger;
        private readonly IM3UParser _m3uParser;

        public M3UController(ILogger<M3UController> logger, IM3UParser m3uParser)
        {
            _logger = logger;
            _m3uParser = m3uParser;
        }

        /// <summary>
        /// Parse un fichier M3U à partir d'une URL
        /// </summary>
        /// <param name="url">URL du fichier M3U</param>
        /// <returns>Playlist M3U parsée</returns>
        [HttpGet("parse")]
        public async Task<ActionResult<M3UPlaylist>> ParseFromUrl([FromQuery] string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return BadRequest("URL is required");
                }

                var playlist = await _m3uParser.ParseFromUrlAsync(url);
                return Ok(playlist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error parsing M3U from URL: {url}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error parsing M3U: {ex.Message}");
            }
        }

        /// <summary>
        /// Parse un fichier M3U uploadé
        /// </summary>
        /// <param name="file">Fichier M3U</param>
        /// <returns>Playlist M3U parsée</returns>
        [HttpPost("upload")]
        public async Task<ActionResult<M3UPlaylist>> UploadAndParse(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File is required");
                }

                // Créer un fichier temporaire
                var tempFilePath = Path.GetTempFileName();
                
                try
                {
                    // Copier le contenu du fichier uploadé dans le fichier temporaire
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Parser le fichier
                    var playlist = await _m3uParser.ParseFromFileAsync(tempFilePath);
                    return Ok(playlist);
                }
                finally
                {
                    // Supprimer le fichier temporaire
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        System.IO.File.Delete(tempFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing uploaded M3U file");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error parsing M3U: {ex.Message}");
            }
        }
    }
}
