using System;
using System.Threading.Tasks;
using LecteurIptv.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LecteurIptv.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StreamingController : ControllerBase
    {
        private readonly ILogger<StreamingController> _logger;
        private readonly IStreamingService _streamingService;

        public StreamingController(ILogger<StreamingController> logger, IStreamingService streamingService)
        {
            _logger = logger;
            _streamingService = streamingService;
        }

        /// <summary>
        /// Proxy un flux vidéo
        /// </summary>
        /// <param name="url">URL du flux à proxifier</param>
        /// <returns>Flux vidéo</returns>
        [HttpGet("proxy")]
        public async Task ProxyStream([FromQuery] string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    await Response.WriteAsync("URL is required");
                    return;
                }

                await _streamingService.ProxyStreamAsync(url, Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error proxying stream: {url}");
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                await Response.WriteAsync($"Error proxying stream: {ex.Message}");
            }
        }

        /// <summary>
        /// Vérifie si un flux est disponible
        /// </summary>
        /// <param name="url">URL du flux à vérifier</param>
        /// <returns>True si le flux est disponible, false sinon</returns>
        [HttpGet("check")]
        public async Task<ActionResult<bool>> CheckStream([FromQuery] string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return BadRequest("URL is required");
                }

                var isAvailable = await _streamingService.IsStreamAvailableAsync(url);
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking stream: {url}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error checking stream: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient les informations sur un flux
        /// </summary>
        /// <param name="url">URL du flux</param>
        /// <returns>Informations sur le flux</returns>
        [HttpGet("info")]
        public async Task<ActionResult<StreamInfo>> GetStreamInfo([FromQuery] string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return BadRequest("URL is required");
                }

                var streamInfo = await _streamingService.GetStreamInfoAsync(url);
                return Ok(streamInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stream info: {url}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting stream info: {ex.Message}");
            }
        }
    }
}
